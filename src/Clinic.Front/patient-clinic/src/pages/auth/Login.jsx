import React, { useState, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { useNavigate, useLocation, Link as RouterLink } from 'react-router-dom';
import {
  Box, Container, Paper, Typography, Alert, Link as MuiLink,
  Button, IconButton
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';

import { verifyOtpAndLogin } from '../../store/slices/authSlice';
import authService from '../../services/authService';
import { useAuth } from '../../hooks/useAuth';
import { handleApiError } from '../../utils/handleApiError';
import FormInput from '../../components/common/FormInput';
import SubmitButton from '../../components/common/SubmitButton';

const Login = () => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();

  const { isAuthenticated } = useAuth();
  const from = location.state?.from?.pathname || '/';

  // --- Состояния ---
  const [step, setStep] = useState(1);
  const [email, setEmail] = useState('');
  const [otp, setOtp] = useState('');
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const isVerifying = useSelector((state) => state.ui.isLoading);

  useEffect(() => {
    if (isAuthenticated) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

  // --- Валидация ---
  const validate = () => {
    const newErrors = {};
    if (step === 1) {
      if (!email) newErrors.email = 'Email is required.';
      else if (!/^\S+@\S+\.\S+$/.test(email)) newErrors.email = 'Invalid email format.';
    }
    if (step === 2) {
      if (!otp) newErrors.otp = 'Code is required.';
      else if (!/^\d{6}$/.test(otp)) newErrors.otp = 'Code must be exactly 6 digits.';
    }
    return newErrors;
  };

  // --- Хендлеры ---
  const handleRequestOtp = async (event) => {
    event.preventDefault();
    setErrors({});
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    setLoading(true);
    try {
      await authService.requestOtp(email, 'patient');
      setStep(2);
    } catch (err) {
      setErrors(handleApiError(err));
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOtp = async (event) => {
    event.preventDefault();
    setErrors({});
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    const resultAction = await dispatch(verifyOtpAndLogin({ email, otp, role: 'patient' }));
    if (!verifyOtpAndLogin.fulfilled.match(resultAction)) {
      setErrors(resultAction.payload || { general: 'An unknown error occurred.' });
    }
  };
  
  const handleEmailChange = (e) => {
    setEmail(e.target.value);
    if (errors.email) setErrors(prev => ({ ...prev, email: null, general: null }));
  };
  const handleOtpChange = (e) => {
    const value = e.target.value.replace(/[^0-9]/g, '').slice(0, 6);
    setOtp(value);
    if (errors.otp) setErrors(prev => ({ ...prev, otp: null, general: null }));
  };

  const renderStepOne = () => (
    <Box component="form" onSubmit={handleRequestOtp} noValidate sx={{ mt: 3, width: '100%' }}>
      <Typography variant="body1" align="center" sx={{ mb: 2 }}>
        Enter your email to sign in.
      </Typography>
      <FormInput name="email" label="Email" type="email" value={email} onChange={handleEmailChange} error={errors.email} required />
      <SubmitButton isLoading={loading}>Get Code</SubmitButton>
    </Box>
  );

  const renderStepTwo = () => (
    <Box component="form" onSubmit={handleVerifyOtp} noValidate sx={{ mt: 3, width: '100%' }}>
      <Typography variant="body2" align="center" sx={{ mb: 2 }}>
        A sign-in code has been sent to <strong>{email}</strong>.
      </Typography>
      <FormInput name="otp" label="Confirmation Code" value={otp} onChange={handleOtpChange} error={errors.otp} required />
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 2 }}>
        <Button variant="text" onClick={() => { setStep(1); setErrors({}); }} disabled={isVerifying}>
          Back
        </Button>
        <SubmitButton isLoading={isVerifying} sx={{ m: 0, width: 'auto' }}>
          Sign In
        </SubmitButton>
      </Box>
    </Box>
  );

  return (
    <Container maxWidth="xs" sx={{ py: 4 }}>
      <Paper sx={{ p: { xs: 2, sm: 4 }, position: 'relative' }}>
        <IconButton
          aria-label="close"
          onClick={() => navigate('/')}
          sx={{ position: 'absolute', top: 8, right: 8, color: (theme) => theme.palette.grey[500] }}
        >
          <CloseIcon />
        </IconButton>
        <Box
          sx={{
            marginTop: 2,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          <Typography component="h1" variant="h5">
            Sign In
          </Typography>
          
          {errors.general && <Alert severity="error" sx={{ width: '100%', mt: 2 }}>{errors.general}</Alert>}

          {step === 1 ? renderStepOne() : renderStepTwo()}
          
          <Typography align="center" variant="body2" sx={{ mt: 3 }}>
            Don't have an account?{' '}
            <MuiLink component={RouterLink} to="/signup" underline="hover">
              Sign Up
            </MuiLink>
          </Typography>
        </Box>
      </Paper>
    </Container>
  );
};

export default Login;