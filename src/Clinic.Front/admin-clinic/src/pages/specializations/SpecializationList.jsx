import React, { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Paper, CircularProgress, Alert, Pagination,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel, IconButton,
  TextField, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle,
  Tooltip

} from '@mui/material';
import MedicalServicesIcon from '@mui/icons-material/MedicalServices'; // Иконка для услуг
import GroupIcon from '@mui/icons-material/Group'; // Иконка для докторов

import { Link, useNavigate } from 'react-router-dom';
import EditIcon from '@mui/icons-material/Edit';
import AddIcon from '@mui/icons-material/Add';
import DeleteIcon from '@mui/icons-material/Delete';

import specializationService from '../../services/specializationService';
import { handleApiError } from '../../utils/handleApiError';
import { SPECIALIZATION_SORT_COLUMNS } from '../../constants/sortColumns';
import useDebounce from '../../hooks/useDebounce';
import { cleanObject } from '../../utils/cleanObject';

// Конфигурация заголовков таблицы
const headCells = [
  { id: 'name', label: 'Specialization Name', width: '90%' },
  { id: 'actions', label: 'Actions', disableSorting: true, align: 'right', width: '10%' },
];

const SpecializationList = () => {
  const navigate = useNavigate();
  // --- Состояния ---
  const [specializations, setSpecializations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('asc');
  const [sortColumn, setSortColumn] = useState('name');
  
  const [filterName, setFilterName] = useState('');
  const debouncedName = useDebounce(filterName, 500);

  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [selectedSpec, setSelectedSpec] = useState(null);

  // --- Загрузка данных ---
  const fetchSpecializations = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const params = {
        page,
        pageSize: 10,
        SortColumn: SPECIALIZATION_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        Name: debouncedName,
      };
      const cleanedParams = cleanObject(params);
      const response = await specializationService.getPaged(cleanedParams);
      setSpecializations(response.items || []);
      setTotalCount(response.totalCount || 0);
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, sortColumn, sortOrder, debouncedName]);

  useEffect(() => { setPage(1); }, [debouncedName]);
  useEffect(() => { fetchSpecializations(); }, [fetchSpecializations]);

  // --- Хендлеры ---
  const handleSortRequest = (propertyId) => {
    const isAsc = sortColumn === propertyId && sortOrder === 'asc';
    setSortOrder(isAsc ? 'desc' : 'asc');
    setSortColumn(propertyId);
  };
  const handlePageChange = (event, value) => {
    setPage(value);
  };

  const handleClickOpenDeleteDialog = (spec) => {
    setSelectedSpec(spec);
    setOpenDeleteDialog(true);
  };
  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setSelectedSpec(null);
  };
  const handleDelete = async () => {
    if (!selectedSpec) return;
    try {
      await specializationService.delete(selectedSpec.id);
      handleCloseDeleteDialog();
      fetchSpecializations(); 
    } catch (err) {
      // Можно было бы отображать ошибку прямо в диалоге
      setError(handleApiError(err).general);
      handleCloseDeleteDialog(); // Закрываем диалог даже при ошибке
    }
  };

  const pageCount = Math.ceil(totalCount / 10);

  // --- Рендер ---
  return (
    <>
      <Paper sx={{ p: 2, boxShadow: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h4">Specializations</Typography>
          <Button component={Link} to="/specializations/new" variant="contained" startIcon={<AddIcon />}>
            Create
          </Button>
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
                <TableCell>
                  <TextField 
                    fullWidth 
                    name="name" 
                    value={filterName} 
                    onChange={(e) => setFilterName(e.target.value)} 
                    size="small" 
                    variant="standard" 
                    placeholder="by Name..." 
                  />
                </TableCell>
                <TableCell />
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow>
              ) : specializations.length > 0 ? (
                specializations.map((specialization) => (
                  <TableRow hover key={specialization.id}>
                    <TableCell>{specialization.name}</TableCell>
                    <TableCell align="right">
                      {/* --- ИСПРАВЛЕНИЕ 1: Оборачиваем кнопки в Box --- */}
                      <Box sx={{ display: 'flex', justifyContent: 'flex-end', flexWrap: 'nowrap' }}>
                        <Tooltip title="View Doctors">
                          {/* --- ИСПРАВЛЕНИЕ 2: Передаем правильный ключ `specialization` --- */}
                          <IconButton onClick={() => navigate('/doctors', { state: { specialization } })}>
                            <GroupIcon />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="View Services">
                          {/* --- ИСПРАВЛЕНИЕ 2: Передаем правильный ключ `specialization` --- */}
                          <IconButton onClick={() => navigate('/services', { state: { specialization } })}>
                            <MedicalServicesIcon />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Edit Specialization">
                            <IconButton aria-label="edit specialization" onClick={() => navigate(`/specializations/edit/${specialization.id}`)}>
                                <EditIcon />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Delete Specialization">
                            <IconButton aria-label="delete specialization" onClick={() => handleClickOpenDeleteDialog(specialization)}>
                                <DeleteIcon />
                            </IconButton>
                        </Tooltip>
                      </Box>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No specializations found</Typography></TableCell></TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
          <Pagination 
            count={pageCount || 1} 
            page={page} 
            onChange={handlePageChange} 
            color="primary" 
            showFirstButton 
            showLastButton 
          />
        </Box>
      </Paper>
      
      <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
        <DialogTitle>Confirm Deletion</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to delete the specialization "<strong>{selectedSpec?.name}</strong>"? This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDeleteDialog}>Cancel</Button>
          <Button onClick={handleDelete} autoFocus>Delete</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default SpecializationList;