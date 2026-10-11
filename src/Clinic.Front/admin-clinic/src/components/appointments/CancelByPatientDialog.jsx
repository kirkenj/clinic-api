// src/components/appointments/CancelByPatientDialog.jsx
import React, { useState, useEffect } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Box, Typography, Alert, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../common/FormInput';
import SubmitButton from '../common/SubmitButton';

const CancelByPatientDialog = ({ open, onClose, appointment, onCancelSuccess }) => {
  const [step, setStep] = useState(1);
  const [otp, setOtp] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (open) {
      setStep(1);
      setOtp('');
      setError(null);
    }
  }, [open, appointment]);

  const handleInitialCancelRequest = async () => {
    if (!appointment) return;
    setLoading(true);
    setError(null);
    try {
      const responseMessage = await appointmentService.cancelByPatient({ appointmentId: appointment.id });
      if (responseMessage && responseMessage.toLowerCase().includes('check email')) {
        setStep(2);
      } else {
        onCancelSuccess();
      }
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };
  
  const handleConfirmWithOtp = async () => {
    if (!appointment) return;
    setLoading(true);
    setError(null);
    try {
      await appointmentService.approveCancellationByPatient({ appointmentId: appointment.id, otp });
      onCancelSuccess();
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };
  
  const handleOtpChange = (e) => {
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
    setOtp(value);
    if (error?.otp) setError(prev => ({ ...prev, otp: null }));
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="xs" fullWidth>
      <DialogTitle>
        Cancel Appointment (as Patient)
        <IconButton aria-label="close" onClick={onClose} sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}

        {step === 1 && <Typography>This action will initiate the cancellation process as if requested by the patient. If the appointment is confirmed, an OTP will be required. Continue?</Typography>}
        
        {step === 2 && (
          <Box>
            <Typography sx={{ mb: 2 }}>An OTP is required to cancel this confirmed appointment. Please ask the patient for the code sent to their email.</Typography>
            <FormInput name="otp" label="Cancellation Code" value={otp} onChange={handleOtpChange} error={error?.otp} required />
          </Box>
        )}
      </DialogContent>
      <DialogActions>
        {step === 1 && (
          <>
            <Button onClick={onClose}>Back</Button>
            <SubmitButton onClick={handleInitialCancelRequest} isLoading={loading} color="warning">Request Cancellation</SubmitButton>
          </>
        )}
        {step === 2 && (
          <>
            <Button onClick={handleInitialCancelRequest} disabled={loading}>Resend Code</Button>
            <SubmitButton onClick={handleConfirmWithOtp} isLoading={loading} color="error">Confirm Cancellation</SubmitButton>
          </>
        )}
      </DialogActions>
    </Dialog>
  );
};

export default CancelByPatientDialog;