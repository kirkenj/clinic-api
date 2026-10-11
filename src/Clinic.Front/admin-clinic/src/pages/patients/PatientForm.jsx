// src/pages/patients/PatientForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, Alert, CircularProgress, Button } from '@mui/material';
import patientService from '../../services/patientService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const PatientForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    id: 0,
    firstName: '',
    lastName: '',
    middleName: '',
    email: '',
    phoneNumber: '',
  });

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    setLoading(true);
    patientService.getById(id)
      .then(data => {
        setFormData(data || { id: Number(id) });
      })
      .catch(err => setError(handleApiError(err)))
      .finally(() => setLoading(false));
  }, [id]);

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      await patientService.update(formData);
      navigate('/patients');
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
        Edit Patient Profile
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
          <FormInput name="lastName" label="Last Name" value={formData.lastName || ''} onChange={handleChange} error={error?.lastName} required />
          <FormInput name="firstName" label="First Name" value={formData.firstName || ''} onChange={handleChange} error={error?.firstName} required />
          <FormInput name="middleName" label="Middle Name" value={formData.middleName || ''} onChange={handleChange} error={error?.middleName} />
          <FormInput name="email" label="Email" type="email" value={formData.email || ''} onChange={handleChange} error={error?.email} required />
          <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber || ''} onChange={handleChange} error={error?.phoneNumber} />

          <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
            <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
              Save Changes
            </SubmitButton>
            <Button variant="outlined" onClick={() => navigate('/patients')} disabled={saving} sx={{ flexGrow: 1 }}>
              Cancel
            </Button>
          </Box>
        </Box>
      </Box>
    </Paper>
  );
};
export default PatientForm;