import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  DialogContentText, Alert, IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import SubmitButton from '../common/SubmitButton';

/**
 * A dialog component for an administrator to confirm the cancellation of an appointment.
 * @param {object} props
 * @param {boolean} props.open - Whether the dialog is open.
 * @param {function} props.onClose - Function to call when the dialog should close.
 * @param {object|null} props.appointment - The appointment object to be cancelled.
 * @param {function} props.onSuccess - Function to call after a successful cancellation.
 */
const CancelByClinicDialog = ({ open, onClose, appointment, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Reset state when the dialog is closed or a new appointment is selected
  useEffect(() => {
    if (!open) {
      setLoading(false);
      setError(null);
    }
  }, [open]);

  const handleConfirm = async () => {
    if (!appointment) return;
    
    setLoading(true);
    setError(null);
    try {
      // The API expects a command with the appointmentId
      await appointmentService.cancelByClinic({ appointmentId: appointment.id });
      onSuccess(); // Notify the parent component of the success
    } catch (err) {
      setError(handleApiError(err).general || 'Failed to cancel the appointment.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>
        Confirm Cancellation (by Clinic)
        <IconButton
          aria-label="close"
          onClick={onClose}
          sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}
        >
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <DialogContentText>
          Are you sure you want to cancel this appointment? 
          The patient will be notified. This action cannot be undone.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={loading}>
          Back
        </Button>
        <SubmitButton onClick={handleConfirm} isLoading={loading} color="error">
          Confirm Cancellation
        </SubmitButton>
      </DialogActions>
    </Dialog>
  );
};

export default CancelByClinicDialog;