import React, { useState, useEffect } from 'react';
import { Box, Paper, Typography, CircularProgress, Alert, Button, Divider } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';

import { useDispatch } from 'react-redux'; // 1. Импортируем useDispatch
import { useAuth } from '../../hooks/useAuth';
import authService from '../../services/authService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';
import LogoutIcon from '@mui/icons-material/Logout'; 
import { logout } from '../../store/slices/authSlice'; 

const MyProfile = () => {
  const { user } = useAuth(); // Берем базовые данные из Redux
  const dispatch = useDispatch(); // 3. Инициализируем dispatch
  // Состояния
  const [isEditMode, setIsEditMode] = useState(false);
  const [formData, setFormData] = useState(null); // Начнем с null
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  // Загружаем полный профиль при монтировании
  useEffect(() => {
    // Ждем, пока из Redux придет роль
    if (user?.role) {
      setLoading(true);
      authService.getMe(user.role)
        .then(profileData => {
          setFormData(profileData);
        })
        .catch(err => {
          setError(handleApiError(err).general);
        })
        .finally(() => {
          setLoading(false);
        });
    } else {
      // Если user еще не загружен в Redux, ждем
      setLoading(true);
    }
  }, [user?.role]); // Зависимость от user.role

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
    if (error && error[e.target.name]) {
      setError(prev => ({ ...prev, [e.target.name]: null }));
    }
  };

  const handleSave = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const updateCommand = {
        firstName: formData.firstName,
        lastName: formData.lastName,
        middleName: formData.middleName,
        email: formData.email,
        phoneNumber: formData.phoneNumber,
      };
      await authService.updateMe(updateCommand, user.role);
      setIsEditMode(false);
      window.location.reload(); // Временное решение для обновления данных
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    setIsEditMode(false);
    // Для отмены просто перезагружаем данные с сервера, чтобы сбросить изменения
    if (user?.role) authService.getMe(user.role).then(setFormData);
    setError(null);
  };
  
  const handleLogout = () => {
    dispatch(logout());
    // Навигация не нужна, так как PrivateRoute сам сделает редирект на /login
  };

  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}><CircularProgress /></Box>;
  if (error) return <Alert severity="error">{error}</Alert>;
  if (!formData) return <Typography>Profile could not be loaded.</Typography>;

  return (
    <Paper sx={{ p: 3, maxWidth: '600px', mx: 'auto' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4">My Profile</Typography>
        {!isEditMode && (
          <Button startIcon={<EditIcon />} onClick={() => setIsEditMode(true)}>
            Edit Profile
          </Button>
        )}
      </Box>
      <Divider sx={{ mb: 3 }} />

      {isEditMode ? (
        // --- РЕЖИМ РЕДАКТИРОВАНИЯ ---
        <Box component="form" onSubmit={handleSave} sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
          <FormInput name="lastName" label="Last Name" value={formData.lastName || ''} onChange={handleChange} error={error?.lastName} required />
          <FormInput name="firstName" label="First Name" value={formData.firstName || ''} onChange={handleChange} error={error?.firstName} required />
          <FormInput name="middleName" label="Middle Name" value={formData.middleName || ''} onChange={handleChange} />
          <FormInput name="email" label="Email" type="email" value={formData.email || ''} onChange={handleChange} error={error?.email} required />
          <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber || ''} onChange={handleChange} error={error?.phoneNumber} />
          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <Button variant="outlined" onClick={handleCancel} disabled={saving}>Cancel</Button>
            <SubmitButton isLoading={saving} startIcon={<SaveIcon />} sx={{ flexGrow: 1 }}>Save Changes</SubmitButton>
          </Box>
        </Box>
      ) : (
        // --- РЕЖИМ ПРОСМОТРА ---
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <Box>
            <Typography variant="overline" color="text.secondary">Last Name</Typography>
            <Typography variant="body1">{formData.lastName}</Typography>
          </Box>
          <Box>
            <Typography variant="overline" color="text.secondary">First Name</Typography>
            <Typography variant="body1">{formData.firstName}</Typography>
          </Box>
          <Box>
            <Typography variant="overline" color="text.secondary">Middle Name</Typography>
            <Typography variant="body1">{formData.middleName || 'Not specified'}</Typography>
          </Box>
          <Box>
            <Typography variant="overline" color="text.secondary">Email</Typography>
            <Typography variant="body1">{formData.email}</Typography>
          </Box>
          <Box>
            <Typography variant="overline" color="text.secondary">Phone Number</Typography>
            <Typography variant="body1">{formData.phoneNumber || 'Not specified'}</Typography>
          </Box>
        </Box>
      )}

            {/* --- 5. НОВАЯ КНОПКА LOGOUT --- */}
      {/* Показываем ее только в режиме просмотра */}
      {!isEditMode && (
        <Box sx={{ mt: 4, pt: 2, borderTop: 1, borderColor: 'divider' }}>
          <Button 
            fullWidth 
            variant="contained" 
            color="error" 
            startIcon={<LogoutIcon />}
            onClick={handleLogout}
          >
            Logout
          </Button>
        </Box>
      )}
    </Paper>
  );
};

export default MyProfile;