// src/store/store.js
import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import uiReducer from './slices/uiSlice';

// Собираем все редьюсеры (slices) в один глобальный store
export const store = configureStore({
  reducer: {
    auth: authReducer, // Всё, что связано с аутентификацией
    ui: uiReducer,     // Всё, что связано с интерфейсом (загрузка, уведомления)
  },
});