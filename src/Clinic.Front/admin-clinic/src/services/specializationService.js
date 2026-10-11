// src/services/specializationService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Specialization';

const specializationService = {
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Gets a list of specializations by their IDs.
   * @param {number[]} ids - An array of specialization IDs.
   */
  getByIds: async (ids) => {
    if (!ids || ids.length === 0) {
      return { items: [] };
    }
    try {
      // --- THE FIX IS HERE ---
      const response = await axiosInstance.get(`${API_URL}/paged`, {
        params: { 
          Ids: ids, 
          page: 1, 
          pageSize: ids.length 
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching specializations by ids:', error);
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

export default specializationService;