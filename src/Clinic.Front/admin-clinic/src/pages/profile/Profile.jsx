// src/pages/profile/MyProfile.jsx
import React, { useState, useEffect } from 'react';
import { Box, Paper, Typography, CircularProgress, Alert, Grid, Divider } from '@mui/material';

import authService from '../../services/authService';
import { handleApiError } from '../../utils/handleApiError';
import { useAuth } from '../../hooks/useAuth';

const MyProfile = () => {
  const { role } = useAuth(); // Get the role to call the correct service

  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!role) return; // Don't fetch if role is not yet available

    setLoading(true);
    authService.getMe(role)
      .then(data => {
        setProfile(data);
      })
      .catch(err => {
        setError(handleApiError(err).general);
      })
      .finally(() => {
        setLoading(false);
      });
  }, [role]); // Re-fetch if the role changes (though unlikely in this context)

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>;
  }

  if (error) {
    return <Alert severity="error">{error}</Alert>;
  }

  if (!profile) {
    return <Typography sx={{ p: 4, textAlign: 'center' }}>Could not load profile information.</Typography>;
  }

  return (
    <Paper sx={{ p: 3, maxWidth: '800px', mx: 'auto', boxShadow: 3 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4">My Profile</Typography>
      </Box>
      <Divider />

      <Grid container spacing={3} sx={{ mt: 1 }}>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Last Name</Typography>
          <Typography variant="body1">{profile.lastName || 'N/A'}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">First Name</Typography>
          <Typography variant="body1">{profile.firstName || 'N/A'}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Middle Name</Typography>
          <Typography variant="body1">{profile.middleName || 'Not specified'}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Email</Typography>
          <Typography variant="body1">{profile.email || 'N/A'}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Phone Number</Typography>
          <Typography variant="body1">{profile.phoneNumber || 'Not specified'}</Typography>
        </Grid>
        {profile.specializationId && (
          <Grid item xs={12} sm={6}>
            <Typography variant="overline" color="text.secondary">Specialization ID</Typography>
            <Typography variant="body1">{profile.specializationId}</Typography>
          </Grid>
        )}
        {profile.officeId && (
          <Grid item xs={12} sm={6}>
            <Typography variant="overline" color="text.secondary">Office ID</Typography>
            <Typography variant="body1">{profile.officeId}</Typography>
          </Grid>
        )}
      </Grid>
    </Paper>
  );
};

export default MyProfile;