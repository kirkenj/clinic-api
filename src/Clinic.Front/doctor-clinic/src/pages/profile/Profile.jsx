// src/pages/profile/MyProfile.jsx
import React, { useState, useEffect, useCallback } from 'react';
import { Box, Paper, Typography, CircularProgress, Alert, Grid, Divider, Avatar } from '@mui/material';
import PersonIcon from '@mui/icons-material/Person';

import { useAuth } from '../../hooks/useAuth';
import authService from '../../services/authService';
import specializationService from '../../services/specializationService';
import officeService from '../../services/officeService';
import { handleApiError } from '../../utils/handleApiError';

const MyProfile = () => {
  const { user } = useAuth(); // Получаем базовые данные о пользователе из Redux

  // Состояния для хранения полных данных
  const [profile, setProfile] = useState(null);
  const [specialization, setSpecialization] = useState(null);
  const [office, setOffice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Загрузка всех данных при монтировании
  const fetchData = useCallback(async () => {
    // Ждем, пока из Redux придет базовый объект user с ролью
    if (!user?.role) return;

    setLoading(true);
    setError(null);
    try {
      // Загружаем полный профиль
      const profileData = await authService.getMe(user.role);
      setProfile(profileData);

      if (profileData) {
        // Параллельно загружаем связанные сущности
        const [specData, officeData] = await Promise.all([
          profileData.specializationId ? specializationService.getById(profileData.specializationId) : Promise.resolve(null),
          profileData.officeId ? officeService.getById(profileData.officeId) : Promise.resolve(null)
        ]);
        setSpecialization(specData);
        setOffice(officeData);
      }
    } catch (err) {
      setError(handleApiError(err).general);
    } finally {
      setLoading(false);
    }
  }, [user?.role]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);
  
  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>;
  if (error) return <Alert severity="error">{error}</Alert>;
  if (!profile) return <Typography sx={{ p: 4, textAlign: 'center' }}>Profile not found.</Typography>;

  return (
    <Paper sx={{ p: { xs: 2, sm: 3 }, maxWidth: '700px', mx: 'auto' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
        <Avatar sx={{ width: 64, height: 64, bgcolor: 'primary.main' }}>
          <PersonIcon sx={{ fontSize: 40 }} />
        </Avatar>
        <Box>
          <Typography variant="h4">{profile.firstName} {profile.lastName}</Typography>
          <Typography variant="h6" color="text.secondary">{specialization?.name || '...'}</Typography>
        </Box>
      </Box>
      <Divider sx={{ my: 2 }} />

      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Typography variant="overline" color="text.secondary">Middle Name</Typography>
          <Typography variant="body1">{profile.middleName || 'Not specified'}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Email</Typography>
          <Typography variant="body1">{profile.email}</Typography>
        </Grid>
        <Grid item xs={12} sm={6}>
          <Typography variant="overline" color="text.secondary">Phone Number</Typography>
          <Typography variant="body1">{profile.phoneNumber || 'Not specified'}</Typography>
        </Grid>
        <Grid item xs={12}>
          <Typography variant="overline" color="text.secondary">Office</Typography>
          <Typography variant="body1">{office?.address || '...'}</Typography>
        </Grid>
      </Grid>
      
      {/* 
        Здесь можно добавить кнопку, которая ведет, например, в Help Desk 
        или создает тикет для администратора с просьбой изменить данные.
        <Box sx={{ mt: 3, textAlign: 'right' }}>
          <Button>Request Profile Change</Button>
        </Box> 
      */}
    </Paper>
  );
};

export default MyProfile;