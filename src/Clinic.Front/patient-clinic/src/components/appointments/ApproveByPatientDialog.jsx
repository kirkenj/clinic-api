// src/components/appointments/ApproveByPatientDialog.jsx
import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, Typography, Alert, IconButton, Link as MuiLink
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import appointmentService from '../../services/appointmentService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../common/FormInput';
import SubmitButton from '../common/SubmitButton';

const ApproveByPatientDialog = ({ open, onClose, appointment, onSuccess }) => {
  const [otp, setOtp] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [resendCooldown, setResendCooldown] = useState(0);

  useEffect(() => {
    if (open) {
      setOtp('');
      setError(null);
    }
  }, [open, appointment]);
  
  useEffect(() => {
    let timer;
    if (resendCooldown > 0) {
      timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
    }
    return () => clearTimeout(timer);
  }, [resendCooldown]);


  const handleSendOtp = async () => {
    if (!appointment || resendCooldown > 0) return;
    setLoading(true);
    setError(null);
    try {
      await appointmentService.sendOtpToConfirm({ appointmentId: appointment.id });
      setResendCooldown(60);
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  const handleConfirm = async (e) => {
    e.preventDefault();
    if (!appointment) return;
    setLoading(true);
    setError(null);
    try {
      await appointmentService.approveByPatient({ appointmentId: appointment.id, otp });
      onSuccess(true);
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
    <Dialog open={open} onClose={() => onClose(false)} maxWidth="xs" fullWidth>
      <DialogTitle>
        Confirm Appointment
        <IconButton aria-label="close" onClick={() => onClose(false)} sx={{ position: 'absolute', right: 8, top: 8, color: (theme) => theme.palette.grey[500] }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent component="form" onSubmit={handleConfirm}>
        <Typography sx={{ mt: 2, mb: 2 }}>
          To confirm your appointment, please enter the code sent to your email.
        </Typography>
        
        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}

        <FormInput name="otp" label="Confirmation Code" value={otp} onChange={handleOtpChange} error={error?.otp} required />
        
        <Box sx={{ mt: 1, display: 'flex', justifyContent: 'flex-end' }}>
          {resendCooldown > 0 ? (
            <Typography variant="body2" color="text.secondary">
              Resend code in {resendCooldown}s
            </Typography>
          ) : (
            <MuiLink component="button" variant="body2" onClick={handleSendOtp} type="button" disabled={loading}>
              Send/Resend Code
            </MuiLink>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={() => onClose(false)} disabled={loading}>Cancel</Button>
        <SubmitButton onClick={handleConfirm} isLoading={loading}>
          Confirm Appointment
        </SubmitButton>
      </DialogActions>
    </Dialog>
  );
};

export default ApproveByPatientDialog;