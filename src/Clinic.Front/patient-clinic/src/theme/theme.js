// src/theme/theme.js
import { createTheme } from '@mui/material/styles';

// Создаем и экспортируем нашу кастомную тему
export const theme = createTheme({
  // 1. ПАЛИТРА (ЦВЕТА)
  // Здесь мы определяем основные цвета приложения.
  palette: {
    mode: 'light', // Тема может быть 'light' или 'dark'
    primary: {
      main: '#1976d2', // Основной синий цвет (для кнопок, ссылок, AppBara)
      light: '#42a5f5',
      dark: '#1565c0',
      contrastText: '#ffffff', // Цвет текста на основном цвете
    },
    secondary: {
      main: '#9c27b0', // Второстепенный фиолетовый цвет ( для акцентов)
      light: '#ba68c8',
      dark: '#7b1fa2',
      contrastText: '#ffffff',
    },
    background: {
      default: '#f4f6f8', // Цвет фона всей страницы
      paper: '#ffffff',   // Цвет фона для "бумажных" элементов (карточки, сайдбар, хедер)
    },
    text: {
      primary: '#121212', // Основной цвет текста
      secondary: '#65748B', // Второстепенный, более светлый цвет текста
    },
  },

  // 2. ТИПОГРАФИКА (ШРИФТЫ И ТЕКСТ)
  // Управление шрифтами, их размерами и начертанием.
  typography: {
    // Убедитесь, что вы подключили этот шрифт, например, из Google Fonts.
    fontFamily: '"Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji"',
    h4: {
      fontWeight: 600,
      fontSize: '1.75rem',
    },
    h5: {
      fontWeight: 600,
      fontSize: '1.25rem',
    },
    h6: {
      fontWeight: 600,
      fontSize: '1.1rem',
    },
  },

  // 3. ФОРМА (Скругление углов)
  // Глобальное управление скруглением углов.
  shape: {
    borderRadius: 8, // Более скругленные углы, чем по умолчанию (4)
  },

  // 4. КОМПОНЕНТЫ (Переопределение стилей)
  // Самая мощная часть. Здесь можно изменить стили по умолчанию для любого компонента MUI.
  components: {
    // Название компонента в формате Mui[ComponentName]
    MuiButton: {
      styleOverrides: {
        // 'root' - это корневой элемент компонента
        root: {
          textTransform: 'none', // Отключаем ALL CAPS на кнопках
          fontWeight: 600,
        },
        // Переопределяем стиль для конкретного варианта
        containedPrimary: {
          '&:hover': {
            backgroundColor: '#1565c0', // Делаем ховер немного темнее
          },
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          // Убираем стандартную тень и используем кастомную
          backgroundImage: 'none',
        },
      },
      // Устанавливаем пропсы по умолчанию
      defaultProps: {
        elevation: 2, // Небольшая тень по умолчанию для всех Paper
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          // Делаем хедер белым, а не синим по умолчанию
          backgroundColor: '#ffffff',
          color: '#121212', // Текст в хедере делаем темным
          boxShadow: '0 1px 4px rgba(0, 0, 0, 0.1)', // Легкая тень для хедера
        },
      },
      defaultProps: {
        elevation: 0, // Отключаем стандартную тень, т.к. задали свою
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: {
          // Убираем рамку у сайдбара
          borderRight: 'none',
          // Добавляем тень
          boxShadow: '0 1px 4px rgba(0, 0, 0, 0.1)',
        }
      }
    },
    MuiListItemButton: {
      styleOverrides: {
        root: {
          // Стиль для активных ссылок в NavLink будет применяться поверх этого
          '&:hover': {
            backgroundColor: 'rgba(0, 0, 0, 0.04)',
          },
          // Стили для активного элемента (NavLink сам добавит класс .active)
          // Однако мы использовали style-prop в NavLink, что является более надежным подходом.
          // Этот способ - альтернатива.
          // '&.active': {
          //   backgroundColor: 'rgba(25, 118, 210, 0.08)',
          //   color: '#1976d2',
          //   fontWeight: 'bold',
          // },
        },
      },
    },
  },
});