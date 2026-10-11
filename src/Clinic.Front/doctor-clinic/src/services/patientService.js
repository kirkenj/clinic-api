// src/services/patientService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Patient';

const patientService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },
};

export default patientService;