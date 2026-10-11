// src/pages/admins/AdminList.jsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  Box, Typography, Paper, CircularProgress, Alert, Pagination,
  TextField, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel
} from '@mui/material';
import adminService from '../../services/adminService';
import { handleApiError } from '../../utils/handleApiError';
import { RECEPTIONIST_SORT_COLUMNS } from '../../constants/sortColumns';
import useDebounce from '../../hooks/useDebounce';

const headCells = [
  { id: 'lastName', label: 'Last Name', width: '25%' },
  { id: 'firstName', label: 'First Name', width: '25%' },
  { id: 'email', label: 'Email', width: '25%' },
  { id: 'phoneNumber', label: 'Phone Number', width: '25%', disableSorting: false },
];

const AdminList = () => {
  const [admins, setAdmins] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [sortOrder, setSortOrder] = useState('asc');
  const [sortColumn, setSortColumn] = useState('lastName');
  const [filters, setFilters] = useState({
    lastName: '',
    firstName: '',
    email: '',
    phoneNumber: ''
  });

  const debouncedLastName = useDebounce(filters.lastName, 500);
  const debouncedFirstName = useDebounce(filters.firstName, 500);
  const debouncedEmail = useDebounce(filters.email, 500);
  const debouncedPhone = useDebounce(filters.phoneNumber, 500);

  const fetchAdmins = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const params = {
        page,
        pageSize: 10,
        SortColumn: RECEPTIONIST_SORT_COLUMNS[sortColumn],
        OrderDescending: sortOrder === 'desc',
        LastName: debouncedLastName,
        FirstName: debouncedFirstName,
        Email: debouncedEmail,
        PhoneNumber: debouncedPhone,
      };
      const response = await adminService.getPaged(params);
      setAdmins(response.items || []);
      setTotalCount(response.totalCount || 0);
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [page, sortColumn, sortOrder, debouncedLastName, debouncedFirstName, debouncedEmail, debouncedPhone]);

  useEffect(() => { setPage(1); }, [debouncedLastName, debouncedFirstName, debouncedEmail, debouncedPhone]);
  useEffect(() => { fetchAdmins(); }, [fetchAdmins]);
  
  const handleFilterChange = (e) => setFilters(prev => ({ ...prev, [e.target.name]: e.target.value }));
  const handleSortRequest = (propertyId) => {
    const isAsc = sortColumn === propertyId && sortOrder === 'asc';
    setSortOrder(isAsc ? 'desc' : 'asc');
    setSortColumn(propertyId);
  };
  const handlePageChange = (e, value) => setPage(value);
  
  const pageCount = Math.ceil(totalCount / 10);

  return (
    <Paper sx={{ p: 2, boxShadow: 3 }}>
      <Typography variant="h4" sx={{ mb: 2 }}>Administrators</Typography>
      
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
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><CircularProgress /></TableCell></TableRow>
            ) : admins.length > 0 ? (
              admins.map((admin) => (
                <TableRow hover key={admin.id}>
                  <TableCell>{admin.lastName}</TableCell>
                  <TableCell>{admin.firstName}</TableCell>
                  <TableCell>{admin.email}</TableCell>
                  <TableCell>{admin.phoneNumber}</TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow><TableCell colSpan={headCells.length} align="center" sx={{ py: 4 }}><Typography>No administrators found.</Typography></TableCell></TableRow>
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 3 }}>
        <Pagination count={pageCount || 1} page={page} onChange={handlePageChange} color="primary" showFirstButton showLastButton />
      </Box>
    </Paper>
  );
};

export default AdminList;