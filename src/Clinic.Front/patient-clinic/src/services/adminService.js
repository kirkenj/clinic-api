// src/services/adminService.js (для Patient-App)
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Receptionist';

/**
 * Service to get information about administrators (for contact purposes).
 */
const adminService = {
  /**
   * Gets a paginated list of administrators.
   */
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },
  // getById может быть не нужен, если контакты есть в общем списке.
  // Оставляем на всякий случай.
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },
};

export default adminService;