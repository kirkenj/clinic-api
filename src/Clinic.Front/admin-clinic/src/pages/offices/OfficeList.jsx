// src/pages/offices/OfficeList.jsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Paper, CircularProgress, Alert, Pagination,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel, IconButton,
  Button, TextField, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle
} from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import AddIcon from '@mui/icons-material/Add';
import officeService from '../../services/officeService';
import { handleApiError } from '../../utils/handleApiError';
import { OFFICE_SORT_COLUMNS } from '../../constants/sortColumns';
import useDebounce from '../../hooks/useDebounce';
import { cleanObject } from '../../utils/cleanObject'; // Импортируем функцию для очистки объекта

// Обновляем заголовки, чтобы выделить место под кнопки
const headCells = [
  { id: 'address', label: 'Address', width: '45%' },
  { id: 'phoneNumber', label: 'Phone Number', width: '40%' },
  { id: 'actions', label: 'Actions', disableSorting: true, align: 'right', width: '15%' },
];

const OfficeList = () => {
  const navigate = useNavigate();

  // Состояния
  const [offices, setOffices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('asc');
  const [sortColumn, setSortColumn] = useState('address');
  const [filters, setFilters] = useState({ address: '', phoneNumber: '' });
  const debouncedAddress = useDebounce(filters.address, 500);
  const debouncedPhone = useDebounce(filters.phoneNumber, 500);

  // 1. Добавляем состояния для диалога удаления
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [selectedOffice, setSelectedOffice] = useState(null);

  // Функция загрузки данных
  const fetchOffices = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const params = {
        page,
        pageSize: 10,
        SortColumn: OFFICE_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        Address: debouncedAddress,
        PhoneNumber: debouncedPhone,
      };

      const cleanedParams = cleanObject(params);      
      const response = await officeService.getPaged(cleanedParams);
      setOffices(response.items || []);
      setTotalCount(response.totalCount || 0);
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, sortColumn, sortOrder, debouncedAddress, debouncedPhone]);

  useEffect(() => { setPage(1); }, [debouncedAddress, debouncedPhone]);
  useEffect(() => { fetchOffices(); }, [fetchOffices]);

  // Хендлеры
  const handleFilterChange = (e) => setFilters(prev => ({ ...prev, [e.target.name]: e.target.value }));
  const handleSortRequest = (propertyId) => {
    const isAsc = sortColumn === propertyId && sortOrder === 'asc';
    setSortOrder(isAsc ? 'desc' : 'asc');
    setSortColumn(propertyId);
  };
  const handlePageChange = (e, value) => setPage(value);
  const pageCount = Math.ceil(totalCount / 10);

  // --- 2. Логика для управления диалогом и удалением ---
  const handleClickOpenDeleteDialog = (office) => {
    setSelectedOffice(office);
    setOpenDeleteDialog(true);
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setSelectedOffice(null);
  };

  const handleDelete = async () => {
    if (!selectedOffice) return;
    try {
      await officeService.delete(selectedOffice.id);
      handleCloseDeleteDialog();
      fetchOffices(); // Перезагружаем список
    } catch (err) {
      setError(handleApiError(err).general);
    }
  };

  return (
    <>
      <Paper sx={{ p: 2, boxShadow: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h4">Offices</Typography>
          <Button component={Link} to="/offices/new" variant="contained" startIcon={<AddIcon />}>Create</Button>
        </Box>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

        <TableContainer>
          <Table stickyHeader>
            <TableHead>
              {/* Заголовки и фильтры в одной строке */}
              <TableRow>
                {headCells.map((headCell) => (
                  <TableCell key={headCell.id} align={headCell.align || 'left'} style={{ width: headCell.width }} sortDirection={sortColumn === headCell.id ? sortOrder : false}>
                    <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                      <TableSortLabel active={sortColumn === headCell.id} direction={sortOrder} onClick={() => !headCell.disableSorting && handleSortRequest(headCell.id)} hideSortIcon={headCell.disableSorting} sx={{ '& .MuiTableSortLabel-icon': { opacity: headCell.disableSorting ? 0 : 1 }}}>
                        {headCell.label}
                      </TableSortLabel>
                      {headCell.id === 'address' && <TextField name="address" value={filters.address} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Address..." sx={{ mt: 1 }} />}
                      {headCell.id === 'phoneNumber' && <TextField name="phoneNumber" value={filters.phoneNumber} onChange={handleFilterChange} size="small" variant="standard" placeholder="by Phone..." sx={{ mt: 1 }} />}
                    </Box>
                  </TableCell>
                ))}
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow>
              ) : offices.length > 0 ? (
                offices.map((office) => (
                  <TableRow hover key={office.id}>
                    <TableCell>{office.address}</TableCell>
                    <TableCell>{office.phoneNumber}</TableCell>
                    {/* 3. Добавляем кнопки действий */}
                    <TableCell align="right">
                      <IconButton aria-label="edit office" onClick={() => navigate(`/offices/edit/${office.id}`)}><EditIcon /></IconButton>
                      <IconButton aria-label="delete office" onClick={() => handleClickOpenDeleteDialog(office)}><DeleteIcon /></IconButton>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No offices found</Typography></TableCell></TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
          <Pagination count={pageCount || 1} page={page} onChange={handlePageChange} color="primary" showFirstButton showLastButton />
        </Box>
      </Paper>

      {/* 4. Диалоговое окно подтверждения удаления */}
      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete the office at "<strong>{selectedOffice?.address}</strong>"? This action cannot be undone.
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

export default OfficeList;