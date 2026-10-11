// src/pages/services/ServiceForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Paper, Typography, Alert, CircularProgress, Button,
  Autocomplete, TextField
} from '@mui/material';
import serviceService from '../../services/serviceService';
import specializationService from '../../services/specializationService';
import { handleApiError } from '../../utils/handleApiError';
import useDebounce from '../../hooks/useDebounce';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const ServiceForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;

  const [formData, setFormData] = useState({
    id: 0,
    name: '',
    price: 0,
    specializationId: '',
  });

  const [specOptions, setSpecOptions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  const [specInput, setSpecInput] = useState('');
  const debouncedSpecInput = useDebounce(specInput, 400);

  useEffect(() => {
    specializationService.getPaged({ Name: debouncedSpecInput, page: 1, pageSize: 20 })
      .then(response => setSpecOptions(response.items || []));
  }, [debouncedSpecInput]);

  useEffect(() => {
    if (isEditMode) {
      setLoading(true);
      serviceService.getById(id)
        .then(data => {
          setFormData(data || {});
          if (data.specialization) { // Если бэкенд вернет вложенный объект
            setSpecOptions(prev => [data.specialization, ...prev.filter(o => o.id !== data.specialization.id)]);
          }
        })
        .catch(err => setError(handleApiError(err)))
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, [id, isEditMode]);

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };
  
  const handleAutocompleteChange = (name, value) => {
    setFormData(prev => ({ ...prev, [name]: value ? value.id : '' }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      const command = { ...formData, price: Number(formData.price), specializationId: Number(formData.specializationId) };
      if (isEditMode) {
        await serviceService.update(command);
      } else {
        await serviceService.create(command);
      }
      navigate('/services');
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  }
  
  return (
    <Paper sx={{ p: 3, boxShadow: 3, maxWidth: '600px', mx: 'auto' }}>
      <Typography variant="h4" gutterBottom align="center">
        {isEditMode ? 'Edit Service' : 'Create Service'}
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <Box sx={{ display: 'flex', flexDirection: 'column' }}>
          <FormInput name="name" label="Service Name" value={formData.name || ''} onChange={handleChange} error={error?.name} required />
          <FormInput name="price" label="Price" type="number" value={formData.price || 0} onChange={handleChange} error={error?.price} required />
          
          <Autocomplete
            options={specOptions}
            getOptionLabel={(option) => option.name || ''}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            value={specOptions.find(o => o.id === formData.specializationId) || null}
            onChange={(event, newValue) => handleAutocompleteChange('specializationId', newValue)}
            onInputChange={(event, newInputValue) => setSpecInput(newInputValue)}
            loading={loading}
            sx={{ mt: 2 }}
            renderInput={(params) => (
              <TextField {...params} label="Specialization" required error={!!error?.specializationId} helperText={error?.specializationId} />
            )}
          />

          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
              {isEditMode ? 'Save Changes' : 'Create'}
            </SubmitButton>
            <Button variant="outlined" onClick={() => navigate('/services')} disabled={saving} sx={{ flexGrow: 1 }}>
              Cancel
            </Button>
          </Box>
        </Box>
      </Box>
    </Paper>
  );
};

export default ServiceForm;