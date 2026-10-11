// src/hoc/PrivateRoute.jsx
import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth'; // Наш хук для доступа к состоянию аутентификации

/**
 * A route guard component.
 * It checks if the user is authenticated. If so, it renders the child routes.
 * If not, it redirects the user to the login page.
 */
const PrivateRoute = () => {
  // 1. Получаем статус аутентификации из Redux store через наш кастомный хук.
  const { isAuthenticated } = useAuth();

  // 2. Возвращаем либо дочерний роут, либо компонент для редиректа.
  // <Outlet /> - это специальный компонент react-router, который рендерит
  // следующий совпавший вложенный роут.
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

export default PrivateRoute;