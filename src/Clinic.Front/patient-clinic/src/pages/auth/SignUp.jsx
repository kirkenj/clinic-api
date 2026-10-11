import React, { useState, useEffect } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import {
  Box, Container, Paper, Typography, Alert,
  Button, Link as MuiLink, Stepper, Step, StepLabel,
  IconButton
} from '@mui/material';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutline';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

import authService from '../../services/authService';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const steps = ['Your Details', 'Confirm Email'];

const SignUp = () => {
  const navigate = useNavigate();

  const [activeStep, setActiveStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    middleName: '',
    email: '',
    phoneNumber: '',
    otp: '',
  });
  
  const [resendCooldown, setResendCooldown] = useState(0);

  useEffect(() => {
    let timer;
    if (resendCooldown > 0) {
      timer = setTimeout(() => setResendCooldown(resendCooldown - 1), 1000);
    }
    return () => clearTimeout(timer);
  }, [resendCooldown]);

  const handleChange = (e) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
    if (errors[e.target.name]) {
      setErrors(prev => ({ ...prev, [e.target.name]: null }));
    }
  };
  const handleOtpChange = (e) => {
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
    setFormData(prev => ({...prev, otp: value}));
    if (errors.otp) setErrors(prev => ({ ...prev, otp: null }));
  };

  const handleRequestOtp = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrors({});
    try {
      const { firstName, lastName, middleName, email, phoneNumber } = formData;
      await authService.signUp({ firstName, lastName, middleName, email, phoneNumber });
      setActiveStep(1);
      setResendCooldown(60);
    } catch (err) {
      setErrors(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };
  
  const handleConfirmSignUp = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrors({});
    try {
      const { email, otp } = formData;
      await authService.confirmSignUp({ email, otp });
      setActiveStep(2);
    } catch (err) {
      setErrors(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  const handleResendOtp = async () => {
    if (resendCooldown > 0) return;
    setLoading(true);
    setErrors({});
    try {
      await authService.resendSignUp({email:formData.email});
      setResendCooldown(60);
    } catch (err) {
      setErrors(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  const handleBack = () => {
    setActiveStep(prev => prev - 1);
    setErrors({});
  };

  const handleSkipToOtp = () => {
    if (!formData.email || !/^\S+@\S+\.\S+$/.test(formData.email)) {
      setErrors({ email: 'Please enter a valid email to proceed.' });
      return;
    }
    setErrors({});
    setActiveStep(1);
  };
  
  const renderStepContent = () => {
    switch (activeStep) {
      case 0:
        return (
          <Box component="form" onSubmit={handleRequestOtp} noValidate>
            <FormInput name="lastName" label="Last Name" value={formData.lastName} onChange={handleChange} error={errors?.lastName} required />
            <FormInput name="firstName" label="First Name" value={formData.firstName} onChange={handleChange} error={errors?.firstName} required />
            <FormInput name="middleName" label="Middle Name" value={formData.middleName} onChange={handleChange} />
            <FormInput name="email" label="Email" type="email" value={formData.email} onChange={handleChange} error={errors?.email} required />
            <FormInput name="phoneNumber" label="Phone Number" value={formData.phoneNumber} onChange={handleChange} error={errors?.phoneNumber} required />
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
              <Button variant="text" size="small" onClick={handleSkipToOtp} disabled={loading} sx={{ textTransform: 'none' }}>
                Already requested code?
              </Button>
              <SubmitButton isLoading={loading} sx={{ m: 0, width: 'auto' }}>Get Confirmation Code</SubmitButton>
            </Box>
          </Box>
        );
      case 1:
        return (
          // --- ОБНОВЛЕННЫЙ JSX ДЛЯ ВТОРОГО ШАГА ---
          <Box component="form" onSubmit={handleConfirmSignUp} noValidate>
            {/* 1. Стрелка "Назад" теперь абсолютно спозиционирована */}
            <IconButton onClick={handleBack} disabled={loading} aria-label="go back" sx={{ position: 'absolute', top: 16, left: 16 }}>
              <ArrowBackIcon />
            </IconButton>
            
            <Typography sx={{ mb: 2 }}>
              A code has been sent to <strong>{formData.email}</strong>. Please enter it to complete your registration.
            </Typography>
            
            <FormInput name="otp" label="Confirmation Code" value={formData.otp} onChange={handleOtpChange} error={errors?.otp} required />
            
            <Box sx={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', mt: 2 }}>
              {resendCooldown > 0 ? (
                <Typography variant="body2" color="text.secondary" sx={{ mr: 'auto' }}>
                  Resend code in {resendCooldown}s
                </Typography>
              ) : (
                // 2. Кнопка "Resend" теперь слева
                <Button variant="text" size="small" onClick={handleResendOtp} disabled={loading} sx={{ textTransform: 'none', mr: 'auto' }}>
                  Resend Code
                </Button>
              )}
              
              {/* 3. Кнопка "Confirm" остается справа */}
              <SubmitButton isLoading={loading} sx={{ m: 0, width: 'auto' }}>
                Confirm & Sign Up
              </SubmitButton>
            </Box>
          </Box>
        );
      case 2:
        return (
          <Box sx={{ textAlign: 'center', py: 4 }}>
            <CheckCircleOutlineIcon color="success" sx={{ fontSize: 60, mb: 2 }} />
            <Typography variant="h5" gutterBottom>Registration Successful!</Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              You can now sign in using your email.
            </Typography>
            <Button variant="contained" onClick={() => navigate('/login')}>
              Go to Sign In page
            </Button>
          </Box>
        );
      default:
        return null;
    }
  };

  return (
      <Container maxWidth="sm" sx={{ py: 4 }}>
      <Paper sx={{ p: { xs: 2, sm: 4 }, position: 'relative' }}>
        <Typography variant="h4" align="center" gutterBottom>Create an Account</Typography>
        
        {activeStep < 2 && (
          <Stepper activeStep={activeStep} sx={{ my: 3 }}>
            {steps.map((label) => <Step key={label}><StepLabel>{label}</StepLabel></Step>)}
          </Stepper>
        )}

        {errors?.general && <Alert severity="error" sx={{ mb: 2 }}>{errors.general}</Alert>}

        {renderStepContent()}
        
        {activeStep < 2 && (
          
          <Typography align="center" sx={{ mt: 3 }}>
            Already have an account? <MuiLink component={RouterLink} to="/login">Sign In</MuiLink>
          </Typography>
        )}
      </Paper>
    </Container>
  );
};

export default SignUp;