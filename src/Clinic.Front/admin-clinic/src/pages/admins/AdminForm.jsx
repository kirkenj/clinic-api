// src/pages/admins/AdminForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, Alert, CircularProgress, Button } from '@mui/material';
import adminService from '../../services/adminService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const AdminForm = () => {
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
    adminService.getById(id)
      .then(data => {
        setFormData(data || { id: Number(id) });
      })
      .catch(err => {
        setError(handleApiError(err).general); // Show general error on load failure
      })
      .finally(() => setLoading(false));
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSaving(true);
    try {
      // API for updating a receptionist expects the full object, including the ID
      await adminService.update(formData);
      navigate('/admins');
    } catch (err) {
      setError(handleApiError(err)); // handleApiError can return field-specific errors
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  }

  // Display a single error message if loading failed and we have no form data
  if (error && !formData.id) {
     return <Alert severity="error">{error.general || "Failed to load administrator data."}</Alert>;
  }

  return (
    <Paper sx={{ p: 3, boxShadow: 3 }}>
      <Typography variant="h4" gutterBottom>
        Edit Administrator Profile
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {/* Show a general error if it exists */}
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <FormInput name="lastName" label="Last Name" value={formData.lastName || ''} onChange={handleChange} error={error?.lastName} required />
        <FormInput name="firstName" label="First Name" value={formData.firstName || ''} onChange={handleChange} error={error?.firstName} required />
        <FormInput name="middleName" label="Middle Name" value={formData.middleName || ''} onChange={handleChange} error={error?.middleName} />
        <FormInput name="email" label="Email" type="email" value={formData.email || ''} onChange={handleChange} error={error?.email} required />
        <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber || ''} onChange={handleChange} error={error?.phoneNumber} />
        
        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
            Save Changes
          </SubmitButton>
          <Button variant="outlined" onClick={() => navigate('/admins')} disabled={saving} sx={{ flexGrow: 1 }}>
            Cancel
          </Button>
        </Box>
      </Box>
    </Paper>
  );
};

export default AdminForm;