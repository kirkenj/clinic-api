// src/pages/support/Support.jsx
import React, { useState, useEffect } from 'react';
import {
  Box, Paper, Typography, CircularProgress, Alert,
  List, ListItem, ListItemText, ListItemAvatar, Avatar
} from '@mui/material';
import ContactSupportIcon from '@mui/icons-material/ContactSupport';
import adminService from '../../services/adminService';
import { handleApiError } from '../../utils/handleApiError';

const Support = () => {
  const [receptionists, setReceptionists] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Загружаем список администраторов при монтировании
  useEffect(() => {
    const fetchAdmins = async () => {
      setLoading(true);
      try {
        // Запрашиваем первую страницу с небольшим количеством, нам не нужны все
        const response = await adminService.getPaged({ page: 1, pageSize: 10 });
        setReceptionists(response.items || []);
      } catch (err) {
        setError(handleApiError(err).general);
      } finally {
        setLoading(false);
      }
    };
    fetchAdmins();
  }, []);

  return (
    <Paper sx={{ p: 3, maxWidth: '700px', mx: 'auto' }}>
      <Box sx={{ textAlign: 'center', mb: 3 }}>
        <ContactSupportIcon color="primary" sx={{ fontSize: 60 }} />
        <Typography variant="h4" gutterBottom>
          Support & Contacts
        </Typography>
        <Typography color="text.secondary">
          If you have any issues with booking or need assistance, please contact one of our administrators.
        </Typography>
      </Box>
      
      {error && <Alert severity="error">{error}</Alert>}
      
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}><CircularProgress /></Box>
      ) : (
        <List>
          {receptionists.map(admin => (
            <ListItem key={admin.id}>
              <ListItemAvatar>
                <Avatar>{admin.firstName?.charAt(0)}{admin.lastName?.charAt(0)}</Avatar>
              </ListItemAvatar>
              <ListItemText
                primary={`${admin.firstName} ${admin.lastName}`}
                secondary={
                  <>
                    <Typography component="span" display="block" variant="body2">
                      Email: {admin.email}
                    </Typography>
                    <Typography component="span" display="block" variant="body2">
                      Phone: {admin.phoneNumber || 'Not specified'}
                    </Typography>
                  </>
                }
              />
            </ListItem>
          ))}
        </List>
      )}
    </Paper>
  );
};

export default Support;