import React, { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Paper, CircularProgress, Alert, Pagination,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel, IconButton,
  TextField, Autocomplete, Button, Tooltip,
  Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle
} from '@mui/material';
import { useNavigate, Link } from 'react-router-dom';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import { useLocation } from 'react-router-dom'; 
import doctorService from '../../services/doctorService';
import specializationService from '../../services/specializationService';
import officeService from '../../services/officeService';
import { handleApiError } from '../../utils/handleApiError';
import { DOCTOR_SORT_COLUMNS } from '../../constants/sortColumns';
import useDebounce from '../../hooks/useDebounce';
import { cleanObject } from '../../utils/cleanObject'; 

const headCells = [
  { id: 'lastName', label: 'Last Name', width: '15%' },
  { id: 'firstName', label: 'First Name', width: '15%' },
  { id: 'email', label: 'Email', width: '20%' },
  { id: 'phoneNumber', label: 'Phone Number', width: '15%', disableSorting: false },
  { id: 'specializationId', label: 'Specialization', disableSorting: true, width: '15%' },
  { id: 'officeId', label: 'Office', disableSorting: true, width: '10%' },
  { id: 'actions', label: 'Actions', disableSorting: true, align: 'right', width: '10%' },
];

const DoctorList = () => {
  const navigate = useNavigate();
  const location = useLocation(); 
  // Data states
  const [doctors, setDoctors] = useState([]);
  const [specializationsMap, setSpecializationsMap] = useState({});
  const [officesMap, setOfficesMap] = useState({});
  const [specOptions, setSpecOptions] = useState([]);
  const [officeOptions, setOfficeOptions] = useState([]);

  // UI states
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Pagination and Sorting states
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('asc');
  const [sortColumn, setSortColumn] = useState('lastName');
  
  // Filters state
  const [filters, setFilters] = useState({
    lastName: '',
    firstName: '',
    email: '',
    phoneNumber: '',
    specializationId: location.state?.specialization?.id || null,
    officeId: location.state?.office?.id || null,
  });

  // Debounced filter values
  const debouncedLastName = useDebounce(filters.lastName, 500);
  const debouncedFirstName = useDebounce(filters.firstName, 500);
  const debouncedEmail = useDebounce(filters.email, 500);
  const debouncedPhone = useDebounce(filters.phoneNumber, 500);

  // Autocomplete input states
  const [specInput, setSpecInput] = useState('');
  const [officeInput, setOfficeInput] = useState('');
  const debouncedSpecInput = useDebounce(specInput, 400);
  const debouncedOfficeInput = useDebounce(officeInput, 400);
  
  // State for delete confirmation dialog
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState(null);


  // --- Data Fetching ---

  useEffect(() => {
    specializationService.getPaged({ Name: debouncedSpecInput, page: 1, pageSize: 20 })
      .then(response => setSpecOptions(response.items || []));
  }, [debouncedSpecInput]);
  
  useEffect(() => {
    officeService.getPaged({ Address: debouncedOfficeInput, page: 1, pageSize: 20 })
      .then(response => setOfficeOptions(response.items || []));
  }, [debouncedOfficeInput]);

  const fetchDoctors = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const params = {
        page,
        pageSize: 10,
        SortColumn: DOCTOR_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        LastName: debouncedLastName,
        FirstName: debouncedFirstName,
        Email: debouncedEmail,
        PhoneNumber: debouncedPhone,
      };
      if (filters.specializationId) params.SpecializationIds = [filters.specializationId];
      if (filters.officeId) params.OfficeIds = [filters.officeId];

      // 2. "Чистим" объект params перед отправкой
      const cleanedParams = cleanObject(params);
      
      console.log("Отправка параметров:", cleanedParams); // Для отладки

      // 3. Передаем очищенный объект в сервис
      const doctorsResponse = await doctorService.getPaged(cleanedParams);
      const doctorsData = doctorsResponse.items || [];
      setDoctors(doctorsData);
      setTotalCount(doctorsResponse.totalCount || 0);

      if (doctorsData.length === 0) return;

      const specializationIds = [...new Set(doctorsData.map(d => d.specializationId).filter(Boolean))];
      const officeIds = [...new Set(doctorsData.map(d => d.officeId).filter(Boolean))];
      
      const [specializationsResponse, officesResponse] = await Promise.all([
        specializationIds.length > 0 ? specializationService.getPaged({ Ids: specializationIds, page: 1, pageSize: specializationIds.length }) : Promise.resolve({ items: [] }),
        officeIds.length > 0 ? officeService.getPaged({ Ids: officeIds, page: 1, pageSize: officeIds.length }) : Promise.resolve({ items: [] })
      ]);

      setSpecializationsMap(prevMap => ({
        ...prevMap,
        ...(specializationsResponse.items || []).reduce((acc, spec) => ({ ...acc, [spec.id]: spec.name }), {})
      }));
      setOfficesMap(prevMap => ({
        ...prevMap,
        ...(officesResponse.items || []).reduce((acc, office) => ({ ...acc, [office.id]: office.address }), {})
      }));
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, sortColumn, sortOrder, debouncedLastName, debouncedFirstName, debouncedEmail, debouncedPhone, filters.specializationId, filters.officeId]);

  useEffect(() => { setPage(1); }, [debouncedLastName, debouncedFirstName, debouncedEmail, debouncedPhone, filters.specializationId, filters.officeId]);
  useEffect(() => { fetchDoctors(); }, [fetchDoctors]);

  // --- Handlers ---
  const handleFilterChange = (e) => setFilters(prev => ({ ...prev, [e.target.name]: e.target.value }));
  const handleAutocompleteChange = (name, value) => setFilters(prev => ({ ...prev, [name]: value ? value.id : null }));
  const handleSortRequest = (propertyId) => {
    const isAsc = sortColumn === propertyId && sortOrder === 'asc';
    setSortOrder(isAsc ? 'desc' : 'asc');
    setSortColumn(propertyId);
  };
  const handlePageChange = (e, value) => setPage(value);
  
  const handleClickOpenDeleteDialog = (doctor) => {
    setSelectedDoctor(doctor);
    setOpenDeleteDialog(true);
  };
  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setSelectedDoctor(null);
  };
  const handleDelete = async () => {
    if (!selectedDoctor) return;
    try {
      await doctorService.delete(selectedDoctor.id);
      handleCloseDeleteDialog();
      fetchDoctors();
    } catch (err) {
      setError(handleApiError(err).general);
    }
  };

  const pageCount = Math.ceil(totalCount / 10);

  // --- Helper functions for rendering ---
  const getSpecializationName = (id) => specializationsMap[id] || 'Not specified';
  const getOfficeAddress = (id) => officesMap[id] || 'Not specified';

  // --- Render ---
  return (
    <>
      <Paper sx={{ p: 2, boxShadow: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h4">Doctors</Typography>
          <Button component={Link} to="/doctors/new" variant="contained" startIcon={<AddIcon />}>Add Doctor</Button>
        </Box>
        
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <TableContainer>
          <Table stickyHeader>
            <TableHead>
              <TableRow>
                {headCells.map((headCell) => (
                  <TableCell key={headCell.id} align={headCell.align || 'left'} style={{ width: headCell.width }} sortDirection={sortColumn === headCell.id ? sortOrder : false}>
                    {!headCell.disableSorting ? (
                      <TableSortLabel active={sortColumn === headCell.id} direction={sortOrder} onClick={() => handleSortRequest(headCell.id)}>
                        {headCell.label}
                      </TableSortLabel>
                    ) : (
                      headCell.label
                    )}
                  </TableCell>
                ))}
              </TableRow>
              <TableRow>
                <TableCell><TextField fullWidth name="lastName" value={filters.lastName} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Last Name..." /></TableCell>
                <TableCell><TextField fullWidth name="firstName" value={filters.firstName} onChange={handleFilterChange} size="small" variant="standard" placeholder="by First Name..." /></TableCell>
                <TableCell><TextField fullWidth name="email" value={filters.email} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Email..." /></TableCell>
                <TableCell><TextField fullWidth name="phoneNumber" value={filters.phoneNumber} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Phone..." /></TableCell>
                <TableCell>
                  <Autocomplete fullWidth size="small"
                    options={specOptions}
                    getOptionLabel={(o) => o.name || ''}
                    isOptionEqualToValue={(o, val) => o.id === val.id}
                    value={specOptions.find(o => o.id === filters.specializationId) || null}
                    onChange={(e, val) => handleAutocompleteChange('specializationId', val)}
                    onInputChange={(e, val) => setSpecInput(val)}
                    renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Specialization..." />}
                  />
                </TableCell>
                <TableCell>
                  <Autocomplete fullWidth size="small"
                    options={officeOptions}
                    getOptionLabel={(o) => o.address || ''}
                    isOptionEqualToValue={(o, val) => o.id === val.id}
                    value={officeOptions.find(o => o.id === filters.officeId) || null}
                    onChange={(e, val) => handleAutocompleteChange('officeId', val)}
                    onInputChange={(e, val) => setOfficeInput(val)}
                    renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Office..." />}
                  />
                </TableCell>
                <TableCell />
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow>
              ) : doctors.length > 0 ? (
                doctors.map((doctor) => (
                  <TableRow hover key={doctor.id}>
                    <TableCell>{doctor.lastName}</TableCell>
                    <TableCell>{doctor.firstName}</TableCell>
                    <TableCell>{doctor.email}</TableCell>
                    <TableCell>{doctor.phoneNumber}</TableCell>
                    <TableCell>{getSpecializationName(doctor.specializationId)}</TableCell>
                    <TableCell>{getOfficeAddress(doctor.officeId)}</TableCell>
                    <TableCell align="right">
                      <Tooltip title="View Appointments">
                        <IconButton onClick={() => navigate('/appointments', { state: { doctor } })}> <CalendarMonthIcon /> </IconButton>
                      </Tooltip>
                      <IconButton aria-label="edit doctor" onClick={() => navigate(`/doctors/edit/${doctor.id}`)}><EditIcon /></IconButton>
                      <IconButton aria-label="delete doctor" onClick={() => handleClickOpenDeleteDialog(doctor)}><DeleteIcon /></IconButton>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No doctors found</Typography></TableCell></TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
          <Pagination count={pageCount || 1} page={page} onChange={handlePageChange} color="primary" showFirstButton showLastButton />
        </Box>
      </Paper>

      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete the doctor "<strong>{`${selectedDoctor?.firstName} ${selectedDoctor?.lastName}`}</strong>"? This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog}>Cancel</Button>
          <Button onClick={handleDelete} color="error" autoFocus>Delete</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default DoctorList;