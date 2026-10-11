// src/services/appointmentResultService.js (для Patient-App)
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/AppointmentResult';

/**
 * Service for a patient to view their appointment results.
 */
const appointmentResultService = {
  /**
   * Gets a paginated list of results.
   * Пациент, скорее всего, не будет вызывать этот метод напрямую.
   * Но он может быть полезен, если вы захотите сделать страницу "История всех результатов".
   */
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Gets the result for a specific appointment. This is the main method we'll use.
   */
  getByAppointmentId: async (appointmentId) => {
    const response = await axiosInstance.get(`${API_URL}/${appointmentId}`);
    return response.data;
  },
};

export default appointmentResultService;