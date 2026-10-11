// src/services/appointmentService.js (для Doctor-App)
import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Appointment';

const appointmentService = {
  getAvailableSlots: async (date, doctorId) => {
    const response = await axiosInstance.get(`${API_URL}/available-slots-for-doctor`, { params: { Date: date, DoctorId: doctorId } });
    return response.data;
  },
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },
  approveByClinic: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_URL}/approve-by-clinic`, approveCommand);
    return response.data;
  },
  cancelByClinic: async (cancelCommand) => {
    const response = await axiosInstance.post(`${API_URL}/cancel-appointment-by-clinic`, cancelCommand);
    return response.data;
  },
  reschedule: async (rescheduleCommand) => {
    const response = await axiosInstance.post(`${API_URL}/reschedule`, rescheduleCommand);
    return response.data;
  },
    getTimetableOptions: async () => {
    const response = await axiosInstance.get(`${API_URL}/timetable-options`);
    return response.data;
  },
};

export default appointmentService;