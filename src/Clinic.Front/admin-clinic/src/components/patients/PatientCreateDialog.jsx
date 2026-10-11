import React, { useState, useEffect } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions, Button,
  Box, Stepper, Step, StepLabel, Typography, Alert, Link as MuiLink,
  IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import patientService from '../../services/patientService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../common/FormInput';
import SubmitButton from '../common/SubmitButton';

const steps = ['Patient Details', 'Confirm OTP'];
const initialFormData = {
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
  phoneNumber: '',
};

const PatientCreateDialog = ({ open, onClose }) => {
  const [activeStep, setActiveStep] = useState(0);
  const [formData, setFormData] = useState(initialFormData);
  const [otp, setOtp] = useState('');
  
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [isOtpStepOnly, setIsOtpStepOnly] = useState(false);
  const [resendCooldown, setResendCooldown] = useState(0);

  // Effect for the cooldown timer
  useEffect(() => {
    let timer;
    if (resendCooldown > 0) {
      timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
    }
    return () => clearTimeout(timer);
  }, [resendCooldown]);
  
  // Reset the entire dialog state when it's closed
  useEffect(() => {
    if (!open) {
      // Delay reset to allow for closing animation
      setTimeout(() => {
        setActiveStep(0);
        setFormData(initialFormData);
        setOtp('');
        setError(null);
        setIsOtpStepOnly(false);
        setResendCooldown(0);
      }, 300);
    }
  }, [open]);

  const handleChange = (e) => {
    if (error && error[e.target.name]) {
        setError(prev => ({ ...prev, [e.target.name]: null }));
    }
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleOtpChange = (e) => {
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
    if (error && error.otp) {
        setError(prev => ({ ...prev, otp: null }));
    }
    setOtp(value);
  };

  const handleNext = async () => {
    setError(null);
    setLoading(true);
    try {
      if (activeStep === 0 && !isOtpStepOnly) {
        // Step 1: Send patient details to get OTP
        await patientService.register(formData);
      }
      if (activeStep === 1) {
        // Step 2: Confirm registration with OTP
        await patientService.confirmRegistration({ email: formData.email, otp });
        onClose(true); // Close dialog on success
        return;
      }
      setActiveStep(1);
      setResendCooldown(60); // Start the cooldown timer
    } catch (err) {
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  const handleBack = () => {
    setActiveStep(prev => prev - 1);
    setIsOtpStepOnly(false);
    setError(null);
  };
  
  const handleSkipToOtp = () => {
    if (!formData.email || !/^\S+@\S+\.\S+$/.test(formData.email)) {
      setError({ email: 'To skip, please enter a valid patient email first.' });
      return;
    }
    setError(null);
    setIsOtpStepOnly(true);
    setActiveStep(1);
    handleResendOtp(); // Trigger resend logic immediately
  };

  const handleResendOtp = async () => {
    if (resendCooldown > 0) return;
    setError(null);
    try {
      // Logic for resending OTP
      await patientService.resendSignUpOtp(formData.email);
      setResendCooldown(60); // Reset the timer
    } catch (err) {
      setError(handleApiError(err));
    }
  };

  return (
    <Dialog open={open} onClose={() => onClose(false)} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ textAlign: 'center', m: 0, p: 2 }}>
        Register New Patient
        <IconButton
          aria-label="close"
          onClick={() => onClose(false)}
          sx={{
            position: 'absolute',
            right: 8,
            top: 8,
            color: (theme) => theme.palette.grey[500],
          }}
        >
          <CloseIcon />
        </IconButton>
      </DialogTitle>
      <DialogContent>
        <Stepper activeStep={activeStep} sx={{ my: 3 }}>
          {steps.map((label) => <Step key={label}><StepLabel>{label}</StepLabel></Step>)}
        </Stepper>

        {error && error.general && <Alert severity="error" sx={{ mb: 2 }}>{error.general}</Alert>}

        {/* Step 1: Patient Details Form */}
        <Box sx={{ display: activeStep === 0 ? 'block' : 'none', mt: 2 }}>
          <FormInput name="lastName" label="Last Name" value={formData.lastName} onChange={handleChange} error={error?.lastName} required />
          <FormInput name="firstName" label="First Name" value={formData.firstName} onChange={handleChange} error={error?.firstName} required />
          <FormInput name="middleName" label="Middle Name" value={formData.middleName} onChange={handleChange} />
          <FormInput name="email" label="Email" type="email" value={formData.email} onChange={handleChange} error={error?.email} required />
          <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber} onChange={handleChange} error={error?.phoneNumber} required />
        </Box>

        {/* Step 2: OTP Confirmation Form */}
        <Box sx={{ display: activeStep === 1 ? 'block' : 'none', mt: 2 }}>
          <Typography sx={{ mb: 2 }}>
            An OTP has been sent to the patient's email: <strong>{formData.email}</strong>.
            Please enter the code to complete the registration.
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
      <DialogActions sx={{ p: '16px 24px', justifyContent: 'space-between' }}>
        {/* Left Action: Back button or Skip link */}
        <Box>
          {activeStep === 1 ? (
            <Button onClick={handleBack} disabled={loading}>Back</Button>
          ) : (
            <MuiLink component="button" variant="body2" onClick={handleSkipToOtp} type="button" disabled={loading}>
              Patient already registered?
            </MuiLink>
          )}
        </Box>

        {/* Right Action: Next/Finish button */}
        <Box>
          <SubmitButton onClick={handleNext} isLoading={loading} sx={{ m: 0 }}>
            {activeStep === steps.length - 1 ? 'Finish Registration' : 'Next'}
          </SubmitButton>
        </Box>
      </DialogActions>
    </Dialog>
  );
};

export default PatientCreateDialog;