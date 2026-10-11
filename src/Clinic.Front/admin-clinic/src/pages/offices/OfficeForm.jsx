// src/pages/offices/OfficeForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, Alert, CircularProgress, Button } from '@mui/material';
import officeService from '../../services/officeService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const OfficeForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;

  const [formData, setFormData] = useState({
    id: 0,
    address: '',
    phoneNumber: '',
  });

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isEditMode) {
      setLoading(true);
      officeService.getById(id)
        .then(data => {
          setFormData(data || { id: Number(id) });
        })
        .catch(err => setError(handleApiError(err)))
        .finally(() => setLoading(false));
    } else {
      setLoading(false); // No need to load in create mode
    }
  }, [id, isEditMode]);

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      if (isEditMode) {
        await officeService.update(formData);
      } else {
        await officeService.create(formData);
      }
      navigate('/offices');
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };

  if (loading && isEditMode) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  }

  return (
    // 1. Ограничиваем ширину и центрируем контейнер
    <Paper sx={{ p: 3, boxShadow: 3, maxWidth: '600px', mx: 'auto' }}>
      <Typography variant="h4" gutterBottom align="center">
        {isEditMode ? 'Edit Office' : 'Create Office'}
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        {/* 2. Оборачиваем поля в Box для вертикального расположения */}
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
            
          <FormInput 
            name="address" 
            label="Address" 
            value={formData.address || ''} 
            onChange={handleChange} 
            error={error?.address} 
            required 
          />
          <FormInput 
            name="phoneNumber" 
            label="Phone Number" 
            value={formData.phoneNumber || ''} 
            onChange={handleChange} 
            error={error?.phoneNumber}
          />

          {/* 3. Кнопки в отдельном контейнере */}
          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
              {isEditMode ? 'Save Changes' : 'Create'}
            </SubmitButton>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/offices')} 
              disabled={saving} 
              sx={{ flexGrow: 1 }}
            >
              Cancel
            </Button>
          </Box>
        </Box>
      </Box>
    </Paper>
  );
};

export default OfficeForm;