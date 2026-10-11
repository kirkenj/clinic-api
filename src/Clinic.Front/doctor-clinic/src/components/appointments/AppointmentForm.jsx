import React, { useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Paper, Typography, Alert, CircularProgress, Button,
  Autocomplete, TextField, Step, Stepper, StepLabel,
  ToggleButtonGroup, ToggleButton
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { format, parseISO } from 'date-fns';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';

import officeService from '../../services/officeService';
import serviceService from '../../services/serviceService';
import doctorService from '../../services/doctorService';
import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import useDebounce from '../../hooks/useDebounce';
import SubmitButton from '../../components/common/SubmitButton';

const steps = ['Select Criteria', 'Choose Time Slot', 'Confirm'];

const AppointmentForm = () => {
  const navigate = useNavigate();
  const { patientId } = useParams();

  // --- Состояния ---
  const [formData, setFormData] = useState({
    officeId: null,
    serviceId: null,
    doctorId: null,
    date: null,
    timeSlot: null,
  });
  const [activeStep, setActiveStep] = useState(0);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);

  const [officeOptions, setOfficeOptions] = useState([]);
  const [serviceOptions, setServiceOptions] = useState([]);
  const [doctorOptions, setDoctorOptions] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);
  const [slotsLoading, setSlotsLoading] = useState(false);
  
  const [officeInput, setOfficeInput] = useState('');
  const [serviceInput, setServiceInput] = useState('');
  const debouncedOfficeInput = useDebounce(officeInput, 400);
  const debouncedServiceInput = useDebounce(serviceInput, 400);

  // --- Загрузка справочников ---
  useEffect(() => {
    officeService.getPaged({ Address: debouncedOfficeInput, page: 1, pageSize: 20 })
      .then(res => setOfficeOptions(res.items || []));
  }, [debouncedOfficeInput]);

  useEffect(() => {
    serviceService.getPaged({ Name: debouncedServiceInput, page: 1, pageSize: 20 })
      .then(res => setServiceOptions(res.items || []));
  }, [debouncedServiceInput]);

  const selectedService = useMemo(() => 
    serviceOptions.find(s => s.id === formData.serviceId), 
  [formData.serviceId, serviceOptions]);

  // Зависимая загрузка докторов
  useEffect(() => {
    if (formData.officeId && selectedService?.specializationId) {
      const params = {
        OfficeIds: [formData.officeId],
        SpecializationIds: [selectedService.specializationId],
        page: 1,
        pageSize: 50
      };
      doctorService.getPaged(params).then(res => setDoctorOptions(res.items || []));
    } else {
      setDoctorOptions([]);
    }
  }, [formData.officeId, selectedService]);

  // Зависимая загрузка слотов
  useEffect(() => {
    if (formData.doctorId && formData.date) {
      setSlotsLoading(true);
      setAvailableSlots([]);

      // Конвертируем локальную дату в UTC перед отправкой
      if (!formData.date || isNaN(formData.date.getTime())) return;
      const utcTimestamp = formData.date.getTime() - (formData.date.getTimezoneOffset() * 60 * 1000);
      const utcDate = new Date(utcTimestamp);
      
      appointmentService.getAvailableSlots(utcDate.toISOString(), formData.doctorId)
        .then(slots => setAvailableSlots(slots || []))
        .catch(err => setError(handleApiError(err)))
        .finally(() => setSlotsLoading(false));
    }
  }, [formData.doctorId, formData.date]);

  // --- Хендлеры ---
  const handleAutocompleteChange = (name, value) => {
    if (error && error[name]) setError(prev => ({ ...prev, [name]: null }));
    const isDoctorField = name === 'doctorId';
    setFormData(prev => ({ 
      ...prev, 
      [name]: value ? value.id : null, 
      // Сбрасываем зависимые поля
      doctorId: isDoctorField ? (value ? value.id : null) : null,
      date: null, 
      timeSlot: null 
    }));
  };

  const handleDoctorChange = (event, value) => {
    if (error && error.doctorId) setError(prev => ({ ...prev, doctorId: null }));
    setFormData(prev => ({ 
      ...prev, 
      doctorId: value ? value.id : null, 
      date: null, 
      timeSlot: null 
    }));
  };

  const handleDateChange = (date) => {
    if (error && error.date) setError(prev => ({ ...prev, date: null }));
    setFormData(prev => ({ ...prev, date, timeSlot: null }));
  };
  
  const isWeekend = (date) => {
    const day = date.getDay();
    return day === 0 || day === 6;
  };

  const handleSlotChange = (event, slot) => {
    if (error && error.timeSlot) setError(prev => ({ ...prev, timeSlot: null }));
    if (slot !== null) {
      setFormData(prev => ({ ...prev, timeSlot: slot }));
    }
  };

  const handleNextStep = () => setActiveStep(prev => prev + 1);
  const handleBackStep = () => setActiveStep(prev => prev - 1);

  const handleSubmit = async () => {
    setLoading(true);
    setError(null);
    try {
      const command = {
        patientId: Number(patientId),
        doctorId: formData.doctorId,
        serviceId: formData.serviceId,
        dateTime: formData.timeSlot, // Это уже строка в UTC
      };
      await appointmentService.create(command);
      setIsSuccess(true);
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  // --- Рендер ---
  if (isSuccess) {
    return (
      <Paper sx={{ p: 4, maxWidth: '600px', mx: 'auto', textAlign: 'center' }}>
        <CheckCircleOutlineIcon color="success" sx={{ fontSize: 60, mb: 2 }} />
        <Typography variant="h5" gutterBottom>Appointment Created!</Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          Check the patient's email for further details and confirmation steps.
        </Typography>
        <Button variant="contained" onClick={() => navigate('/appointments')}>
          Back to Appointments List
        </Button>
      </Paper>
    );
  }
  
  return (
    <Paper sx={{ p: 3, maxWidth: '600px', mx: 'auto' }}>
      <Typography variant="h4" gutterBottom align="center">Create New Appointment</Typography>
      <Stepper activeStep={activeStep} sx={{ my: 3 }} alternativeLabel>
        {steps.map(label => <Step key={label}><StepLabel>{label}</StepLabel></Step>)}
      </Stepper>
      {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}

      {activeStep === 0 && (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <Autocomplete options={officeOptions} getOptionLabel={o => o.address || ''} value={officeOptions.find(o => o.id === formData.officeId) || null} onChange={(e, val) => handleAutocompleteChange('officeId', val)} onInputChange={(e, val) => setOfficeInput(val)} renderInput={(params) => <TextField {...params} label="1. Select Office" error={!!error?.officeId} helperText={error?.officeId} />} />
          <Autocomplete options={serviceOptions} getOptionLabel={s => s.name || ''} value={selectedService || null} onChange={(e, val) => handleAutocompleteChange('serviceId', val)} onInputChange={(e, val) => setServiceInput(val)} renderInput={(params) => <TextField {...params} label="2. Select Service" error={!!error?.serviceId} helperText={error?.serviceId} />} />
          <Autocomplete options={doctorOptions} getOptionLabel={d => d.lastName ? `${d.lastName} ${d.firstName}` : ''} value={doctorOptions.find(d => d.id === formData.doctorId) || null} onChange={handleDoctorChange} disabled={!formData.officeId || !formData.serviceId} renderInput={(params) => <TextField {...params} label="3. Select Doctor" error={!!error?.doctorId} helperText={error?.doctorId} />} />
          <DatePicker label="4. Select Date" value={formData.date} onChange={handleDateChange} shouldDisableDate={isWeekend} renderInput={(params) => <TextField {...params} fullWidth error={!!error?.date} helperText={error?.date} />} disabled={!formData.doctorId} />
          <Box sx={{ width: '100%', display: 'flex', justifyContent: 'flex-end', mt: 2 }}>
            <Button variant="contained" onClick={handleNextStep} disabled={!formData.officeId || !formData.serviceId || !formData.doctorId || !formData.date}>Next</Button>
          </Box>
        </Box>
      )}

      {activeStep === 1 && (
        <Box>
          <Typography variant="h6" gutterBottom>Available Slots for {formData.date ? format(formData.date, 'dd MMMM yyyy') : ''}</Typography>
          {slotsLoading ? <CircularProgress /> : availableSlots.length > 0 ? (
            <ToggleButtonGroup value={formData.timeSlot} exclusive onChange={handleSlotChange} orientation="vertical" fullWidth>
              {availableSlots.map(slot => (
                <ToggleButton key={slot} value={slot}>
                  {format(parseISO(slot), 'HH:mm')}
                </ToggleButton>
              ))}
            </ToggleButtonGroup>
          ) : (
            <Typography>No available slots for this day. Please select another date.</Typography>
          )}
          {error?.timeSlot && <Alert severity="error" sx={{mt: 2}}>{error.timeSlot}</Alert>}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 3 }}>
            <Button onClick={handleBackStep}>Back</Button>
            <Button variant="contained" onClick={handleNextStep} disabled={!formData.timeSlot}>Next</Button>
          </Box>
        </Box>
      )}

      {activeStep === 2 && (
        <Box>
          <Typography variant="h6" gutterBottom>Confirm your Appointment</Typography>
          <Typography><strong>Patient:</strong> {patientId}</Typography>
          <Typography><strong>Doctor:</strong> {doctorOptions.find(d => d.id === formData.doctorId)?.lastName}</Typography>
          <Typography><strong>Service:</strong> {selectedService?.name}</Typography>
          <Typography><strong>Office:</strong> {officeOptions.find(o => o.id === formData.officeId)?.address}</Typography>
          <Typography><strong>Time:</strong> {formData.timeSlot ? format(parseISO(formData.timeSlot), 'dd.MM.yyyy HH:mm') : 'N/A'}</Typography>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 3 }}>
            <Button onClick={handleBackStep} disabled={loading}>Back</Button>
            <SubmitButton onClick={handleSubmit} isLoading={loading}>Confirm Appointment</SubmitButton>
          </Box>
        </Box>
      )}
    </Paper>
  );
};

export default AppointmentForm;