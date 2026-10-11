// src/pages/Landing.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { Box, CircularProgress } from '@mui/material';

/**
 * A "fork" component that redirects users based on their authentication status.
 * - Authenticated users go to their appointments list.
 * - Unauthenticated users go to the new appointment form.
 */
const Landing = () => {
  const { isAuthenticated, isAuthChecked } = useAuth();

  // Пока идет первоначальная проверка сессии в App.jsx, показываем прелоадер.
  // Это предотвращает "мигание" страницы.
  if (!isAuthChecked) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  // После проверки выполняем редирект.
  if (isAuthenticated) {
    return <Navigate to="/my-appointments" replace />;
  } else {
    return <Navigate to="/new-appointment" replace />;
  }
};

export default Landing;