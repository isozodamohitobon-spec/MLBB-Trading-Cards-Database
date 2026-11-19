
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Копируем файлы проектов
COPY OrisProject.csproj ./
COPY OrisProject.ORM/OrisProject.ORM.csproj ./OrisProject.ORM/

# Восстанавливаем зависимости
RUN dotnet restore OrisProject.csproj

# Копируем остальные файлы
COPY . .

# Собираем приложение
RUN dotnet publish OrisProject.csproj -c Release -o /app/out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "OrisProject.dll"]
