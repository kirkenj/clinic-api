// src/components/layout/MainLayout.jsx
import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Toolbar } from '@mui/material';
import Header from './Header';
import Sidebar from './Sidebar';

const drawerWidth = 240; // Define drawer width for calculations

/**
 * The main layout component for the application.
 * It orchestrates the Header and Sidebar and provides a content area for pages.
 */
const MainLayout = () => {
  // State to manage the visibility of the mobile sidebar
  const [mobileOpen, setMobileOpen] = useState(false);

  // Function to toggle the mobile sidebar's state
  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  return (
    <Box sx={{ display: 'flex' }}>
      {/* Pass the toggle function to the Header */}
      <Header onDrawerToggle={handleDrawerToggle} />

      {/* Pass the state and the toggle function to the Sidebar */}
      <Sidebar 
        mobileOpen={mobileOpen}
        onDrawerToggle={handleDrawerToggle}
      />

      {/* Main content area for the pages */}
      <Box 
        component="main" 
        sx={{ 
          flexGrow: 1, 
          p: 3, 
          // Set content width, accounting for the drawer on desktop
          width: { sm: `calc(100% - ${drawerWidth}px)` } 
        }}
      >
        {/* A spacer to prevent content from being hidden under the fixed Header */}
        <Toolbar />
        
        {/* react-router will render the current page's component here */}
        <Outlet />
      </Box>
    </Box>
  );
};

export default MainLayout;