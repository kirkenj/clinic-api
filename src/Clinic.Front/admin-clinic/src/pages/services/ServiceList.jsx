// src/pages/services/ServiceList.jsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  Box, Paper, Typography, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TableSortLabel, IconButton, Button, Pagination,
  CircularProgress, Alert, TextField, Autocomplete, Dialog, DialogTitle,
  DialogContent, DialogContentText, DialogActions 
} from '@mui/material';
import { useNavigate, Link } from 'react-router-dom';
import EditIcon from '@mui/icons-material/Edit';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete'; // 1. Импортируем иконку

import serviceService from '../../services/serviceService';
import specializationService from '../../services/specializationService';
import { SERVICE_SORT_COLUMNS } from '../../constants/sortColumns';
import { handleApiError } from '../../utils/handleApiError';
import useDebounce from '../../hooks/useDebounce';
import { cleanObject } from '../../utils/cleanObject';

const headCells = [
  { id: 'name', label: 'Service Name', width: '40%' },
  { id: 'price', label: 'Price', width: '20%' },
  { id: 'specializationId', label: 'Specialization', disableSorting: true, width: '30%' },
  { id: 'actions', label: 'Actions', disableSorting: true, align: 'right', width: '10%' },
];

const ServiceList = () => {
  const navigate = useNavigate();
  
  const [services, setServices] = useState([]);
  const [specializationsMap, setSpecializationsMap] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('asc');
  const [sortColumn, setSortColumn] = useState('name');
  
  // --- Состояния для фильтров ---
  const [filters, setFilters] = useState({
    name: '',
    priceStart: '',
    priceEnd: '',
    specialization: null,
  });

  const debouncedName = useDebounce(filters.name, 500);
  const debouncedPriceStart = useDebounce(filters.priceStart, 500);
  const debouncedPriceEnd = useDebounce(filters.priceEnd, 500);

  const [specOptions, setSpecOptions] = useState([]);
  const [specInput, setSpecInput] = useState('');
  const debouncedSpecInput = useDebounce(specInput, 400);

  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [selectedService, setSelectedService] = useState(null);

  useEffect(() => {
    specializationService.getPaged({ Name: debouncedSpecInput, page: 1, pageSize: 20 })
      .then(res => setSpecOptions(res.items || []));
  }, [debouncedSpecInput]);

  const { name, priceStart, priceEnd, specialization } = filters;

  const fetchServices = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const params = {
        page,
        pageSize: 10,
        SortColumn: SERVICE_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        Name: debouncedName,
        PriceStart: debouncedPriceStart,
        PriceEnd: debouncedPriceEnd,
        SpecializationIds: specialization ? [specialization.id] : null,
      };
      
      const cleanedParams = cleanObject(params);
      const response = await serviceService.getPaged(cleanedParams);
      const serviceData = response.items || [];
      setServices(serviceData);
      setTotalCount(response.totalCount || 0);

      if (serviceData.length > 0) {
        const specializationIds = [...new Set(serviceData.map(s => s.specializationId).filter(Boolean))];
        if (specializationIds.length > 0) {
          const specResponse = await specializationService.getPaged({ Ids: specializationIds, page: 1, pageSize: specializationIds.length });
          const specMap = (specResponse.items || []).reduce((acc, item) => ({ ...acc, [item.id]: item.name }), {});
          setSpecializationsMap(prev => ({ ...prev, ...specMap }));
        }
      }
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, sortColumn, sortOrder, debouncedName, debouncedPriceStart, debouncedPriceEnd, specialization]);

  useEffect(() => { setPage(1); }, [debouncedName, debouncedPriceStart, debouncedPriceEnd, specialization]);
  useEffect(() => { fetchServices(); }, [fetchServices]);

  const handleFilterChange = (e) => setFilters(prev => ({ ...prev, [e.target.name]: e.target.value }));
  const handleAutocompleteChange = (name, value) => setFilters(prev => ({ ...prev, [name]: value }));
  const handleSortRequest = (propertyId) => {
    const isAsc = sortColumn === propertyId && sortOrder === 'asc';
    setSortOrder(isAsc ? 'desc' : 'asc');
    setSortColumn(propertyId);
  };
  const handlePageChange = (event, value) => setPage(value);
  const pageCount = Math.ceil(totalCount / 10);

  const handleClickOpenDeleteDialog = (service) => {
    setSelectedService(service);
    setOpenDeleteDialog(true);
  };
  const handleCloseDeleteDialog = () => {
    setSelectedService(null);
    setOpenDeleteDialog(false);
  };

  const handleDelete = async () => {
    if (!selectedService) return;
    try {
      await serviceService.delete(selectedService.id);
      handleCloseDeleteDialog();
      fetchServices();
    } catch (err) {
      setError(handleApiError(err).general);
    }
  };

  return (
    <>
    <Paper sx={{ p: 2, boxShadow: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4">Services</Typography>
        <Button component={Link} to="/services/new" variant="contained" startIcon={<AddIcon />}>Add Service</Button>
      </Box>
      
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      <TableContainer>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              {headCells.map((headCell) => (
                <TableCell key={headCell.id} align={headCell.align || 'left'} style={{ width: headCell.width }} sortDirection={sortColumn === headCell.id ? sortOrder : false}>
                  <TableSortLabel active={sortColumn === headCell.id} direction={sortOrder} onClick={() => handleSortRequest(headCell.id)}>
                    {headCell.label}
                  </TableSortLabel>
                </TableCell>
              ))}
            </TableRow>
            <TableRow>
              <TableCell><TextField fullWidth name="name" value={filters.name} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Name..." /></TableCell>
              <TableCell>
                <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                  <TextField name="priceStart" value={filters.priceStart} onChange={handleFilterChange} size="small" variant="standard" placeholder="from" type="number" />
                  <span>-</span>
                  <TextField name="priceEnd" value={filters.priceEnd} onChange={handleFilterChange} size="small" variant="standard" placeholder="to" type="number" />
                </Box>
              </TableCell>
              <TableCell>
                <Autocomplete fullWidth size="small"
                  options={specOptions}
                  getOptionLabel={(o) => o.name || ''}
                  isOptionEqualToValue={(o, val) => o.id === val.id}
                  value={filters.specialization}
                  onChange={(e, val) => handleAutocompleteChange('specialization', val)}
                  onInputChange={(e, val) => setSpecInput(val)}
                  renderInput={(params) => <TextField {...params} variant="standard" placeholder="by Specialization..." />}
                />
              </TableCell>
              <TableCell />
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow>
            ) : services.length > 0 ? (
              services.map((service) => (
                <TableRow hover key={service.id}>
                  <TableCell>{service.name}</TableCell>
                  <TableCell>${service.price.toFixed(2)}</TableCell>
                  <TableCell>{specializationsMap[service.specializationId] || 'Not specified'}</TableCell>
                  <TableCell align="right">
                    <IconButton aria-label="edit service" onClick={() => navigate(`/services/edit/${service.id}`)}><EditIcon /></IconButton>
                    <IconButton aria-label="delete service" onClick={() => handleClickOpenDeleteDialog(service)}><DeleteIcon /></IconButton>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No services found</Typography></TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
        <Pagination count={pageCount || 1} page={page} onChange={handlePageChange} color="primary" showFirstButton showLastButton />
      </Box>
    </Paper>

          {/* 5. Диалог для подтверждения удаления */}
      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete the service "<strong>{selectedService?.name}</strong>"? This action cannot be undone.
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

export default ServiceList;