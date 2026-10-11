// src/services/serviceService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Service'; 

const serviceService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
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

export default serviceService;