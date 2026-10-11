// src/constants/appointmentConstants.js

// Числовые значения, соответствующие C# enum AppointmentStatus
export const APPOINTMENT_STATUSES = {
  WaitingUserConfirm: 0,
  WaitingClinicConfirm: 1,
  Rescheduled: 2,
  Confirmed: 3,
  CancelledByUser: 4,
  CancelledByClinic: 5,
};

// Обратный маппинг для парсинга строкового ответа от API в число
export const APPOINTMENT_STATUS_FROM_STRING = {
  WaitingUserConfirm: APPOINTMENT_STATUSES.WaitingUserConfirm,
  WaitingClinicConfirm: APPOINTMENT_STATUSES.WaitingClinicConfirm,
  Rescheduled: APPOINTMENT_STATUSES.Rescheduled,
  Confirmed: APPOINTMENT_STATUSES.Confirmed,
  CancelledByUser: APPOINTMENT_STATUSES.CancelledByUser,
  CancelledByClinic: APPOINTMENT_STATUSES.CancelledByClinic,
};

// Тексты для отображения в UI. Ключи - числовые значения.
export const APPOINTMENT_STATUS_TEXT = {
  [APPOINTMENT_STATUSES.WaitingUserConfirm]: 'Awaiting Patient', // ИСПРАВЛЕНО
  [APPOINTMENT_STATUSES.WaitingClinicConfirm]: 'Awaiting Clinic',
  [APPOINTMENT_STATUSES.Rescheduled]: 'Rescheduled',
  [APPOINTMENT_STATUSES.Confirmed]: 'Confirmed',
  [APPOINTMENT_STATUSES.CancelledByUser]: 'Cancelled by Patient',
  [APPOINTMENT_STATUSES.CancelledByClinic]: 'Cancelled by Clinic',
};

// Цвета для Chip в UI. Ключи - числовые значения.
export const APPOINTMENT_STATUS_COLORS = {
  [APPOINTMENT_STATUSES.WaitingUserConfirm]: 'warning', // ИСПРАВЛЕНО
  [APPOINTMENT_STATUSES.WaitingForClinic]: 'warning',
  [APPOINTMENT_STATUSES.Rescheduled]: 'info',
  [APPOINTMENT_STATUSES.Confirmed]: 'primary',
  [APPOINTMENT_STATUSES.CancelledByUser]: 'default',
  [APPOINTMENT_STATUSES.CancelledByClinic]: 'error',
};