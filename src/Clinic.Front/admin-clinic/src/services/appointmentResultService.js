// src/services/appointmentResultService.js
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/AppointmentResult';

/**
 * Сервис для выполнения CRUD-операций с результатами приемов.
 */
const appointmentResultService = {
  /**
   * Получение страничного списка результатов приемов.
   */
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Получение результата по ID записи на прием (appointmentId).
   */
  getByAppointmentId: async (appointmentId) => {
    const response = await axiosInstance.get(`${API_URL}/${appointmentId}`);
    return response.data;
  },

  /**
   * Создание нового результата приема.
   */
  create: async (addCommand) => {
    const response = await axiosInstance.post(API_URL, addCommand);
    return response.data;
  },

  /**
   * Обновление существующего результата приема.
   */
  update: async (updateCommand) => {
    const response = await axiosInstance.put(API_URL, updateCommand);
    return response.data;
  },

  /**
   * Удаление результата приема по его ID.
   */
  delete: async (id) => {
    const response = await axiosInstance.delete(`${API_URL}/${id}`);
    return response.data;
  },
};

export default appointmentResultService;