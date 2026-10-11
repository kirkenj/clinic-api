// src/components/layout/MainLayout.jsx
import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Container } from '@mui/material';
import BottomNav from './BottomNav';

// src/components/layout/MainLayout.jsx
const MainLayout = () => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Container component="main" sx={{ flexGrow: 1, py: 3, mb: '56px' }}>
        <Outlet />
      </Container>
      <BottomNav />
    </Box>
  );
};

export default MainLayout;