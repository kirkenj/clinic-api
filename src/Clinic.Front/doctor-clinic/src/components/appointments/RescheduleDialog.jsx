// src/components/appointments/RescheduleDialog.jsx
import React, { useState, useEffect, useMemo } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, Typography, Alert, CircularProgress,
  Autocomplete, TextField, ToggleButtonGroup, ToggleButton
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { format, parseISO } from 'date-fns';
import CloseIcon from '@mui/icons-material/Close';
import IconButton from '@mui/material/IconButton';

import doctorService from '../../services/doctorService';
import appointmentService from '../../services/appointmentService';
import serviceService from '../../services/serviceService'; // 1. Импортируем сервис для услуг
import { handleApiError } from '../../utils/handleApiError';
import SubmitButton from '../common/SubmitButton';

const RescheduleDialog = ({ open, onClose, appointment, onSuccess }) => {
  // --- Состояния ---
  const [newDoctorId, setNewDoctorId] = useState(null);
  const [newDate, setNewDate] = useState(null);
  const [newTimeSlot, setNewTimeSlot] = useState(null);
  
  const [doctorOptions, setDoctorOptions] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);
  
  const [loading, setLoading] = useState(false);
  const [slotsLoading, setSlotsLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(false);
  const [error, setError] = useState(null);
  
  // --- Логика загрузки ---

  // Эффект, который запускает всю цепочку загрузки при открытии диалога
  useEffect(() => {
    // Сбрасываем все при закрытии
    if (!open) {
      setNewDoctorId(null);
      setNewDate(null);
      setNewTimeSlot(null);
      setDoctorOptions([]);
      setAvailableSlots([]);
      return;
    }

    if (appointment?.id) {
      const loadInitialData = async () => {
        try {
          setInitialLoading(true);
          setError(null);

          // 2. Шаг 1: Получаем запись, чтобы узнать serviceId
          const appointmentData = await appointmentService.getById(appointment.id);
          
          if (!appointmentData?.serviceId) {
            throw new Error("Service information is missing in the appointment.");
          }

          // 3. Шаг 2: Получаем услугу, чтобы узнать specializationId
          const serviceData = await serviceService.getById(appointmentData.serviceId);

          if (!serviceData?.specializationId) {
            throw new Error("Specialization information is missing in the service.");
          }

          // 4. Шаг 3: Получаем список докторов нужной специализации
          const params = {
            SpecializationIds: [serviceData.specializationId],
            page: 1,
            pageSize: 100,
          };
          const doctorsResponse = await doctorService.getPaged(params);
          setDoctorOptions(doctorsResponse.items || []);
          
          // По умолчанию выбираем текущего доктора
          setNewDoctorId(appointmentData.doctorId);

        } catch (err) {
          setError(handleApiError(err));
        } finally {
          setInitialLoading(false);
        }
      };
      
      loadInitialData();
    }
  }, [open, appointment]);

  // Загрузка слотов (без изменений)
  useEffect(() => {
    if (newDoctorId && newDate) {
      setSlotsLoading(true);
      setAvailableSlots([]);
      const isoDate = new Date(newDate).toISOString();
      appointmentService.getAvailableSlots(isoDate, newDoctorId)
        .then(slots => setAvailableSlots(slots || []))
        .catch(err => setError(handleApiError(err)))
        .finally(() => setSlotsLoading(false));
    }
  }, [newDoctorId, newDate]);


  const handleSubmit = async () => {
    if (!appointment || !newTimeSlot || !newDoctorId) return;
    setLoading(true);
    setError(null);
    try {
      const command = {
        appointmentId: appointment.id,
        newDoctorId,
        newDateTime: newTimeSlot,
      };
      await appointmentService.reschedule(command);
      onSuccess();
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };
  
  const selectedDoctorObject = useMemo(() => 
    doctorOptions.find(d => d.id === newDoctorId), 
  [newDoctorId, doctorOptions]);

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>
        Reschedule Appointment
        <IconButton aria-label="close" onClick={onClose} sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent dividers>
        {initialLoading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>
        ) : error && error.general ? (
          <Alert severity="error">{error.general}</Alert>
        ) : (
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, mt: 1 }}>
            <Autocomplete
              options={doctorOptions}
              getOptionLabel={(d) => d.lastName ? `${d.lastName} ${d.firstName}` : ''}
              value={selectedDoctorObject || null}
              onChange={(e, value) => {
                setNewDoctorId(value ? value.id : null);
                setNewDate(null);
                setNewTimeSlot(null);
              }}
              renderInput={(params) => <TextField {...params} label="1. Select New Doctor (or keep current)" />}
            />
            <DatePicker
              label="2. Select New Date"
              value={newDate}
              onChange={(date) => {
                date.setUTCDate(date.getUTCDate() + 1); 
                date.setUTCHours(0); 
                setNewDate(date);
                setNewTimeSlot(null);
              }}
              renderInput={(params) => <TextField {...params} fullWidth />}
              disabled={!newDoctorId}
            />
            <Box>
              <Typography variant="subtitle2" gutterBottom>3. Select New Time Slot</Typography>
              {slotsLoading ? <CircularProgress size={24} /> : availableSlots.length > 0 ? (
                <ToggleButtonGroup value={newTimeSlot} exclusive onChange={(e, val) => setNewTimeSlot(val)} orientation="vertical" fullWidth>
                  {availableSlots.map(slot => (
                    <ToggleButton key={slot} value={slot}>{format(parseISO(slot), 'HH:mm')}</ToggleButton>
                  ))}
                </ToggleButtonGroup>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  {newDoctorId && newDate ? 'No available slots for this day.' : 'Select a doctor and a date to see available slots.'}
                </Typography>
              )}
            </Box>
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={loading}>Cancel</Button>
        <SubmitButton onClick={handleSubmit} isLoading={loading} disabled={!newTimeSlot}>
          Confirm Reschedule
        </SubmitButton>
      </DialogActions>
    </Dialog>
  );
};

export default RescheduleDialog;