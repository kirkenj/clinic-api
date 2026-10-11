// src/components/ErrorFallback.jsx
import React from 'react';
import { Box, Typography, Button } from '@mui/material';

/**
 * A fallback UI component displayed by ErrorBoundary when a critical
 * rendering error occurs in the application.
 */
const ErrorFallback = () => (
  <Box
    sx={{
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      height: '100vh',
      textAlign: 'center',
      p: 2,
    }}
  >
    <Typography variant="h4" gutterBottom>
      Something went wrong.
    </Typography>
    <Typography variant="body1" sx={{ mb: 2 }}>
      An unexpected error occurred. Please try reloading the page.
    </Typography>
    <Button variant="contained" onClick={() => window.location.reload()}>
      Reload Page
    </Button>
  </Box>
);

export default ErrorFallback;