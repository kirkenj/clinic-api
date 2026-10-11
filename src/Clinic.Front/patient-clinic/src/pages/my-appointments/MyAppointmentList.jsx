import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Paper, Typography, CircularProgress, Alert, Pagination,
  List, ListItem, ListItemText, ListItemAvatar, Avatar,
  Button, Divider, Chip, Tooltip, IconButton
} from '@mui/material';
import { format, parseISO } from 'date-fns';
import MedicalServicesIcon from '@mui/icons-material/MedicalServices';
import CancelIcon from '@mui/icons-material/Cancel';
import FilterListIcon from '@mui/icons-material/FilterList';
import HowToRegIcon from '@mui/icons-material/HowToReg';

import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_FROM_STRING, APPOINTMENT_STATUS_TEXT, APPOINTMENT_STATUS_COLORS } from '../../constants/appointmentConstants';
import appointmentService from '../../services/appointmentService';
import doctorService from '../../services/doctorService';
import serviceService from '../../services/serviceService';
import { handleApiError } from '../../utils/handleApiError';
import { cleanObject } from '../../utils/cleanObject';
import CancelByPatientDialog from '../../components/appointments/CancelByPatientDialog';
import ApproveByPatientDialog from '../../components/appointments/ApproveByPatientDialog';
import AppointmentFilterDialog from '../../components/appointments/AppointmentFilterDialog';

