// src/services/doctorService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Doctor';

const doctorService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },
  create: async (addDoctorCommand) => {
    const response = await axiosInstance.post(API_URL, addDoctorCommand);
    return response.data;
  },
  update: async (updateDoctorCommand) => {
    const response = await axiosInstance.put(API_URL, updateDoctorCommand);
    return response.data;
  },
  delete: async (id) => {
    const response = await axiosInstance.delete(`${API_URL}/${id}`);
    return response.data;
  },
};

export default doctorService;