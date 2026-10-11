#!/bin/sh
# Прекратить выполнение при любой ошибке
set -e 

# Путь к файлу config.js внутри контейнера Nginx
CONFIG_JS_PATH=/usr/share/nginx/html/config.js

echo "Генерация config.js на основе переменных окружения..."

# Перезаписываем файл config.js
# Используем значение REACT_APP_API_URL, а если оно не задано,
# подставляем URL по умолчанию для безопасности.
echo "window.config = {" > ${CONFIG_JS_PATH}
echo "  API_URL: '${REACT_APP_API_URL:-http://api.default.url}'" >> ${CONFIG_JS_PATH}
# Сюда можно добавлять другие переменные, если они понадобятся
# echo "  FEATURE_FLAG: '${FEATURE_FLAG:-false}'," >> ${CONFIG_JS_PATH}
echo "};" >> ${CONFIG_JS_PATH}

echo "Файл config.js успешно создан:"
cat ${CONFIG_JS_PATH} # Выводим содержимое для отладки в логах

# Эта строка ОЧЕНЬ ВАЖНА!
# Она запускает основной процесс, который был указан в CMD Dockerfile (т.е. Nginx)
exec "$@"