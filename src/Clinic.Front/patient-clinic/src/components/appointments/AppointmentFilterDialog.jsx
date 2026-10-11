// src/components/appointments/AppointmentFilterDialog.jsx
import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, TextField, Autocomplete, Select, MenuItem, FormControl, InputLabel, IconButton
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import CloseIcon from '@mui/icons-material/Close';

import doctorService from '../../services/doctorService';
import { APPOINTMENT_STATUSES, APPOINTMENT_STATUS_TEXT } from '../../constants/appointmentConstants';
import useDebounce from '../../hooks/useDebounce';

const AppointmentFilterDialog = ({ open, onClose, initialFilters, onApply }) => {
  const [filters, setFilters] = useState(initialFilters);

  const [doctorOptions, setDoctorOptions] = useState([]);
  const [doctorInput, setDoctorInput] = useState('');
  const debouncedDoctorInput = useDebounce(doctorInput, 400);

  // Синхронизируем внутреннее состояние с пропами при открытии
  useEffect(() => {
    if (open) {
      setFilters(initialFilters);
    }
  }, [open, initialFilters]);
  
  // Загрузка опций для Autocomplete
  useEffect(() => {
    doctorService.getPaged({ LastName: debouncedDoctorInput, page: 1, pageSize: 20 })
      .then(res => setDoctorOptions(res.items || []));
  }, [debouncedDoctorInput]);

  const handleAutocompleteChange = (name, value) => {
    setFilters(prev => ({ ...prev, [name]: value }));
  };
  const handleDateChange = (name, date) => {
    setFilters(prev => ({ ...prev, [name]: date }));
  };
  const handleStatusChange = (event) => {
    setFilters(prev => ({ ...prev, status: event.target.value }));
  };

  const handleApplyFilters = () => {
    onApply(filters);
    onClose();
  };

  const handleResetFilters = () => {
    const resetState = {
      doctor: null,
      status: '',
      dateFrom: null,
      dateTo: null,
    };
    setFilters(resetState);
    onApply(resetState);
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle sx={{ textAlign: 'center' }}>
        Filter Appointments
        <IconButton aria-label="close" onClick={onClose} sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        {/* --- ИСПОЛЬЗУЕМ FLEXBOX ДЛЯ ВЕРТИКАЛЬНОГО РАСПОЛОЖЕНИЯ --- */}
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, pt: 2 }}>
          <Autocomplete 
            options={doctorOptions} 
            getOptionLabel={(o) => o.lastName ? `${o.lastName} ${o.firstName}` : ''} 
            isOptionEqualToValue={(o, v) => o.id === v.id} 
            value={filters.doctor} 
            onInputChange={(e, val) => setDoctorInput(val)} 
            onChange={(e, val) => handleAutocompleteChange('doctor', val)} 
            renderInput={(params) => <TextField {...params} label="Filter by Doctor" />} 
          />
          
          <FormControl fullWidth>
            <InputLabel id="status-filter-label">Status</InputLabel>
            <Select
              labelId="status-filter-label"
              label="Status"
              value={filters.status}
              onChange={handleStatusChange}
            >
              <MenuItem value=""><em>All Statuses</em></MenuItem>
              {Object.entries(APPOINTMENT_STATUSES).map(([key, value]) => (
                <MenuItem key={value} value={value}>
                  {APPOINTMENT_STATUS_TEXT[value] || key}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <DatePicker 
            label="From Date" 
            value={filters.dateFrom} 
            onChange={(date) => handleDateChange('dateFrom', date)} 
            renderInput={(params) => <TextField {...params} fullWidth />} 
          />

          <DatePicker 
            label="To Date" 
            value={filters.dateTo} 
            onChange={(date) => handleDateChange('dateTo', date)} 
            renderInput={(params) => <TextField {...params} fullWidth />} 
          />
        </Box>
      </DialogContent>
      <DialogActions sx={{ p: '16px 24px', justifyContent: 'space-between' }}>
        <Button onClick={handleResetFilters}>Reset</Button>
        <Box>
          <Button onClick={onClose} sx={{ mr: 1 }}>Cancel</Button>
          <Button onClick={handleApplyFilters} variant="contained">Apply</Button>
        </Box>
      </DialogActions>
    </Dialog>
  );
};

export default AppointmentFilterDialog;