// src/api/axiosInstance.js
import axios from 'axios';
import qs from 'qs'; // 1. Импортируем библиотеку
import { store } from '../store/store';
import { setLoading } from '../store/slices/uiSlice';
import { logout } from '../store/slices/authSlice';

const axiosInstance = axios.create({
  baseURL: window.config.API_URL, 
  
  // 2. ДОБАВЛЯЕМ СЕРИАЛИЗАТОР ПАРАМЕТРОВ
  // Эта настройка говорит axios, как форматировать массивы в URL.
  paramsSerializer: {
    // Используем функцию serialize из qs
    serialize: (params) => {
      // `arrayFormat: 'repeat'` - это именно тот формат, который ожидает ASP.NET Core
      // Он превратит { Ids: [1, 2] } в "Ids=1&Ids=2"
      return qs.stringify(params, { arrayFormat: 'repeat' });
    }
  }
});

// --- Остальной код с интерсепторами остается без изменений ---

axiosInstance.interceptors.request.use(
  (config) => {
    store.dispatch(setLoading(true));
    const token = store.getState().auth.token;
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    store.dispatch(setLoading(false));
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  (response) => {
    store.dispatch(setLoading(false));
    return response;
  },
  (error) => {
    store.dispatch(setLoading(false));
    if (error.response?.status === 401) {
      store.dispatch(logout());
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;