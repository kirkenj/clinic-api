// src/hooks/useAuth.js
import { useMemo } from 'react';
import { useSelector } from 'react-redux';

/**
 * A custom hook to access authentication data from the Redux store.
 * Provides a convenient and memoized access to the user state, token, and authentication flag.
 *
 * @returns {object} An object containing:
 * - `user`: The current user object or null.
 * - `token`: The user's JWT token or null.
 * - `role`: The user's role or null.
 * - `isAuthenticated`: A boolean, true if the user is authenticated.
 */
export const useAuth = () => {
  // Get the entire 'auth' slice from the Redux store.
  // useSelector subscribes the component to updates in this slice.
  const { user, token, isAuthenticated } = useSelector((state) => state.auth);

  // useMemo is used to memoize the returned object.
  // This prevents unnecessary re-renders of child components that receive this object as a prop.
  // A new object will only be created if `user`, `token`, or `isAuthenticated` changes.
  return useMemo(() => ({
    user,
    token,
    role: user?.role || null, // Conveniently extract the role from the user object
    isAuthenticated,
  }), [user, token, isAuthenticated]);
};