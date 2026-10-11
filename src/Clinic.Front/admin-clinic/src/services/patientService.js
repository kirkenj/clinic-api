// src/services/patientService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Patient';
const AUTH_API_URL = '/api/PatientAuth'; // Префикс для эндпоинтов регистрации

const patientService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },

  update: async (updatePatientCommand) => {
    const response = await axiosInstance.put(API_URL, updatePatientCommand);
    return response.data;
  },
  
  delete: async (id) => {
    const response = await axiosInstance.delete(`${API_URL}/${id}`);
    return response.data;
  },

  register: async (signUpData) => {
    const response = await axiosInstance.post(`${AUTH_API_URL}/sign-up`, signUpData);
    return response.data;
  },

  confirmRegistration: async (confirmationData) => {
    const response = await axiosInstance.post(`${AUTH_API_URL}/confirm-sign-up-code`, confirmationData);
    return response.data;
  },

  resendSignUpOtp: async (email) => {
    // Предполагаем, что ваш API имеет такой эндпоинт, который мы ранее определили
    const response = await axiosInstance.post(`/api/PatientAuth/send-sign-up-code`, { email });
    return response.data;
  },
};

export default patientService;