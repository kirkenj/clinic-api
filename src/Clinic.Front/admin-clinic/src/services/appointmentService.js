import axiosInstance from '../api/axiosInstance';

const API_URL = '/api/Appointment';

/**
 * Сервис для выполнения всех операций с записями на прием (Appointments).
 * Предназначен для использования в админ-панели.
 */
const appointmentService = {
  /**
   * Получение списка свободных слотов для доктора на конкретную дату.
   */
  getAvailableSlots: async (date, doctorId) => {
    const response = await axiosInstance.get(`${API_URL}/available-slots-for-doctor`, { params: { Date: date, DoctorId: doctorId } });
    return response.data;
  },

  /**
   * Получение страничного списка записей с фильтрами.
   */
  getPaged: async (params = {}) => {
    const response = await axiosInstance.get(`${API_URL}/paged`, { params });
    return response.data;
  },

  /**
   * Получение одной записи по ID.
   */
  getById: async (id) => {
    const response = await axiosInstance.get(`${API_URL}/${id}`);
    return response.data;
  },

  /**
   * Создание новой записи от имени персонала.
   */
  createByStaff: async (addCommand) => {
    const response = await axiosInstance.post(API_URL, addCommand);
    return response.data;
  },

  /**
   * Запрос на отправку OTP для подтверждения записи пациентом.
   */
  sendOtpToConfirmByPatient: async (confirmRequest) => {
    const response = await axiosInstance.post(`${API_URL}/send-otp-to-confirm-appointment-by-patient`, confirmRequest);
    return response.data;
  },

  /**
   * Подтверждение записи со стороны пациента (инициируется админом).
   */
  approveByPatient: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_URL}/approve-by-patient`, approveCommand);
    return response.data;
  },

  /**
   * Подтверждение записи со стороны клиники.
   */
  approveByClinic: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_URL}/approve-by-clinic`, approveCommand);
    return response.data;
  },

  /**
   * Инициирование отмены записи со стороны пациента.
   */
  cancelByPatient: async (cancelCommand) => {
    const response = await axiosInstance.post(`${API_URL}/cancel-by-patient`, cancelCommand);
    return response.data;
  },

  /**
   * Подтверждение отмены записи со стороны пациента с OTP.
   */
  approveCancellationByPatient: async (approveCommand) => {
    const response = await axiosInstance.post(`${API_URL}/approve-cancellation-by-patient`, approveCommand);
    return response.data;
  },

  /**
   * Отмена записи со стороны клиники.
   */
  cancelByClinic: async (cancelCommand) => {
    const response = await axiosInstance.post(`${API_URL}/cancel-appointment-by-clinic`, cancelCommand);
    return response.data;
  },

  /**
   * Перенос записи на другое время/к другому доктору.
   */
  reschedule: async (rescheduleCommand) => {
    const response = await axiosInstance.post(`${API_URL}/reschedule`, rescheduleCommand);
    return response.data;
  },
  
  /**
   * Получение опций расписания.
   */
  getTimetableOptions: async () => {
    const response = await axiosInstance.get(`${API_URL}/timetable-options`);
    return response.data;
  },
};

export default appointmentService;