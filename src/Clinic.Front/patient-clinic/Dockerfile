# --- ЭТАП 1: СБОРКА (BUILDER) ---
FROM node:18-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# --- ЭТАП 2: ПРОДАКШЕН (PRODUCTION) ---
FROM nginx:stable-alpine

# Копируем только результат сборки
COPY --from=builder /app/build /usr/share/nginx/html

# Копируем и делаем исполняемым наш entrypoint-скрипт
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# [Опционально] Если нужен кастомный конфиг Nginx для роутинга
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

# Устанавливаем наш скрипт как точку входа
ENTRYPOINT ["/entrypoint.sh"]
# Команда по умолчанию, которую выполнит entrypoint
CMD ["nginx", "-g", "daemon off;"]