// src/services/appointmentService.js (для Patient-App)
import axiosInstance from '../api/axiosInstance';

const API_PREFIX = '/api/Appointment';

const appointmentService = {
  // --- Публичные методы для создания записи ---
  getAvailableSlots: async (date, doctorId) => { 
    const response = await axiosInstance.get(`${API_PREFIX}/available-slots-for-doctor`, { params: { date, doctorId } });
    return response.data;
  },
  
  // --- Методы, требующие авторизации пациента ---
  createAsPatient: async (addCommand) => {
    const response = await axiosInstance.post(`${API_PREFIX}/as-patient`, addCommand);
    return response.data;
  },
  getMyPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_PREFIX}/as-patient`, { params });
    return response.data;
  },
  getMyById: async (id) => {
    const response = await axiosInstance.get(`${API_PREFIX}/as-patient/${id}`);
    return response.data;
  },

  // --- Флоу подтверждения записи пациентом ---
  sendOtpToConfirm: async (confirmRequest) => {
    const response = await axiosInstance.post(`${API_PREFIX}/send-otp-to-confirm-appointment-by-patient`, confirmRequest);
    return response.data;
  },
  approveByPatient: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_PREFIX}/approve-by-patient`, approveCommand);
    return response.data;
  },
  
  // --- Флоу отмены записи пациентом ---
  cancelByPatient: async (cancelCommand) => {
    const response = await axiosInstance.post(`${API_PREFIX}/cancel-by-patient`, cancelCommand);
    return response.data;
  },
  approveCancellationByPatient: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_PREFIX}/approve-cancellation-by-patient`, approveCommand);
    return response.data;
  },
};

export default appointmentService;