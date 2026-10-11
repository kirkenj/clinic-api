// src/hooks/useAuth.js
import { useMemo } from 'react';
import { useSelector } from 'react-redux';

/**
 * A custom hook to access authentication data from the Redux store.
 * Provides a convenient and memoized access to the user state, authentication status,
 * and the status of the initial authentication check.
 *
 * @returns {object} An object containing:
 * - `user`: The current user object or null.
 * - `isAuthenticated`: A boolean, true if the user is authenticated.
 * - `isAuthChecked`: A boolean, true if the initial auth check has completed.
 * - `role`: The user's role or null.
 */
export const useAuth = () => {
  // 1. Извлекаем ВСЕ необходимые поля из auth-слайса
  const { user, isAuthenticated, isAuthChecked } = useSelector((state) => state.auth);

  // 2. Мемоизируем результат, включая isAuthChecked
  return useMemo(() => ({
    user,
    isAuthenticated,
    isAuthChecked, // <-- Добавляем это поле
    role: user?.role || null,
  }), [user, isAuthenticated, isAuthChecked]); // Добавляем в массив зависимостей
};