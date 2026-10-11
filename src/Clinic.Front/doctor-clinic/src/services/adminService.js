// src/services/adminService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Receptionist';

/**
 * Service for reading Administrator (Receptionist) data.
 * Editing is handled via authService.
 */
const adminService = {
  /**
   * Gets a paginated list of administrators.
   */
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Gets a single administrator by their ID.
   */
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },
};

export default adminService;