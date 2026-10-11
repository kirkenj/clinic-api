// src/pages/specializations/SpecializationForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, Alert, CircularProgress, Button } from '@mui/material';
import specializationService from '../../services/specializationService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const SpecializationForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;

  const [formData, setFormData] = useState({
    id: 0,
    name: '',
  });

  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isEditMode) {
      setLoading(true);
      specializationService.getById(id)
        .then(data => {
          setFormData(data || { id: Number(id) });
        })
        .catch(err => setError(handleApiError(err)))
        .finally(() => setLoading(false));
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
        await specializationService.update(formData);
      } else {
        await specializationService.create(formData);
      }
      navigate('/specializations');
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
    <Paper sx={{ p: 3, boxShadow: 3, maxWidth: '600px', mx: 'auto' }}>
      <Typography variant="h4" gutterBottom align="center">
        {isEditMode ? 'Edit Specialization' : 'Create Specialization'}
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <Box sx={{ display: 'flex', flexDirection: 'column' }}>
            
          <FormInput 
            name="name" 
            label="Specialization Name" 
            value={formData.name || ''} 
            onChange={handleChange} 
            error={error?.name} 
            required 
          />

          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
              {isEditMode ? 'Save Changes' : 'Create'}
            </SubmitButton>
            <Button 
              variant="outlined" 
              onClick={() => navigate('/specializations')} 
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

export default SpecializationForm;