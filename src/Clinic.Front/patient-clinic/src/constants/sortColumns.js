// src/constants/sortColumns.js

/**
 * Этот файл содержит константы, которые сопоставляют имена полей для сортировки
 * с их числовыми значениями (enum) на бэкенде.
 * Это делает код на фронтенде более читаемым и легко поддерживаемым.
 */

export const APPOINTMENT_RESULT_SORT_COLUMNS = {
  id: 0,
  appointmentId: 1,
};

export const APPOINTMENT_SORT_COLUMNS = {
  id: 0,
  doctorId: 1, 
  patientId: 2,
  serviceId: 3,
  dateTimeStart: 4,
  isApprovedByClinic: 5,
  isApprovedByUser: 6,
  status: 7,
};

export const DOCTOR_SORT_COLUMNS = {
  id: 0,
  firstName: 1,
  lastName: 2,
  middleName: 3,
  email: 4,
  phoneNumber: 5,
  specializationId: 6,
  officeId: 7,
};

export const OFFICE_SORT_COLUMNS = {
  id: 0,
  address: 1,
  phoneNumber: 2,
};

export const PATIENT_SORT_COLUMNS = {
  id: 0,
  firstName: 1,
  lastName: 2,
  middleName: 3,
  email: 4,
  phoneNumber: 5,
};

export const RECEPTIONIST_SORT_COLUMNS = {
  id: 0,
  firstName: 1,
  lastName: 2,
  middleName: 3,
  email: 4,
  phoneNumber: 5,
};

export const SERVICE_SORT_COLUMNS = {
  id: 0,
  name: 1,
  price: 2,
  specializationId: 3,
};

export const SPECIALIZATION_SORT_COLUMNS = {
  id: 0,
  name: 1,
};