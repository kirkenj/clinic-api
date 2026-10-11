// src/components/layout/BottomNav.jsx
import React from 'react';
import { Paper, BottomNavigation, BottomNavigationAction } from '@mui/material';
import EventIcon from '@mui/icons-material/Event';
import AddCircleOutlineIcon from '@mui/icons-material/AddCircleOutline';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import LoginIcon from '@mui/icons-material/Login'; // Иконка для входа
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth'; // Импортируем наш хук
import SupportAgentIcon from '@mui/icons-material/SupportAgent'; // Новая иконка

const BottomNav = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated } = useAuth(); // Получаем статус аутентификации

  const getCurrentValue = () => {
    if (location.pathname.startsWith('/new-appointment')) return '/new-appointment';
    if (isAuthenticated && location.pathname.startsWith('/profile')) return '/profile';
    if (isAuthenticated) return '/my-appointments';
    return false; // Никакая кнопка не активна по умолчанию для гостя
  };

  const handleChange = (event, newValue) => {
    navigate(newValue);
  };

  return (
    <Paper sx={{ position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000 }} elevation={3}>
      <BottomNavigation
        showLabels
        value={getCurrentValue()}
        onChange={handleChange}
      >
        {isAuthenticated ? (
          // --- Кнопки для авторизованного пользователя ---
          [
            <BottomNavigationAction key="my-appointments" label="My Appointments" value="/my-appointments" icon={<EventIcon />} />,
            <BottomNavigationAction key="book" label="Book Now" value="/new-appointment" icon={<AddCircleOutlineIcon />} />,
            <BottomNavigationAction key="support" label="Support" value="/support" icon={<SupportAgentIcon />} />,
            <BottomNavigationAction key="profile" label="My Profile" value="/profile" icon={<AccountCircleIcon />} />
          ]
        ) : (
          // --- Кнопки для гостя ---
          [
            <BottomNavigationAction key="book" label="Book Now" value="/new-appointment" icon={<AddCircleOutlineIcon />} />,
            <BottomNavigationAction key="login" label="Sign In" value="/login" icon={<LoginIcon />} />,
            <BottomNavigationAction key="support" label="Support" value="/support" icon={<SupportAgentIcon />} />,
          ]
        )}
      </BottomNavigation>
    </Paper>
  );
};

export default BottomNav;