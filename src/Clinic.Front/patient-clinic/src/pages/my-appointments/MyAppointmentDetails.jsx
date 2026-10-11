// src/pages/my-appointments/MyAppointmentDetails.jsx
import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, CircularProgress, Alert, Grid, Chip, Button, Divider, IconButton } from '@mui/material';
import { format, parseISO } from 'date-fns';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import HowToRegIcon from '@mui/icons-material/HowToReg';
import CancelIcon from '@mui/icons-material/Cancel';

import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_FROM_STRING, APPOINTMENT_STATUS_TEXT, APPOINTMENT_STATUS_COLORS } from '../../constants/appointmentConstants';
import appointmentService from '../../services/appointmentService';
import appointmentResultService from '../../services/appointmentResultService';
import doctorService from '../../services/doctorService';
import serviceService from '../../services/serviceService';
import specializationService from '../../services/specializationService';
import { handleApiError } from '../../utils/handleApiError';

import ApproveByPatientDialog from '../../components/appointments/ApproveByPatientDialog';
import CancelByPatientDialog from '../../components/appointments/CancelByPatientDialog';

const MyAppointmentDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [appointment, setAppointment] = useState(null);
  const [doctor, setDoctor] = useState(null);
  const [service, setService] = useState(null);
  const [specialization, setSpecialization] = useState(null);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [openApproveDialog, setOpenApproveDialog] = useState(false);
  const [openCancelDialog, setOpenCancelDialog] = useState(false);

  const fetchData = useCallback(async () => {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      // Используем метод для получения СВОЕЙ записи
      const rawAppointmentData = await appointmentService.getMyById(id);
      
      if (rawAppointmentData) {
        const numericStatus = APPOINTMENT_STATUS_FROM_STRING[rawAppointmentData.status] ?? -1;
        const appointmentData = { ...rawAppointmentData, status: numericStatus };
        setAppointment(appointmentData);

        const [doctorData, serviceData, resultData] = await Promise.all([
          doctorService.getById(appointmentData.doctorId),
          serviceService.getById(appointmentData.serviceId),
          appointmentResultService.getByAppointmentId(id).catch(() => null)
        ]);
        
        setDoctor(doctorData);
        setService(serviceData);
        setResult(resultData);

        if (doctorData?.specializationId) {
          const specializationData = await specializationService.getById(doctorData.specializationId);
          setSpecialization(specializationData);
        }
      }
    } catch (err) {
      // Если пациент пытается получить доступ к чужой записи, API вернет 403/404
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { fetchData(); }, [fetchData]);
  
  const handleActionSuccess = () => {
    setOpenCancelDialog(false);
    setOpenApproveDialog(false);
    fetchData();
  };


  const isNotOutdated = (appt) => new Date(appt?.dateTime) > Date.now();
  
  const isCancelable = (appt) => [
      APPOINTMENT_STATUSES.WaitingClinicConfirm,
      APPOINTMENT_STATUSES.WaitingUserConfirm,
      APPOINTMENT_STATUSES.Confirmed].includes(appt.status) && isNotOutdated(appt);
  const canBeApproved = (appt) => appt.status === APPOINTMENT_STATUSES.WaitingUserConfirm && isNotOutdated(appt);

  if (loading) return <CircularProgress />;
  if (error) return <Alert severity="error">{error}</Alert>;
  if (!appointment) return <Typography>Appointment not found.</Typography>;

  return (
    <>
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
          <IconButton onClick={() => navigate(-1)} aria-label="go back"><ArrowBackIcon /></IconButton>
          <Typography variant="h4">Appointment Details</Typography>
        </Box>
        <Chip label={APPOINTMENT_STATUS_TEXT[appointment.status] || 'Unknown'} color={APPOINTMENT_STATUS_COLORS[appointment.status] || 'default'} />
        <Divider sx={{ my: 2 }} />
        
        <Grid container spacing={2}>
          <Grid item xs={12}><Typography variant="overline">Doctor</Typography><Typography variant="h6">{doctor ? `${doctor.firstName} ${doctor.lastName}` : '...'}</Typography><Typography color="text.secondary">{specialization?.name || '...'}</Typography></Grid>
          <Grid item xs={12}><Typography variant="overline">Service</Typography><Typography variant="body1">{service?.name || '...'}</Typography></Grid>
          <Grid item xs={12}><Typography variant="overline">Date & Time</Typography><Typography variant="body1">{format(parseISO(appointment.dateTime), 'EEEE, d MMMM yyyy, HH:mm')}</Typography></Grid>
        </Grid>
        
        <Box sx={{ mt: 3, display: 'flex', gap: 1 }}>
          {canBeApproved(appointment) && (<Button variant="contained" startIcon={<HowToRegIcon />} onClick={() => setOpenApproveDialog(true)}>Confirm</Button>)}
          {isCancelable(appointment) && (<Button variant="outlined" color="error" startIcon={<CancelIcon />} onClick={() => setOpenCancelDialog(true)}>Cancel</Button>)}
        </Box>
      </Paper>
      
      {/* Показываем блок с результатом, только если запись подтверждена или завершена */}
      {[APPOINTMENT_STATUSES.Confirmed, APPOINTMENT_STATUSES.Completed].includes(appointment.status) && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="h5">Appointment Result</Typography>
          <Divider sx={{ my: 2 }} />
          {result ? (
            <Box><Typography variant="subtitle2">Complaints</Typography><Typography paragraph>{result.complaints}</Typography><Typography variant="subtitle2">Conclusion</Typography><Typography paragraph>{result.conclusion}</Typography><Typography variant="subtitle2">Recommendations</Typography><Typography paragraph>{result.recommendations}</Typography></Box>
          ) : (
            <Typography color="text.secondary">The result has not been added by the doctor yet.</Typography>
          )}
        </Paper>
      )}

      <ApproveByPatientDialog open={openApproveDialog} onClose={() => setOpenApproveDialog(false)} appointment={appointment} onSuccess={handleActionSuccess}/>
      <CancelByPatientDialog open={openCancelDialog} onClose={() => setOpenCancelDialog(false)} appointment={appointment} onCancelSuccess={handleActionSuccess}/>
    </>
  );
};

export default MyAppointmentDetails;