// src/components/appointments/ApproveByPatientDialog.jsx
import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Box, Typography, Alert, Link as MuiLink, IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../common/FormInput';
import SubmitButton from '../common/SubmitButton';

const ApproveByPatientDialog = ({ open, onClose, appointment }) => {
  const [otp, setOtp] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [resendCooldown, setResendCooldown] = useState(0);

  // Cooldown timer effect
  useEffect(() => {
    let timer;
    if (resendCooldown > 0) {
      timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
    }
    return () => clearTimeout(timer);
  }, [resendCooldown]);

  // Reset state when dialog opens/closes
  useEffect(() => {
    if (!open) {
      setTimeout(() => {
        setOtp('');
        setError(null);
        setResendCooldown(0);
      }, 300);
    } else {
      // Start cooldown timer when dialog opens
      setResendCooldown(60); 
    }
  }, [open]);

  const handleOtpChange = (e) => {
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
    if (error && error.otp) setError(prev => ({ ...prev, otp: null }));
    setOtp(value);
  };

  const handleResendOtp = async () => {
    if (resendCooldown > 0 || !appointment) return;
    setError(null);
    try {
      await appointmentService.sendOtpToConfirmByPatient({ appointmentId: appointment.id });
      setResendCooldown(60); // Reset timer
    } catch (err) {
      setError(handleApiError(err));
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!appointment) return;
    setLoading(true);
    setError(null);
    try {
      const command = {
        patientId: appointment.patientId,
        appointmentId: appointment.id,
        otp,
      };
      await appointmentService.approveByPatient(command);
      onClose(true); // Close and signal success
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onClose={() => onClose(false)} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ textAlign: 'center', m: 0, p: 2 }}>
        Approve Appointment
        <IconButton
          aria-label="close"
          onClick={() => onClose(false)}
          sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}
        >
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 2 }}>
          {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}
          <Typography sx={{ mb: 2 }}>
            An OTP has been sent to the patient's email to confirm this appointment.
          </Typography>
          <FormInput name="otp" label="Confirmation Code" value={otp} onChange={handleOtpChange} error={error?.otp} required />
          <Box sx={{ mt: 1, textAlign: 'right' }}>
            {resendCooldown > 0 ? (
              <Typography variant="body2" color="text.secondary">
                Resend code in {resendCooldown}s
              </Typography>
            ) : (
              <MuiLink component="button" variant="body2" onClick={handleResendOtp} type="button">
                Resend Code
              </MuiLink>
            )}
          </Box>
        </Box>
      </DialogContent>
      <DialogActions sx={{ p: '16px 24px' }}>
        <SubmitButton onClick={handleSubmit} isLoading={loading} sx={{ m: 0, width: '100%' }}>
          Confirm
        </SubmitButton>
      </DialogActions>
    </Dialog>
  );
};

export default ApproveByPatientDialog;