const MyAppointmentList = () => {
  const navigate = useNavigate();

  // --- Состояния ---
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [doctorsMap, setDoctorsMap] = useState({});
  const [servicesMap, setServicesMap] = useState({});
  
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  
  const [filters, setFilters] = useState({
    doctor: null,
    status: '',
    dateFrom: null,
    dateTo: null,
  });

  const [openCancelDialog, setOpenCancelDialog] = useState(false);
  const [openApproveDialog, setOpenApproveDialog] = useState(false);
  const [openFilterDialog, setOpenFilterDialog] = useState(false);
  const [selectedAppointment, setSelectedAppointment] = useState(null);

  const { doctor, status, dateFrom, dateTo } = filters;

  const fetchMyAppointments = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params = {
        page,
        pageSize: 10,
        DoctorIds: doctor ? [doctor.id] : null,
        Statuses: status ? [status] : null,
        DateTimeStart: dateFrom ? dateFrom.toISOString() : null,
        DateTimeEnd: dateTo ? new Date(dateTo.setHours(23, 59, 59, 999)).toISOString() : null,
      };
      
      const cleanedParams = cleanObject(params);
      const response = await appointmentService.getMyPaged(cleanedParams);
      
      const appointmentsData = (response.items || []).map(apt => ({
        ...apt,
        status: APPOINTMENT_STATUS_FROM_STRING[apt.status] ?? -1,
      }));
      setAppointments(appointmentsData);
      setTotalCount(response.totalCount || 0);

      if (appointmentsData.length > 0) {
        const doctorIds = [...new Set(appointmentsData.map(a => a.doctorId).filter(Boolean))];
        const serviceIds = [...new Set(appointmentsData.map(a => a.serviceId).filter(Boolean))];
        
        const [doctorsResponse, servicesResponse] = await Promise.all([
          doctorIds.length > 0 ? doctorService.getPaged({ Ids: doctorIds, page: 1, pageSize: doctorIds.length }) : Promise.resolve({ items: [] }),
          serviceIds.length > 0 ? serviceService.getPaged({ Ids: serviceIds, page: 1, pageSize: serviceIds.length }) : Promise.resolve({ items: [] })
        ]);

        const createMap = (items) => (items || []).reduce((acc, item) => ({ ...acc, [item.id]: item }), {});
        setDoctorsMap(prev => ({ ...prev, ...createMap(doctorsResponse.items) }));
        setServicesMap(prev => ({ ...prev, ...createMap(servicesResponse.items) }));
      }
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, doctor, status, dateFrom, dateTo]);

  useEffect(() => { setPage(1); }, [doctor, status, dateFrom, dateTo]);
  useEffect(() => { fetchMyAppointments(); }, [fetchMyAppointments]);

  const handlePageChange = (event, value) => setPage(value);
  const handleApplyFilters = (newFilters) => setFilters(newFilters);
  
  const handleOpenDialog = (setter, appointment) => {
    setSelectedAppointment(appointment);
    setter(true);
  };
  
  const handleActionSuccess = () => {
    setSelectedAppointment(null);
    setOpenCancelDialog(false);
    setOpenApproveDialog(false);
    fetchMyAppointments();
  };

  const isNotOutdated = (appt) => new Date(appt?.dateTime) > Date.now();

  const isApprovable = (item) => [
    APPOINTMENT_STATUSES.WaitingClinicConfirm]
    .includes(item.status) && isNotOutdated(item);
  
  const isCancelable = (item) => [
    APPOINTMENT_STATUSES.WaitingClinicConfirm,
    APPOINTMENT_STATUSES.WaitingUserConfirm,
    APPOINTMENT_STATUSES.Confirmed
  ].includes(item.status) && isNotOutdated(item);

  const pageCount = Math.ceil(totalCount / 10);

  return (
    <>
      <Paper sx={{ p: 2, boxShadow: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h4">My Appointments</Typography>
          <Button variant="outlined" startIcon={<FilterListIcon />} onClick={() => setOpenFilterDialog(true)}>
            Filters
          </Button>
        </Box>

        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: Object.values(filters).some(v => v) ? 2 : 0 }}>
          {filters.doctor && <Chip label={`Doctor: ${filters.doctor.lastName}`} onDelete={() => setFilters(p => ({...p, doctor: null}))} />}
          {filters.status && <Chip label={`Status: ${APPOINTMENT_STATUS_TEXT[filters.status]}`} onDelete={() => setFilters(p => ({...p, status: ''}))} />}
          {filters.dateFrom && <Chip label={`From: ${format(filters.dateFrom, 'dd/MM/yy')}`} onDelete={() => setFilters(p => ({...p, dateFrom: null}))} />}
          {filters.dateTo && <Chip label={`To: ${format(filters.dateTo, 'dd/MM/yy')}`} onDelete={() => setFilters(p => ({...p, dateTo: null}))} />}
        </Box>
        
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}><CircularProgress /></Box>
        ) : appointments.length > 0 ? (
          <List sx={{ width: '100%' }}>
            {appointments.map((item, index) => {
              const appointmentDoctor = doctorsMap[item.doctorId];
              const appointmentServiceItem = servicesMap[item.serviceId];
              return (
                <React.Fragment key={item.id}>
                  <ListItem
                    onClick={() => navigate(`/my-appointments/${item.id}`)}
                    alignItems="flex-start"
                    secondaryAction={
                      <Box>
                        {isApprovable(item) && (
                          <Tooltip title="Confirm Appointment">
                            <IconButton edge="end" aria-label="confirm" color="warning" onClick={() => handleOpenDialog(setOpenApproveDialog, item)}>
                              <HowToRegIcon />
                            </IconButton>
                          </Tooltip>
                        )}
                        {isCancelable(item) && (
                          <Tooltip title="Cancel Appointment">
                            <IconButton edge="end" aria-label="cancel" color="error" onClick={() => handleOpenDialog(setOpenCancelDialog, item)}>
                              <CancelIcon />
                            </IconButton>
                          </Tooltip>
                        )}
                      </Box>
                    }
                  >
                    <ListItemAvatar>
                      <Avatar sx={{ bgcolor: 'primary.main' }}><MedicalServicesIcon /></Avatar>
                    </ListItemAvatar>
                    <ListItemText
                      primary={appointmentServiceItem?.name || `Service ID: ${item.serviceId}`}
                      secondary={
                        <>
                          <Typography component="span" variant="body2" display="block" color="text.primary">
                            Dr. {appointmentDoctor ? `${appointmentDoctor.firstName} ${appointmentDoctor.lastName}` : '...'}
                          </Typography>
                          <Typography component="span" variant="body2" display="block" color="text.secondary">
                            {item.dateTime ? format(parseISO(item.dateTime), 'EEEE, d MMMM yyyy, HH:mm') : 'N/A'}
                          </Typography>
                          <Chip 
                            label={APPOINTMENT_STATUS_TEXT[item.status] || 'Unknown'} 
                            color={APPOINTMENT_STATUS_COLORS[item.status] || 'default'}
                            size="small" 
                            sx={{ mt: 1 }}
                          />
                        </>
                      }
                    />
                  </ListItem>
                  {index < appointments.length - 1 && <Divider variant="inset" component="li" />}
                </React.Fragment>
              );
            })}
          </List>
        ) : (
          <Typography sx={{ textAlign: 'center', p: 4, color: 'text.secondary' }}>
            You have no appointments for the selected filters.
          </Typography>
        )}
        
        {pageCount > 1 && (
          <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
            <Pagination count={pageCount} page={page} onChange={handlePageChange} color="primary" />
          </Box>
        )}
      </Paper>
      
      <AppointmentFilterDialog
        open={openFilterDialog}
        onClose={() => setOpenFilterDialog(false)}
        initialFilters={filters}
        onApply={handleApplyFilters}
      />
      
      {selectedAppointment && (
        <>
          {<CancelByPatientDialog
            open={openCancelDialog}
            onClose={() => setOpenCancelDialog(false)}
            appointment={selectedAppointment}
            onCancelSuccess={handleActionSuccess}
          />}
          <ApproveByPatientDialog
            open={openApproveDialog}
            onClose={() => handleActionSuccess(false)}
            appointment={selectedAppointment}
            onSuccess={() => handleActionSuccess(true)}
          />
        </>
      )}
    </>
  );
};

export default MyAppointmentList;