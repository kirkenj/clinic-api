import React, { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { Box, Paper, Typography, CircularProgress, Alert, Grid, Chip, Button, Divider, IconButton, Tooltip } from '@mui/material';
import { format, parseISO } from 'date-fns';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import HowToRegIcon from '@mui/icons-material/HowToReg';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import PersonOffIcon from '@mui/icons-material/PersonOff';
import AutorenewIcon from '@mui/icons-material/Autorenew';
import PostAddIcon from '@mui/icons-material/PostAdd';
import EditIcon from '@mui/icons-material/Edit';
import PersonSearchIcon from '@mui/icons-material/PersonSearch';

import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_FROM_STRING, APPOINTMENT_STATUS_TEXT, APPOINTMENT_STATUS_COLORS } from '../../constants/appointmentConstants';
import appointmentService from '../../services/appointmentService';
import appointmentResultService from '../../services/appointmentResultService';
import doctorService from '../../services/doctorService';
import patientService from '../../services/patientService';
import serviceService from '../../services/serviceService';
import specializationService from '../../services/specializationService';
import { handleApiError } from '../../utils/handleApiError';

import ApproveByPatientDialog from '../../components/appointments/ApproveByPatientDialog';
import CancelByClinicDialog from '../../components/appointments/CancelByClinicDialog';
import CancelByPatientDialog from '../../components/appointments/CancelByPatientDialog';
import RescheduleDialog from '../../components/appointments/RescheduleDialog';
import AppointmentResultForm from '../../components/appointments/AppointmentResultForm';

const AppointmentDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [appointment, setAppointment] = useState(null);
  const [patient, setPatient] = useState(null);
  const [doctor, setDoctor] = useState(null);
  const [service, setService] = useState(null);
  const [specialization, setSpecialization] = useState(null);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [openApproveDialog, setOpenApproveDialog] = useState(false);
  const [openClinicCancel, setOpenClinicCancel] = useState(false);
  const [openPatientCancel, setOpenPatientCancel] = useState(false);
  const [openReschedule, setOpenReschedule] = useState(false);
  const [isEditingResult, setIsEditingResult] = useState(false);

  const fetchData = useCallback(async () => {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      const rawAppointmentData = await appointmentService.getById(id);
      
      if (rawAppointmentData) {
        const numericStatus = APPOINTMENT_STATUS_FROM_STRING[rawAppointmentData.status] ?? -1;
        const appointmentData = { ...rawAppointmentData, status: numericStatus };
        setAppointment(appointmentData);

        const [patientData, doctorData, serviceData, resultData] = await Promise.all([
          patientService.getById(appointmentData.patientId),
          doctorService.getById(appointmentData.doctorId),
          serviceService.getById(appointmentData.serviceId),
          appointmentResultService.getByAppointmentId(id).catch(() => null)
        ]);
        
        setPatient(patientData);
        setDoctor(doctorData);
        setService(serviceData);
        setResult(resultData);

        if (doctorData?.specializationId) {
          const specializationData = await specializationService.getById(doctorData.specializationId);
          setSpecialization(specializationData);
        }
      }
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => { fetchData(); }, [fetchData]);
  
  const handleActionSuccess = () => {
    setOpenClinicCancel(false);
    setOpenPatientCancel(false);
    setOpenReschedule(false);
    setOpenApproveDialog(false);
    setIsEditingResult(false);
    fetchData();
  };

  const handleApproveByClinic = async () => {
    if (!appointment) return;
    try {
      await appointmentService.approveByClinic({ appointmentId: appointment.id });
      fetchData();
    } catch (err) {
      setError(handleApiError(err).general);
    }
  };

  const isNotOutdated = (appt) => new Date(appt?.dateTime) > Date.now();
  const canManageResult = (status) => [APPOINTMENT_STATUSES.Confirmed].includes(status) && !isNotOutdated(appointment);
  const isAppointmentCancelable = (item) => [APPOINTMENT_STATUSES.WaitingClinicConfirm, APPOINTMENT_STATUSES.WaitingUserConfirm, APPOINTMENT_STATUSES.Confirmed].includes(item.status) && isNotOutdated(item);
  const isAppointmentReschedulable = (item) => [APPOINTMENT_STATUSES.WaitingClinicConfirm, APPOINTMENT_STATUSES.Confirmed].includes(item.status) && item.dateTime && isNotOutdated(item);
  const isAppointmentApprovableByPatient = (item) => [APPOINTMENT_STATUSES.WaitingUserConfirm].includes(item.status) && isNotOutdated(item);
  const isAppointmentApprovableByClinic = (item) => [APPOINTMENT_STATUSES.WaitingClinicConfirm].includes(item.status) && isNotOutdated(item);
  


  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>;
  if (error) return <Alert severity="error">{error}</Alert>;
  if (!appointment) return <Typography sx={{ p: 4, textAlign: 'center' }}>Appointment not found.</Typography>;

  return (
    <>
      <Paper sx={{ p: { xs: 2, sm: 3 }, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 2 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <IconButton onClick={() => navigate(-1)} aria-label="go back"><ArrowBackIcon /></IconButton>
            <Typography variant="h4">Appointment Details</Typography>
          </Box>
          <Chip label={APPOINTMENT_STATUS_TEXT[appointment.status] || 'Unknown'} color={APPOINTMENT_STATUS_COLORS[appointment.status] || 'default'} />
        </Box>
        <Divider sx={{ my: 2 }} />
        
        <Grid container spacing={3}>
          <Grid item xs={12} sm={6}>
            <Typography variant="overline" color="text.secondary">Patient</Typography>
            {patient ? (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Typography variant="h6" component={Link} to={`/patients/edit/${patient.id}`} color="primary.main" sx={{ textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}>
                  {patient.firstName} {patient.lastName}
                </Typography>
                <Tooltip title="View all appointments for this patient"><IconButton size="small" onClick={(e) => { e.stopPropagation(); navigate('/appointments', { state: { patient } })}}><PersonSearchIcon /></IconButton></Tooltip>
              </Box>
            ) : <CircularProgress size={20} />}
          </Grid>
          <Grid item xs={12} sm={6}>
            <Typography variant="overline" color="text.secondary">Doctor</Typography>
            {doctor ? (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Typography variant="h6" component={Link} to={`/doctors/edit/${doctor.id}`} color="primary.main" sx={{ textDecoration: 'none', '&:hover': { textDecoration: 'underline' } }}>
                  {doctor.firstName} {doctor.lastName}
                </Typography>
                 <Tooltip title="View all appointments for this doctor"><IconButton size="small" onClick={(e) => { e.stopPropagation(); navigate('/appointments', { state: { doctor } })}}><PersonSearchIcon /></IconButton></Tooltip>
              </Box>
            ) : <CircularProgress size={20} />}
             <Typography color="text.secondary">{specialization?.name || '...'}</Typography>
          </Grid>
          <Grid item xs={12} sm={6}><Typography variant="overline" color="text.secondary">Service</Typography><Typography variant="body1">{service?.name || '...'}</Typography></Grid>
          <Grid item xs={12} sm={6}><Typography variant="overline" color="text.secondary">Date & Time</Typography><Typography variant="body1">{appointment.dateTime ? format(parseISO(appointment.dateTime), 'EEEE, d MMMM yyyy, HH:mm') : 'N/A'}</Typography></Grid>
        </Grid>
        
        <Box sx={{ mt: 3, display: 'flex', gap: 1, flexWrap: 'wrap' }}>
          {isAppointmentReschedulable(appointment) && (<Button variant="contained" startIcon={<AutorenewIcon />} onClick={() => setOpenReschedule(true)}>Reschedule</Button>)}
          {isAppointmentCancelable(appointment) && (<><Button variant="outlined" color="error" startIcon={<CancelIcon />} onClick={() => setOpenClinicCancel(true)}>Cancel (by Clinic)</Button><Button variant="outlined" color="secondary" startIcon={<PersonOffIcon />} onClick={() => setOpenPatientCancel(true)}>Cancel (as Patient)</Button></>)}
          {isAppointmentApprovableByClinic(appointment) && (<Button variant="contained" color="primary" startIcon={<CheckCircleIcon />} onClick={handleApproveByClinic}>Approve (by Clinic)</Button>)}
          {isAppointmentApprovableByPatient(appointment) && (<Button variant="contained" color="warning" startIcon={<HowToRegIcon />} onClick={() => setOpenApproveDialog(true)}>Approve (by Patient)</Button>)}
        </Box>
      </Paper>
      
      <Paper sx={{ p: { xs: 2, sm: 3 } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h5">Appointment Result</Typography>
          {!isEditingResult && canManageResult(appointment.status) && (
            <Button variant={result ? "outlined" : "contained"} startIcon={result ? <EditIcon /> : <PostAddIcon />} onClick={() => setIsEditingResult(true)}>
              {result ? 'Edit Result' : 'Add Result'}
            </Button>
          )}
        </Box>
        <Divider sx={{ my: 2 }} />
        {isEditingResult ? (
          <AppointmentResultForm appointmentId={appointment.id} existingResult={result} onSuccess={handleActionSuccess} onCancel={() => setIsEditingResult(false)}/>
        ) : ( result ? (<Box><Typography variant="subtitle2" color="text.secondary">Complaints</Typography><Typography paragraph>{result.complaints || 'Not specified.'}</Typography><Typography variant="subtitle2" color="text.secondary">Conclusion</Typography><Typography paragraph>{result.conclusion || 'Not specified.'}</Typography><Typography variant="subtitle2" color="text.secondary">Recommendations</Typography><Typography paragraph>{result.recommendations || 'Not specified.'}</Typography></Box>)
           : ( canManageResult(appointment.status) ? <Typography color="text.secondary">No result has been added for this appointment yet.</Typography> : <></> ))}
      </Paper>

      <RescheduleDialog open={openReschedule} onClose={() => setOpenReschedule(false)} appointment={appointment} onSuccess={handleActionSuccess} />
      <CancelByClinicDialog open={openClinicCancel} onClose={() => setOpenClinicCancel(false)} appointment={appointment} onSuccess={handleActionSuccess} />
      <CancelByPatientDialog open={openPatientCancel} onClose={() => setOpenPatientCancel(false)} appointment={appointment} onCancelSuccess={handleActionSuccess} />
      <ApproveByPatientDialog open={openApproveDialog} onClose={() => handleActionSuccess(false)} appointment={appointment} onSuccess={() => handleActionSuccess(true)} />
    </>
  );
};

export default AppointmentDetails;