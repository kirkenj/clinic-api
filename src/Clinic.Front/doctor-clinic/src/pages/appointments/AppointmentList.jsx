import React, { useState, useEffect, useCallback } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box, Typography, Paper, CircularProgress, Alert, Pagination, Tooltip,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel, IconButton,
  TextField, Autocomplete, Select, MenuItem, FormControl, Chip
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { parseISO, format } from 'date-fns';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import CancelIcon from '@mui/icons-material/Cancel';
import AutorenewIcon from '@mui/icons-material/Autorenew';

import { useAuth } from '../../hooks/useAuth';
import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_FROM_STRING, APPOINTMENT_STATUS_TEXT, APPOINTMENT_STATUS_COLORS } from '../../constants/appointmentConstants';
import appointmentService from '../../services/appointmentService';
import doctorService from '../../services/doctorService';
import patientService from '../../services/patientService';
import serviceService from '../../services/serviceService';
import { handleApiError } from '../../utils/handleApiError';
import { APPOINTMENT_SORT_COLUMNS } from '../../constants/sortColumns';
import { cleanObject } from '../../utils/cleanObject';
import useDebounce from '../../hooks/useDebounce';

import CancelByClinicDialog from '../../components/appointments/CancelByClinicDialog';
import RescheduleDialog from '../../components/appointments/RescheduleDialog';

const headCells = [
  { id: 'dateTimeStart', label: 'Date & Time', width: '18%', disableSorting: false },
  { id: 'patient', label: 'Patient', disableSorting: true, width: '18%' },
  { id: 'doctor', label: 'Doctor', disableSorting: true, width: '18%' },
  { id: 'service', label: 'Service', disableSorting: true, width: '15%' },
  { id: 'status', label: 'Status', disableSorting: false, width: '15%' },
  { id: 'actions', label: 'Actions', disableSorting: true, align: 'right', width: '16%' },
];

