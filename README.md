
# MLBB Trading Cards Database (ОРИС)

Веб-приложение для хранения и просмотра коллекции физических карточек Mobile Legends: Bang Bang.

Проект разработан на C# с использованием собственного HTTP-сервера (HttpListener), собственной ORM, собственного шаблонизатора и базы данных PostgreSQL.

## Требования

- .NET 8.0 SDK
- PostgreSQL
- Docker Desktop (для запуска через Docker)

## Как запустить локально

1. Установите PostgreSQL.
2. Создайте базу данных `oris_db`.
3. Выполните SQL-скрипты из папки `SQL` для создания таблиц и заполнения базы начальными данными.
4. Установите переменную окружения `ORIS_DB_CONNECTION`.

Пример:

`Host=localhost;Port=5432;Database=oris_db;Username=postgres;Password=ВАШ_ПАРОЛЬ`

5. Откройте терминал в папке проекта.
6. Выполните:

```bash
dotnet run
```

7. Откройте в браузере:

http://localhost:8080/

## Запуск через Docker

1. Убедитесь, что PostgreSQL запущен.
2. Соберите Docker-образ:

```bash
docker build -t oris-project .
```

3. Запустите контейнер, передав строку подключения к PostgreSQL и адрес сервера:

```bash
docker run --rm -p 8080:8080 -e "ORIS_DB_CONNECTION=Host=host.docker.internal;Port=5432;Database=oris_db;Username=postgres;Password=ВАШ_ПАРОЛЬ" -e "ORIS_HTTP_PREFIX=http://*:8080/" oris-project
```

4. Откройте:

http://localhost:8080/

## Функциональность

- Просмотр коллекции карточек.
- Поиск карточек.
- Фильтрация по редкости.
- Просмотр подробной информации о карточке.
- Добавление карточек через административную панель.
- Удаление карточек.
- Хранение данных в PostgreSQL.

## Технологии

- C# / .NET 8
- HttpListener
- PostgreSQL
- Npgsql
- Собственная ORM
- Собственный шаблонизатор
- HTML, CSS, JavaScript
- NUnit для автоматического тестирования
- Docker
