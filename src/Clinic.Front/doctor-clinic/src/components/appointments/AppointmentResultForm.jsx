// src/components/appointments/AppointmentResultForm.jsx
import React, { useState, useEffect } from 'react';
import { Box, Paper, Typography, Alert, Button } from '@mui/material';

import appointmentResultService from '../../services/appointmentResultService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../common/FormInput';
import SubmitButton from '../common/SubmitButton';

/**
 * A form component to add or edit an appointment result.
 * @param {object} props
 * @param {number} props.appointmentId - The ID of the parent appointment.
 * @param {object|null} props.existingResult - The existing result object if editing, or null if creating.
 * @param {function} props.onSuccess - Callback function to execute on successful save.
 * @param {function} props.onCancel - Callback function to cancel editing.
 */
const AppointmentResultForm = ({ appointmentId, existingResult, onSuccess, onCancel }) => {
  const [formData, setFormData] = useState({
    appointmentId: Number(appointmentId),
    complaints: '',
    conclusion: '',
    recommendations: '',
  });

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  const isEditMode = !!existingResult?.id;

  // Заполняем форму данными, если мы в режиме редактирования
  useEffect(() => {
    if (existingResult) {
      setFormData(existingResult);
    } else {
      // Если создаем новый, убедимся что ID правильный
      setFormData({
        appointmentId: Number(appointmentId),
        complaints: '',
        conclusion: '',
        recommendations: '',
      });
    }
  }, [existingResult, appointmentId]);

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      if (isEditMode) {
        await appointmentResultService.update(formData);
      } else {
        await appointmentResultService.create(formData);
      }
      onSuccess(); // Сообщаем родителю об успехе
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <Paper sx={{ p: 3, mt: 3, border: '1px solid', borderColor: 'divider' }}>
      <Typography variant="h6" gutterBottom>
        {isEditMode ? 'Edit Result' : 'Add New Result'}
      </Typography>
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error?.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <FormInput name="complaints" label="Complaints" value={formData.complaints || ''} onChange={handleChange} error={error?.complaints} multiline rows={3} />
        <FormInput name="conclusion" label="Conclusion / Diagnosis" value={formData.conclusion || ''} onChange={handleChange} error={error?.conclusion} multiline rows={3} />
        <FormInput name="recommendations" label="Recommendations" value={formData.recommendations || ''} onChange={handleChange} error={error?.recommendations} multiline rows={3} />
        
        <Box sx={{ mt: 2, display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
          <Button onClick={onCancel} disabled={saving}>Cancel</Button>
          <SubmitButton isLoading={saving}>
            {isEditMode ? 'Save Result' : 'Add Result'}
          </SubmitButton>
        </Box>
      </Box>
    </Paper>
  );
};

export default AppointmentResultForm;