// src/utils/handleApiError.js

/**
 * Обрабатывает объект ошибки от Axios и возвращает стандартизированный объект с ошибками.
 * @param {Error} error - Объект ошибки, полученный из блока `catch`.
 * @returns {{general?: string, email?: string, otp?: string}} - Объект с ошибками.
 *          `general` для общих ошибок, остальные ключи - для конкретных полей.
 */
export const handleApiError = (error) => {
  console.error("API Error Caught:", error);

  if (error.response) {
    const data = error.response.data;

    // --- НОВАЯ ЛОГИКА ДЛЯ ФОРМАТА { code, description } ---
    if (data && typeof data.code === 'string' && typeof data.description === 'string') {
      const { code, description } = data;

      // Здесь мы сопоставляем API-коды с именами полей в нашей форме.
      // Это нужно будет дополнять по мере появления новых кодов от бэкенда.
      switch (code.toLowerCase()) {
        case 'invalidemail':
        case 'invalidemailformat':
        case 'userwiththisemailnotfound':
          return { email: description }; // Ошибка относится к полю email

        case 'invalidotp':
        case 'otptooshort':
        case 'otpexpired':
          return { otp: description }; // Ошибка относится к полю otp
        
        // Коды для других полей, например, при регистрации
        case 'invalidphonenumber':
          return { phoneNumber: description };

        // Если код не соответствует ни одному полю, считаем ошибку общей
        default:
          return { general: description };
      }
    }
    // --- КОНЕЦ НОВОЙ ЛОГИКИ ---

    // Оставляем старые проверки как запасные, преобразуя их в новый формат
    if (data && typeof data.errors === 'object' && data.errors !== null) {
      const errorMessages = Object.values(data.errors).flat().join(' ');
      return { general: errorMessages || 'Ошибка валидации.' };
    }
    if (data && (data.message || data.error || data.title)) {
      return { general: data.message || data.error || data.title };
    }
    if (typeof data === 'string') {
      return { general: data };
    }
    
    return { general: `Ошибка сервера: ${error.response.status}` };
  }
  
  if (error.request) {
    return { general: 'Сервер не отвечает. Проверьте ваше интернет-соединение.' };
  }

  return { general: error.message || 'Произошла непредвиденная ошибка' };
};