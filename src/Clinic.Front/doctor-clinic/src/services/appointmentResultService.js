// src/services/appointmentResultService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/AppointmentResult';

const appointmentResultService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },
  getByAppointmentId: async (appointmentId) => {
    const response = await axiosInstance.get(`${API_URL}/${appointmentId}`);
    return response.data;
  },
  create: async (addCommand) => {
    const response = await axiosInstance.post(API_URL, addCommand);
    return response.data;
  },
  update: async (updateCommand) => {
    const response = await axiosInstance.put(API_URL, updateCommand);
    return response.data;
  },
  delete: async (id) => {
    const response = await axiosInstance.delete(`${API_URL}/${id}`);
    return response.data;
  },
};

export default appointmentResultService;