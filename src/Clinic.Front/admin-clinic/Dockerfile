# --- ЭТАП 1: СБОРКА ПРИЛОЖЕНИЯ ---
FROM node:18-alpine AS builder

WORKDIR /app

# Копируем package.json и lock-файл для кэширования зависимостей
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь остальной исходный код
COPY . .

# Собираем продакшен-версию
RUN npm run build


# --- ЭТАП 2: ФИНАЛЬНЫЙ ОБРАЗ С NGINX ---
FROM nginx:stable-alpine

# Копируем только собранные статические файлы с первого этапа
COPY --from=builder /app/build /usr/share/nginx/html

# Копируем скрипт в корень контейнера
COPY entrypoint.sh /entrypoint.sh

# Делаем его исполняемым (это очень важно!)
RUN chmod +x /entrypoint.sh

# Указываем правильный, полный путь
ENTRYPOINT ["/entrypoint.sh"]
# Указываем команду по умолчанию, которая будет передана в entrypoint
CMD ["nginx", "-g", "daemon off;"]