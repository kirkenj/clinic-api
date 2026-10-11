// src/components/GlobalPreloader.jsx
import React from 'react';
import { useSelector } from 'react-redux';
import { Backdrop, CircularProgress } from '@mui/material';

/**
 * A global preloader component that shows a full-screen loading indicator
 * based on the `isLoading` state in the Redux UI slice.
 */
const GlobalPreloader = () => {
  // Subscribes to the isLoading state from the Redux store
  const isLoading = useSelector((state) => state.ui.isLoading);

  return (
    // Backdrop is an MUI component for dimming the background
    <Backdrop
      // A high zIndex ensures it's on top of everything else
      sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 10 }}
      open={isLoading} // Shown or hidden based on the state
    >
      <CircularProgress color="inherit" />
    </Backdrop>
  );
};

export default GlobalPreloader;