// src/services/authService.js
import axiosInstance from '../api/axiosInstance';

const API_PREFIX = '/api/PatientAuth'; // Жестко задаем префикс пациента

const authService = {
  requestOtp: async (email) => {
    const response = await axiosInstance.post(`${API_PREFIX}/send-sign-in-code`, { email });
    return response.data;
  },
  verifyOtp: async ({ email, otp }) => {
    const response = await axiosInstance.post(`${API_PREFIX}/confirm-sign-in-code`, { email, otp });
    return { token: response.data };
  },
  checkAccess: async () => {
    const response = await axiosInstance.get(`${API_PREFIX}/check-access`);
    return response.data;
  },
  getMe: async () => {
    const response = await axiosInstance.get(`${API_PREFIX}/Me`);
    return response.data;
  },
  updateMe: async (updateCommand) => {
    const response = await axiosInstance.put(`${API_PREFIX}/Me`, updateCommand);
    return response.data;
  },
  signUp: async (signUpData) => {
    const response = await axiosInstance.post(`${API_PREFIX}/sign-up`, signUpData);
    return response.data;
  },
  resendSignUp: async (email) => {
    const response = await axiosInstance.post(`${API_PREFIX}/send-sign-up-code`, email);
    return response.data;
  },
    confirmSignUp: async (confirmationData) => {
    const response = await axiosInstance.post('/api/PatientAuth/confirm-sign-up-code', confirmationData);
    return response.data;
  },
};

export default authService;