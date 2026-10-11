// src/App.jsx
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { BrowserRouter as Router } from 'react-router-dom';
import { ThemeProvider, CssBaseline, Box, CircularProgress } from '@mui/material';
import { theme } from './theme/theme';
import AppRoutes from './routes/AppRoutes';
import GlobalPreloader from './components/GlobalPreloader';
import ErrorBoundary from './context/ErrorBoundary';
import { checkAuth } from './store/slices/authSlice'; // 1. Импортируем thunk
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';

function App() {
  const dispatch = useDispatch();
  // Получаем флаг, чтобы знать, когда можно рендерить роуты
  const isAuthChecked = useSelector((state) => state.auth.isAuthChecked);

  // 2. Вызываем проверку при первом монтировании компонента
  useEffect(() => {
    dispatch(checkAuth());
  }, [dispatch]); // Зависимость от dispatch - стандартная практика

  // 3. Показываем прелоадер на весь экран, пока идет проверка
  // Это предотвращает "мигание" страницы логина для авторизованного пользователя
  if (!isAuthChecked) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <ErrorBoundary>
          <Router>
            <GlobalPreloader />
            <AppRoutes />
          </Router>
        </ErrorBoundary>
      </ThemeProvider>
    </LocalizationProvider>
  );
}

export default App;