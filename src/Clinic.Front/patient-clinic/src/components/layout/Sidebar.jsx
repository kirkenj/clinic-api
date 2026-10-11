// src/components/layout/Sidebar.jsx
import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  Drawer, List, ListItem, ListItemButton, ListItemIcon, ListItemText,
  Toolbar, Box
} from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard'; 
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth'; 
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import PeopleIcon from '@mui/icons-material/People';
import MedicalServicesIcon from '@mui/icons-material/MedicalServices';
const drawerWidth = 240;

// Navigation items for the sidebar
const navItems = [
  { text: 'Dashboard', icon: <DashboardIcon />, path: '/' },
  { text: 'My Schedule', icon: <CalendarMonthIcon />, path: '/my-schedule' },
  { text: 'Appointments', icon: <EventAvailableIcon />, path: '/appointments' },
  { text: 'Doctors', icon: <MedicalServicesIcon />, path: '/doctors' },
  { text: 'Patients', icon: <PeopleIcon />, path: '/patients' }, 
];

/**
 * The application's responsive Sidebar for navigation.
 * @param {object} props
 * @param {boolean} props.mobileOpen - The visibility state on mobile.
 * @param {function} props.onDrawerToggle - Function to close/toggle the sidebar.
 */
const Sidebar = ({ mobileOpen, onDrawerToggle }) => {
  // The content of the drawer, extracted for reusability.
  const drawerContent = (
    <div>
      <Toolbar /> {/* Spacer to place content below the header */}
      <Box sx={{ overflow: 'auto' }}>
        <List>
          {navItems.map((item) => (
            <NavLink
              to={item.path}
              key={item.text}
              end={item.path === '/'}
              onClick={mobileOpen ? onDrawerToggle : undefined} // Close on mobile click
              style={({ isActive }) => ({
                textDecoration: 'none',
                color: 'inherit',
                backgroundColor: isActive ? 'rgba(25, 118, 210, 0.08)' : 'transparent',
                display: 'block',
              })}
            >
              <ListItem disablePadding>
                <ListItemButton>
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={item.text} />
                </ListItemButton>
              </ListItem>
            </NavLink>
          ))}
        </List>
      </Box>
    </div>
  );

  return (
    <Box
      component="nav"
      sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      aria-label="main navigation"
    >
      {/* Mobile version: temporary, pop-out Drawer */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onDrawerToggle}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: 'block', sm: 'none' },
          '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
        }}
      >
        {drawerContent}
      </Drawer>
      
      {/* Desktop version: permanent, visible Drawer */}
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', sm: 'block' },
          '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
        }}
        open
      >
        {drawerContent}
      </Drawer>
    </Box>
  );
};

export default Sidebar;