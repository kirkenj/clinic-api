import React, { useState, useEffect, useMemo } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box, Paper, Typography, Alert, CircularProgress, Button,
  Autocomplete, TextField, Step, Stepper, StepLabel,
  ToggleButtonGroup, ToggleButton, IconButton
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { format, parseISO } from 'date-fns';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

import officeService from '../../services/officeService';
import serviceService from '../../services/serviceService';
import doctorService from '../../services/doctorService';
import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import useDebounce from '../../hooks/useDebounce';
import SubmitButton from '../../components/common/SubmitButton';
import { useAuth } from '../../hooks/useAuth';

const steps = ['Select Criteria', 'Choose Time Slot', 'Confirm & Book'];
const FORM_DATA_KEY = 'newAppointmentFormData';
const STEP_KEY = 'newAppointmentActiveStep';

const NewAppointment = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated } = useAuth();

  // --- Состояния, инициализируемые из sessionStorage ---
  const [formData, setFormData] = useState(() => {
    try {
      const savedData = sessionStorage.getItem(FORM_DATA_KEY);
      if (savedData) {
        const parsedData = JSON.parse(savedData);
        // Важно: `date` хранится как строка, нужно преобразовать обратно в объект Date
        if (parsedData.date) {
          parsedData.date = new Date(parsedData.date);
        }
        return parsedData;
      }
    } catch (error) {
      console.error("Failed to parse form data from session storage", error);
    }
    return {
      officeId: null,
      serviceId: null,
      doctorId: null,
      date: null,
      timeSlot: null,
    };
  });

  const [activeStep, setActiveStep] = useState(() => {
    const savedStep = sessionStorage.getItem(STEP_KEY);
    return savedStep ? parseInt(savedStep, 10) : 0;
  });

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
  
  const selectedService = useMemo(() => serviceOptions.find(s => s.id === formData.serviceId), [formData.serviceId, serviceOptions]);
  const selectedOffice = useMemo(() => officeOptions.find(o => o.id === formData.officeId), [formData.officeId, officeOptions]);
  const selectedDoctor = useMemo(() => doctorOptions.find(d => d.id === formData.doctorId), [formData.doctorId, doctorOptions]);

  // --- Сохранение в sessionStorage ---
  useEffect(() => {
    try {
      sessionStorage.setItem(FORM_DATA_KEY, JSON.stringify(formData));
    } catch (error) {
      console.error("Failed to save form data to session storage", error);
    }
  }, [formData]);

  useEffect(() => {
    sessionStorage.setItem(STEP_KEY, activeStep.toString());
  }, [activeStep]);

  // --- Загрузка данных для Autocomplete ---
  useEffect(() => {
    officeService.getPaged({ Address: debouncedOfficeInput, page: 1, pageSize: 20 })
      .then(res => setOfficeOptions(res.items || []));
  }, [debouncedOfficeInput]);

  useEffect(() => {
    serviceService.getPaged({ Name: debouncedServiceInput, page: 1, pageSize: 20 })
      .then(res => setServiceOptions(res.items || []));
  }, [debouncedServiceInput]);

  useEffect(() => {
    if (formData.officeId && selectedService?.specializationId) {
      const params = { OfficeIds: [formData.officeId], SpecializationIds: [selectedService.specializationId], page: 1, pageSize: 50 };
      doctorService.getPaged(params).then(res => setDoctorOptions(res.items || []));
    } else {
      setDoctorOptions([]);
    }
  }, [formData.officeId, selectedService]);

  useEffect(() => {
    if (formData.doctorId && formData.date) {
      setSlotsLoading(true);
      setAvailableSlots([]);
      console.log('Fetching available slots for:', formData.date, formData.doctorId);
      appointmentService.getAvailableSlots(formData.date, formData.doctorId)
        .then(slots => setAvailableSlots(slots || []))
        .catch(err => setError(handleApiError(err)))
        .finally(() => setSlotsLoading(false));
    } else {
      setAvailableSlots([]);
    }
  }, [formData.doctorId, formData.date]);

  // --- Хендлеры ---
  const handleAutocompleteChange = (name, value) => {
    setError(null);
    setFormData(prev => ({ ...prev, [name]: value ? value.id : null, doctorId: null, date: null, timeSlot: null }));
  };
  const handleDoctorChange = (event, value) => {
    setError(null);
    setFormData(prev => ({ ...prev, doctorId: value ? value.id : null, date: null, timeSlot: null }));
  };
  const handleDateChange = (date) => {
    setError(null);
    date.setUTCHours(0)
    setFormData(prev => ({ ...prev, date, timeSlot: null }));
  };
  const handleSlotChange = (event, slot) => {
    setError(null);
    if (slot !== null) setFormData(prev => ({ ...prev, timeSlot: slot }));
  };
  const handleNextStep = () => setActiveStep(prev => prev + 1);
  const handleBackStep = () => setActiveStep(prev => prev - 1);

  const handleSubmit = async () => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: location }, replace: true });
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const command = {
        doctorId: formData.doctorId,
        serviceId: formData.serviceId,
        dateTime: formData.timeSlot,
      };
      await appointmentService.createAsPatient(command);
      setIsSuccess(true);
      sessionStorage.removeItem(FORM_DATA_KEY);
      sessionStorage.removeItem(STEP_KEY);
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };
  
  if (isSuccess) {
    return (
      <Paper sx={{ p: 4, maxWidth: '600px', mx: 'auto', textAlign: 'center' }}>
        <CheckCircleOutlineIcon color="success" sx={{ fontSize: 60, mb: 2 }} />
        <Typography variant="h5" gutterBottom>Appointment Request Sent!</Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          Please check your email to confirm your appointment.
        </Typography>
        <Button variant="contained" onClick={() => navigate('/my-appointments')}>
          View My Appointments
        </Button>
      </Paper>
    );
  }

  return (
    <Paper sx={{ p: 3, maxWidth: '800px', mx: 'auto' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
        {isAuthenticated && (
          <IconButton onClick={() => navigate('/my-appointments')} aria-label="go to my appointments">
            <ArrowBackIcon />
          </IconButton>
        )}
        <Typography variant="h4" component="h1">
          Book an Appointment
        </Typography>
      </Box>
      <Stepper activeStep={activeStep} sx={{ my: 3 }} alternativeLabel>
        {steps.map(label => <Step key={label}><StepLabel>{label}</StepLabel></Step>)}
      </Stepper>
      {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}

      {activeStep === 0 && (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <Autocomplete options={officeOptions} getOptionLabel={o => o.address || ''} value={selectedOffice || null} onChange={(e, val) => handleAutocompleteChange('officeId', val)} onInputChange={(e, val) => setOfficeInput(val)} renderInput={(params) => <TextField {...params} label="1. Select Office" />} />
          <Autocomplete options={serviceOptions} getOptionLabel={s => s.name || ''} value={selectedService || null} onChange={(e, val) => handleAutocompleteChange('serviceId', val)} onInputChange={(e, val) => setServiceInput(val)} renderOption={(props, option) => (<Box component="li" {...props}><Box sx={{ flexGrow: 1 }}>{option.name}</Box><Typography variant="body2" color="text.secondary">${option.price?.toFixed(2)}</Typography></Box>)} renderInput={(params) => <TextField {...params} label="2. Select Service" />} />
          <Autocomplete options={doctorOptions} getOptionLabel={d => `${d.lastName} ${d.firstName}` || ''} value={selectedDoctor || null} onChange={handleDoctorChange} disabled={!formData.officeId || !formData.serviceId} renderInput={(params) => <TextField {...params} label="3. Select Doctor" />} />
          <DatePicker label="4. Select Date" value={formData.date} onChange={handleDateChange} renderInput={(params) => <TextField {...params} fullWidth />} disabled={!formData.doctorId} />
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
              {availableSlots.map(slot => (<ToggleButton key={slot} value={slot}>{format(parseISO(slot), 'HH:mm')}</ToggleButton>))}
            </ToggleButtonGroup>
          ) : (<Typography>No available slots for this day. Please select another date.</Typography>)}
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 3 }}>
            <Button onClick={handleBackStep}>Back</Button>
            <Button variant="contained" onClick={handleNextStep} disabled={!formData.timeSlot}>Next</Button>
          </Box>
        </Box>
      )}

      {activeStep === 2 && (
        <Box>
          <Typography variant="h6" gutterBottom>Confirm your Appointment</Typography>
          <Typography><strong>Doctor:</strong> {selectedDoctor?.lastName} {selectedDoctor?.firstName}</Typography>
          <Typography><strong>Service:</strong> {selectedService?.name} {selectedService?.price && `($${selectedService.price.toFixed(2)})`}</Typography>
          <Typography><strong>Office:</strong> {selectedOffice?.address}</Typography>
          <Typography><strong>Time:</strong> {formData.timeSlot ? format(parseISO(formData.timeSlot), 'dd.MM.yyyy HH:mm') : 'N/A'}</Typography>
          <Box sx={{ mt: 3, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Button onClick={handleBackStep} disabled={loading}>Back</Button>
            <SubmitButton onClick={handleSubmit} isLoading={loading}>
              {isAuthenticated ? 'Confirm & Book' : 'Sign In to Book'}
            </SubmitButton>
          </Box>
        </Box>
      )}
    </Paper>
  );
};

export default NewAppointment;