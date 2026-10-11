// src/pages/appointments/AppointmentResultForm.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Box, Paper, Typography, Alert, CircularProgress, Button } from '@mui/material';

import appointmentResultService from '../../services/appointmentResultService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const AppointmentResultForm = () => {
  const { appointmentId } = useParams(); // ID записи, к которой привязан результат
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    appointmentId: Number(appointmentId),
    complaints: '',
    conclusion: '',
    recommendations: '',
  });
  
  const [isEditMode, setIsEditMode] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);

  // Загружаем существующий результат, если он есть
  useEffect(() => {
    setLoading(true);
    appointmentResultService.getByAppointmentId(appointmentId)
      .then(data => {
        if (data) {
          setFormData(data);
          setIsEditMode(true);
        }
      })
      .catch(err => {
        const apiError = handleApiError(err);
        if (apiError.code !== 'AppointmentResult.NotFoundWithAppointmentId') {
          setError(apiError); // Показываем только непредвиденные ошибки
        }
      })
      .finally(() => setLoading(false));
  }, [appointmentId]);

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
      navigate(`/appointments/${appointmentId}`); // Возвращаемся на детальную страницу
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setSaving(false);
    }
  };
  
  if (loading) return <CircularProgress />;

  return (
    <Paper sx={{ p: 3, maxWidth: '800px', mx: 'auto' }}>
      <Typography variant="h4" gutterBottom align="center">
        {isEditMode ? 'Edit Appointment Result' : 'Add Appointment Result'}
      </Typography>
      <Typography align="center" color="text.secondary" sx={{ mb: 2 }}>
        For Appointment ID: {appointmentId}
      </Typography>
      
      <Box component="form" onSubmit={handleSubmit} noValidate>
        {error?.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
        
        <FormInput
          name="complaints"
          label="Complaints"
          value={formData.complaints || ''}
          onChange={handleChange}
          error={error?.complaints}
          multiline
          rows={4}
        />
        <FormInput
          name="conclusion"
          label="Conclusion / Diagnosis"
          value={formData.conclusion || ''}
          onChange={handleChange}
          error={error?.conclusion}
          multiline
          rows={4}
        />
        <FormInput
          name="recommendations"
          label="Recommendations"
          value={formData.recommendations || ''}
          onChange={handleChange}
          error={error?.recommendations}
          multiline
          rows={4}
        />
        
        <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
          <SubmitButton isLoading={saving} sx={{ flexGrow: 1 }}>
            {isEditMode ? 'Save Result' : 'Add Result'}
          </SubmitButton>
          <Button
            variant="outlined"
            onClick={() => navigate(`/appointments/${appointmentId}`)}
            disabled={saving}
            sx={{ flexGrow: 1 }}
          >
            Cancel
          </Button>
        </Box>
      </Box>
    </Paper>
  );
};

export default AppointmentResultForm;