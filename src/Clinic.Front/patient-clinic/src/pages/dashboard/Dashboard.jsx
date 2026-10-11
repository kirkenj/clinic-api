// src/pages/dashboard/Dashboard.jsx
import React from 'react';
import { Box, Typography, Paper } from '@mui/material';
import { useAuth } from '../../hooks/useAuth'; // Import the hook to access user data

/**
 * The main dashboard page of the application, displayed after login.
 * It shows a welcome message to the current user.
 */
const Dashboard = () => {
  // Get current user data using our custom hook
  const { user } = useAuth();

  return (
    // Paper creates a "card" with a shadow and background, which is nice for content
    <Paper sx={{ padding: 3, boxShadow: 3 }}>
      <Typography variant="h4" gutterBottom>
        Welcome!
      </Typography>
      
      {/* 
        Check if the user object exists before trying to access its properties.
        This prevents errors if the component renders before user data is loaded into Redux.
      */}
      {user ? (
        <Typography variant="body1">
          You are signed in as <strong>{user.fullName || user.email}</strong>.
        </Typography>
      ) : (
        <Typography variant="body1">
          Loading user data...
        </Typography>
      )}

      <Box sx={{ mt: 3 }}>
        <Typography variant="body1">
          Use the side menu to navigate through the application sections.
        </Typography>
      </Box>
    </Paper>
  );
};

export default Dashboard;