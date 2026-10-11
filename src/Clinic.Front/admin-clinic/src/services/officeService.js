// src/services/officeService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Office';

const officeService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Gets a list of offices by their IDs.
   * @param {number[]} ids - An array of office IDs.
   */
  getByIds: async (ids) => {
    // Return early if the array is empty to avoid an unnecessary API call.
    if (!ids || ids.length === 0) {
      return { items: [] };
    }
    
    try {
      // --- THE FIX IS HERE ---
      // We must provide all required pagination parameters.
      // To get all specified IDs in one go, we set page=1 and pageSize to the number of IDs.
      const response = await axiosInstance.get(`${API_URL}/paged`, {
        params: { 
          Ids: ids,
          page: 1, 
          pageSize: ids.length 
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching offices by ids:', error);
      throw error;
    }
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

export default officeService;