import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box, Paper, Typography, Alert, CircularProgress, Button,
  Autocomplete, TextField
} from '@mui/material';

import doctorService from '../../services/doctorService';
import specializationService from '../../services/specializationService';
import officeService from '../../services/officeService';
import { handleApiError } from '../../utils/handleApiError';
import useDebounce from '../../hooks/useDebounce';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const DoctorForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditMode = !!id;

  // --- State Management ---
  const [formData, setFormData] = useState({
    id: 0,
    firstName: '',
    lastName: '',
    middleName: '',
    email: '',
    phoneNumber: '',
    specializationId: '',
    officeId: '',
  });

  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);
  const [loading, setLoading] = useState(true);

  // --- State for Autocomplete fields ---
  const [specOptions, setSpecOptions] = useState([]);
  const [officeOptions, setOfficeOptions] = useState([]);
  const [specLoading, setSpecLoading] = useState(false);
  const [officeLoading, setOfficeLoading] = useState(false);
  
  const [specInput, setSpecInput] = useState('');
  const [officeInput, setOfficeInput] = useState('');
  const debouncedSpecInput = useDebounce(specInput, 400);
  const debouncedOfficeInput = useDebounce(officeInput, 400);

  // --- Data Fetching ---

  // Effect to load options for Specialization Autocomplete
  useEffect(() => {
    setSpecLoading(true);
    specializationService.getPaged({ Name: debouncedSpecInput, page: 1, pageSize: 20 })
      .then(response => {
        setSpecOptions(response.items || []);
      })
      .finally(() => setSpecLoading(false));
  }, [debouncedSpecInput]);
  
  // Effect to load options for Office Autocomplete
  useEffect(() => {
    setOfficeLoading(true);
    officeService.getPaged({ Address: debouncedOfficeInput, page: 1, pageSize: 20 })
      .then(response => {
        setOfficeOptions(response.items || []);
      })
      .finally(() => setOfficeLoading(false));
  }, [debouncedOfficeInput]);
  
  // Effect to load doctor data when in edit mode
  useEffect(() => {
    if (isEditMode) {
      setLoading(true);
      doctorService.getById(id)
        .then(data => {
          setFormData(data || {});
          // Pre-populate the dropdowns with the current doctor's specialization and office
          // to ensure they are visible on initial load.
          if (data.specialization) {
            setSpecOptions(prev => [data.specialization, ...prev.filter(o => o.id !== data.specialization.id)]);
          }
          if (data.office) {
            setOfficeOptions(prev => [data.office, ...prev.filter(o => o.id !== data.office.id)]);
          }
        })
        .catch(err => setError(handleApiError(err)))
        .finally(() => setLoading(false));
    } else {
      setLoading(false); // We are in create mode, no data to load
    }
  }, [id, isEditMode]);

  // --- Handlers ---

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };
  
  const handleAutocompleteChange = (name, value) => {
    // `value` is the selected object, or null if cleared
    setFormData(prev => ({ ...prev, [name]: value ? value.id : '' }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      // Ensure IDs are numbers before sending
      const command = { 
        ...formData, 
        specializationId: Number(formData.specializationId), 
        officeId: Number(formData.officeId),
      };

      if (isEditMode) {
        await doctorService.update(command);
      } else {
        await doctorService.create(command);
      }
      navigate('/doctors');
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };
  
  // --- Render ---

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  }
  
 return (
    <Paper sx={{ p: 3, boxShadow: 3, maxWidth: '600px', mx: 'auto' }}> {/* 1. Ограничиваем ширину и центрируем Paper */}
      <Typography variant="h4" gutterBottom align="center"> {/* 2. Центрируем заголовок */}
        {isEditMode ? 'Edit Doctor' : 'Create Doctor'}
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        {/* 3. Оборачиваем все поля в Box с flex-контейнером */}
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
            
          <FormInput name="lastName" label="Last Name" value={formData.lastName || ''} onChange={handleChange} error={error?.lastName} required />
          <FormInput name="firstName" label="First Name" value={formData.firstName || ''} onChange={handleChange} error={error?.firstName} required />
          <FormInput name="middleName" label="Middle Name" value={formData.middleName || ''} onChange={handleChange} error={error?.middleName} />
          <FormInput name="email" label="Email" type="email" value={formData.email || ''} onChange={handleChange} error={error?.email} required />
          <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber || ''} onChange={handleChange} error={error?.phoneNumber} />
          
          <Autocomplete
            options={specOptions}
            getOptionLabel={(option) => option.name || ''}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            value={specOptions.find(o => o.id === formData.specializationId) || null}
            onChange={(event, newValue) => handleAutocompleteChange('specializationId', newValue)}
            onInputChange={(event, newInputValue) => setSpecInput(newInputValue)}
            loading={specLoading}
            renderInput={(params) => (
              <TextField {...params} margin="normal" label="Specialization" required error={!!error?.specializationId} helperText={error?.specializationId} />
            )}
          />
         
          <Autocomplete
            options={officeOptions}
            getOptionLabel={(option) => option.address || ''}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            value={officeOptions.find(o => o.id === formData.officeId) || null}
            onChange={(event, newValue) => handleAutocompleteChange('officeId', newValue)}
            onInputChange={(event, newInputValue) => setOfficeInput(newInputValue)}
            loading={officeLoading}
            renderInput={(params) => (
              <TextField {...params} margin="normal" label="Office" required error={!!error?.officeId} helperText={error?.officeId} />
            )}
          />

          {/* 4. Кнопки оставляем в отдельном Box для гибкости */}
          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
              {isEditMode ? 'Save Changes' : 'Create'}
            </SubmitButton>
            <Button variant="outlined" onClick={() => navigate('/doctors')} disabled={saving} sx={{ flexGrow: 1 }}>
              Cancel
            </Button>
          </Box>
        </Box>
      </Box>
    </Paper>
  );
};

export default DoctorForm;