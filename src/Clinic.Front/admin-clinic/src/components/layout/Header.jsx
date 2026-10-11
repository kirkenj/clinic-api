// src/components/layout/Header.jsx
import React from 'react';
import { useDispatch } from 'react-redux';
import { Link as RouterLink } from 'react-router-dom'; // Импортируем Link для навигации
import { AppBar, Toolbar, Typography, Box, Button, IconButton, Link } from '@mui/material';
import AccountCircle from '@mui/icons-material/AccountCircle';
import MenuIcon from '@mui/icons-material/Menu';
import { logout } from '../../store/slices/authSlice';
import { useAuth } from '../../hooks/useAuth';

const Header = ({ onDrawerToggle }) => {
  const dispatch = useDispatch();
  const { user } = useAuth();

  const handleLogout = () => {
    dispatch(logout());
  };

  return (
    <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
      <Toolbar>
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={onDrawerToggle}
          sx={{ mr: 2, display: { sm: 'none' } }}
        >
          <MenuIcon />
        </IconButton>

        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Clinic Admin Panel
        </Typography>

        {user && (
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            {/* Делаем иконку и имя ссылками на /profile */}
            <Link component={RouterLink} to="/profile" color="inherit" sx={{ display: 'flex', alignItems: 'center', textDecoration: 'none' }}>
              <IconButton color="inherit" aria-label="account of current user">
                <AccountCircle />
              </IconButton>
              <Typography sx={{ display: { xs: 'none', sm: 'block' }, mr: 2 }}>
                {user.fullName || user.email}
              </Typography>
            </Link>

            <Button color="inherit" variant="outlined" onClick={handleLogout}>
              Logout
            </Button>
          </Box>
        )}
      </Toolbar>
    </AppBar>
  );
};

export default Header;