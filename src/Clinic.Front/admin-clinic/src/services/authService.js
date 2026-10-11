// src/services/authService.js
import axiosInstance from '../api/axiosInstance';

const API_PREFIX = '/api/ReceptionistAuth';

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
  }
};

export default authService;