const DoctorAppointmentList = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();
  
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [doctorsMap, setDoctorsMap] = useState({});
  const [patientsMap, setPatientsMap] = useState({});
  const [servicesMap, setServicesMap] = useState({});
  
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('desc');
  const [sortColumn, setSortColumn] = useState('dateTimeStart');
  
  const [filters, setFilters] = useState({
    patient: location.state?.patient || null,
    doctor: location.state?.doctor || null,
    service: null,
    status: '',
    dateFrom: null,
    dateTo: null,
  });

  const [doctorOptions, setDoctorOptions] = useState([]);
  const [patientOptions, setPatientOptions] = useState([]);
  const [serviceOptions, setServiceOptions] = useState([]);
  
  const [doctorInput, setDoctorInput] = useState('');
  const [patientInput, setPatientInput] = useState('');
  const [serviceInput, setServiceInput] = useState('');
  const debouncedDoctorInput = useDebounce(doctorInput, 400);
  const debouncedPatientInput = useDebounce(patientInput, 400);
  const debouncedServiceInput = useDebounce(serviceInput, 400);
  
  const [openClinicCancel, setOpenClinicCancel] = useState(false);
  const [openRescheduleDialog, setOpenRescheduleDialog] = useState(false);
  const [selectedAppointment, setSelectedAppointment] = useState(null);

  const { patient, doctor, service, status, dateFrom, dateTo } = filters;
  
  useEffect(() => {
    doctorService.getPaged({ LastName: debouncedDoctorInput, page: 1, pageSize: 20 }).then(res => setDoctorOptions(res.items || []));
  }, [debouncedDoctorInput]);
  useEffect(() => {
    patientService.getPaged({ LastName: debouncedPatientInput, page: 1, pageSize: 20 }).then(res => setPatientOptions(res.items || []));
  }, [debouncedPatientInput]);
  useEffect(() => {
    serviceService.getPaged({ Name: debouncedServiceInput, page: 1, pageSize: 20 }).then(res => setServiceOptions(res.items || []));
  }, [debouncedServiceInput]);

  const fetchAppointments = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params = {
        page,
        pageSize: 10,
        SortColumn: APPOINTMENT_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        PatientIds: patient ? [patient.id] : null,
        DoctorIds: doctor ? [doctor.id] : null,
        ServiceIds: service ? [service.id] : null,
        Statuses: status ? [status] : null,
        DateTimeStart: dateFrom ? dateFrom.toISOString() : null,
        DateTimeEnd: dateTo ? new Date(dateTo.setHours(23, 59, 59, 999)).toISOString() : null,
      };
      
      const cleanedParams = cleanObject(params);
      const response = await appointmentService.getPaged(cleanedParams);
      
      const appointmentsData = (response.items || []).map(apt => ({
        ...apt,
        status: APPOINTMENT_STATUS_FROM_STRING[apt.status] ?? -1,
      }));
      setAppointments(appointmentsData);
      setTotalCount(response.totalCount || 0);

      if (appointmentsData.length > 0) {
        const doctorIds = [...new Set(appointmentsData.map(a => a.doctorId).filter(Boolean))];
        const patientIds = [...new Set(appointmentsData.map(a => a.patientId).filter(Boolean))];
        const serviceIds = [...new Set(appointmentsData.map(a => a.serviceId).filter(Boolean))];
        
        const [doctorsResponse, patientsResponse, servicesResponse] = await Promise.all([
          doctorIds.length > 0 ? doctorService.getPaged({ Ids: doctorIds, page: 1, pageSize: doctorIds.length }) : Promise.resolve({ items: [] }),
          patientIds.length > 0 ? patientService.getPaged({ Ids: patientIds, page: 1, pageSize: patientIds.length }) : Promise.resolve({ items: [] }),
          serviceIds.length > 0 ? serviceService.getPaged({ Ids: serviceIds, page: 1, pageSize: serviceIds.length }) : Promise.resolve({ items: [] })
        ]);

        const createMap = (items) => (items || []).reduce((acc, item) => ({ ...acc, [item.id]: item }), {});
        setDoctorsMap(prev => ({ ...prev, ...createMap(doctorsResponse.items) }));
        setPatientsMap(prev => ({ ...prev, ...createMap(patientsResponse.items) }));
        setServicesMap(prev => ({ ...prev, ...createMap(servicesResponse.items) }));
      }
    } catch (err) { setError(handleApiError(err).general); } finally { setLoading(false); }
  }, [page, sortColumn, sortOrder, patient, doctor, service, status, dateFrom, dateTo]);

  useEffect(() => { setPage(1); }, [patient, doctor, service, status, dateFrom, dateTo]);
  useEffect(() => { fetchAppointments(); }, [fetchAppointments]);
  
  const handleSortRequest = (propertyId) => {
    setSortOrder(prev => (sortColumn === propertyId && prev === 'asc' ? 'desc' : 'asc'));
    setSortColumn(propertyId);
  };
  const handleAutocompleteChange = (name, value) => setFilters(prev => ({ ...prev, [name]: value }));
  const handleDateChange = (name, date) => setFilters(prev => ({ ...prev, [name]: date }));
  const handleStatusChange = (event) => {
    const { target: { value } } = event;
    setFilters(prev => ({
      ...prev,
      // `value` теперь всегда будет массивом
      status: typeof value === 'string' ? value.split(',') : value,
    }));
  };
  const handlePageChange = (e, value) => setPage(value);
  
  const handleActionSuccess = (shouldReload = true) => {
    setOpenClinicCancel(false);
    setOpenRescheduleDialog(false);
    setSelectedAppointment(null);
    if (shouldReload) fetchAppointments();
  };
  const handleOpenDialog = (setter, appointment) => {
    setSelectedAppointment(appointment);
    setter(true);
  };
  const handleApproveByClinic = async (appointmentId) => {
    try {
      await appointmentService.approveByClinic({ appointmentId });
      fetchAppointments();
    } catch (err) { setError(handleApiError(err).general); }
  };
  
  const isMyAppointment = (appt) => user?.id === appt?.doctorId;
  const isNotOutdated = (appt) => new Date(appt?.dateTime) > Date.now();
  const isCancelable = (appt) => isMyAppointment(appt) && isNotOutdated(appt) && [APPOINTMENT_STATUSES.WaitingClinicConfirm,APPOINTMENT_STATUSES.WaitingUserConfirm, APPOINTMENT_STATUSES.Confirmed].includes(appt.status);
  const isReschedulable = (appt) => isMyAppointment(appt) && isNotOutdated(appt) && [APPOINTMENT_STATUSES.WaitingClinicConfirm, APPOINTMENT_STATUSES.Confirmed].includes(appt.status);
  const canApproveByClinic = (appt) => isMyAppointment(appt) && isNotOutdated(appt) && appt.status === APPOINTMENT_STATUSES.WaitingClinicConfirm;
  const pageCount = Math.ceil(totalCount / 10);

  return (
    <>
      <Paper sx={{ p: 2, boxShadow: 3 }}>
        <Typography variant="h4" sx={{ mb: 2 }}>Appointments</Typography>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <TableContainer>
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                {headCells.map((headCell) => (
                  <TableCell key={headCell.id} style={{ width: headCell.width }} sortDirection={sortColumn === headCell.id ? sortOrder : false} align={headCell.align || 'left'}>
                    {headCell.disableSorting ? (
                      headCell.label
                    ) : (
                      <TableSortLabel
                        active={sortColumn === headCell.id}
                        direction={sortOrder}
                        onClick={() => handleSortRequest(headCell.id)}
                      >
                        {headCell.label}
                      </TableSortLabel>
                    )}
                  </TableCell>
                ))}
              </TableRow>
              <TableRow>
                <TableCell sx={{ verticalAlign: 'bottom' }}><DatePicker label="From" value={filters.dateFrom} onChange={(date) => handleDateChange('dateFrom', date)} renderInput={(params) => <TextField {...params} size="small" variant="standard" fullWidth />} /><DatePicker label="To" value={filters.dateTo} onChange={(date) => handleDateChange('dateTo', date)} renderInput={(params) => <TextField {...params} size="small" variant="standard" fullWidth sx={{ mt: 1 }}/>} /></TableCell>
                <TableCell><Autocomplete options={patientOptions} getOptionLabel={(o) => o.lastName ? `${o.lastName} ${o.firstName}` : ''} isOptionEqualToValue={(o, v) => o.id === v.id} value={filters.patient} onChange={(e, val) => handleAutocompleteChange('patient', val)} onInputChange={(e, val) => setPatientInput(val)} renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Patient..." />} /></TableCell>
                <TableCell><Autocomplete options={doctorOptions} getOptionLabel={(o) => o.lastName ? `${o.lastName} ${o.firstName}` : ''} isOptionEqualToValue={(o, v) => o.id === v.id} value={filters.doctor} onChange={(e, val) => handleAutocompleteChange('doctor', val)} onInputChange={(e, val) => setDoctorInput(val)} renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Doctor..." />} /></TableCell>
                <TableCell><Autocomplete options={serviceOptions} getOptionLabel={(o) => o.name || ''} isOptionEqualToValue={(o, v) => o.id === v.id} value={filters.service} onChange={(e, val) => handleAutocompleteChange('service', val)} onInputChange={(e, val) => setServiceInput(val)} renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Service..." />} /></TableCell>
                <TableCell>
                  <FormControl variant="standard" fullWidth><Select
                  multiple // Это ключ к решению
                  name="Statuses"
                  value={filters.status || []} // value всегда должен быть массивом
                  onChange={handleStatusChange}
                  // renderValue показывает выбранные значения в виде чипсов
                  renderValue={(selected) => (
                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                      {selected.map((value) => (
                        <Chip key={value} label={APPOINTMENT_STATUS_TEXT[value] || '...'} size="small" />
                      ))}
                    </Box>
                  )}
                >
                  {/* Мы не можем иметь опцию "All", когда у нас multiple select */}
                  {/* <MenuItem value=""><em>All</em></MenuItem> */}
                  {Object.entries(APPOINTMENT_STATUSES).map(([key, value]) => (
                    <MenuItem key={value} value={value}>
                      {APPOINTMENT_STATUS_TEXT[value] || key}
                    </MenuItem>
                  ))}
                </Select></FormControl>
                </TableCell>
                <TableCell />
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? ( <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow> )
               : appointments.length > 0 ? (
                appointments.map((item) => (
                  <TableRow hover key={item.id} onClick={() => navigate(`/appointments/${item.id}`)} sx={{ cursor: 'pointer' }}>
                    <TableCell>{item.dateTime ? format(parseISO(item.dateTime), 'dd.MM.yyyy HH:mm') : 'N/A'}</TableCell>
                    <TableCell>{patientsMap[item.patientId] ? `${patientsMap[item.patientId].lastName} ${patientsMap[item.patientId].firstName}` : '...'}</TableCell>
                    <TableCell>{doctorsMap[item.doctorId] ? `${doctorsMap[item.doctorId].lastName} ${doctorsMap[item.doctorId].firstName}` : '...'}</TableCell>
                    <TableCell>{servicesMap[item.serviceId]?.name || '...'}</TableCell>
                    <TableCell><Chip label={APPOINTMENT_STATUS_TEXT[item.status] || 'Unknown'} color={APPOINTMENT_STATUS_COLORS[item.status] || 'default'} size="small" /></TableCell>
                    <TableCell align="right">
                      <Box sx={{ display: 'flex', justifyContent: 'flex-end' }} onClick={(e) => e.stopPropagation()}>
                        {canApproveByClinic(item) && (<Tooltip title="Approve"><IconButton color="primary" onClick={() => handleApproveByClinic(item.id)}><CheckCircleIcon /></IconButton></Tooltip>)}
                        {isReschedulable(item) && (<Tooltip title="Reschedule"><IconButton color="info" onClick={() => handleOpenDialog(setOpenRescheduleDialog, item)}><AutorenewIcon /></IconButton></Tooltip>)}
                        {isCancelable(item) && (<Tooltip title="Cancel"><IconButton color="error" onClick={() => handleOpenDialog(setOpenClinicCancel, item)}><CancelIcon /></IconButton></Tooltip>)}
                      </Box>
                    </TableCell>
                  </TableRow>
                ))
              ) : ( <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No appointments found</Typography></TableCell></TableRow> )}
            </TableBody>
          </Table>
        </TableContainer>
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
          <Pagination count={pageCount || 1} page={page} onChange={handlePageChange} color="primary" />
        </Box>
      </Paper>
      
      {selectedAppointment && (
        <>
          <CancelByClinicDialog open={openClinicCancel} onClose={() => handleActionSuccess(false)} appointment={selectedAppointment} onSuccess={() => handleActionSuccess(true)}/>
          <RescheduleDialog open={openRescheduleDialog} onClose={() => handleActionSuccess(false)} appointment={selectedAppointment} onSuccess={() => handleActionSuccess(true)}/>
        </>
      )}
    </>
  );
};

export default DoctorAppointmentList;