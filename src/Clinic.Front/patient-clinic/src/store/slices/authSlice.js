// src/store/slices/authSlice.js
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { jwtDecode } from 'jwt-decode';
import axios from 'axios'; // 1. Импортируем 'axios' напрямую!

import authService from '../../services/authService';
import { handleApiError } from '../../utils/handleApiError';
import { setIsLoggingIn } from './uiSlice';

const token = localStorage.getItem('token');

const initialState = {
  user: null,
  token: token,
  isAuthenticated: !!token,
  isAuthChecked: false,
};

export const verifyOtpAndLogin = createAsyncThunk(
  'auth/verifyOtp',
  async (credentials, { dispatch, rejectWithValue }) => {
    dispatch(setIsLoggingIn(true));
    try {
      // Шаг 1: Получаем токен. Этот запрос анонимный, используем authService.
      const { token } = await authService.verifyOtp(credentials);
      localStorage.setItem('token', token);

      // Шаг 2: Получаем профиль с помощью ОДНОРАЗОВОГО экземпляра axios.
      const userProfileResponse = await axios.get(
        `${window.config.API_URL}/api/PatientAuth/Me`,
        {
          headers: {
            'Authorization': `Bearer ${token}` // Вручную устанавливаем заголовок
          }
        }
      );
      const userProfile = userProfileResponse.data;

      // Шаг 3: Собираем финальный объект
      const fullName = [userProfile.lastName, userProfile.firstName, userProfile.middleName].filter(Boolean).join(' ');
      const finalUserObject = { ...userProfile, role: 'Patient', fullName };

      return { token, user: finalUserObject };
    } catch (error) {
      localStorage.removeItem('token');
      return rejectWithValue(handleApiError(error));
    } finally {
      dispatch(setIsLoggingIn(false));
    }
  }
);

export const checkAuth = createAsyncThunk(
  'auth/check',
  async (_, { getState, rejectWithValue }) => {
    const { ui } = getState();
    if (ui.isLoggingIn) {
      return rejectWithValue('Login process is active, skipping check.');
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) throw new Error('No token found');

      // ... (проверка срока годности и роли токена) ...
      const decodedToken = jwtDecode(token);
      if (decodedToken.exp * 1000 < Date.now()) throw new Error('Token expired');
      const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
      if (decodedToken[roleClaim] !== 'Patient') {
        throw new Error('Invalid role for Admin Panel');
      }

      // Используем тот же "одноразовый" подход для проверки доступа и получения профиля
      const checkAccessPromise = axios.get(
        `${window.config.API_URL}/api/PatientAuth/check-access`,
        { headers: { 'Authorization': `Bearer ${token}` } }
      );
      const getMePromise = axios.get(
        `${window.config.API_URL}/api/PatientAuth/Me`,
        { headers: { 'Authorization': `Bearer ${token}` } }
      );
      
      const [_, userProfileResponse] = await Promise.all([checkAccessPromise, getMePromise]);
      const userProfile = userProfileResponse.data;

      const fullName = [userProfile.lastName, userProfile.firstName, userProfile.middleName].filter(Boolean).join(' ');
      const finalUserObject = { ...userProfile, role: 'Patient', fullName };

      return { token, user: finalUserObject };
    } catch (error) {
      localStorage.removeItem('token');
      return rejectWithValue(error.message);
    }
  }
);

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: (state) => {
      localStorage.removeItem('token');
      state.token = null;
      state.user = null;
      state.isAuthenticated = false;
      state.isAuthChecked = true;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(verifyOtpAndLogin.fulfilled, (state, action) => {
        state.isAuthenticated = true;
        state.token = action.payload.token;
        state.user = action.payload.user;
        state.isAuthChecked = true;
      })
      .addCase(checkAuth.fulfilled, (state, action) => {
        state.isAuthenticated = true;
        state.token = action.payload.token;
        state.user = action.payload.user;
        state.isAuthChecked = true;
      })
      .addCase(checkAuth.rejected, (state) => {
        state.isAuthenticated = false;
        state.token = null;
        state.user = null;
        state.isAuthChecked = true;
      });
  },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;