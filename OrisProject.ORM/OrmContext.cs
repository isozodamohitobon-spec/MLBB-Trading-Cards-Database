
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using OrisProject.ORM.Interfaces;

namespace OrisProject.ORM
{
    public class OrmContext : IOrmContext
    {
        private readonly string _connectionString;

        public OrmContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Проверяем названия таблиц и столбцов
        private static string ValidateIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                !Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                throw new ArgumentException("Некорректное имя таблицы или столбца.");
            }

            return "\"" + name + "\"";
        }

        // Получаем имя столбца из атрибута Column
        private static string GetColumnName(PropertyInfo property)
        {
            ColumnAttribute? attribute =
                property.GetCustomAttribute<ColumnAttribute>();

            return attribute?.Name ?? property.Name;
        }

        // Проверяем, нужно ли использовать свойство в ORM
        private static PropertyInfo[] GetMappedProperties<T>()
        {
            return typeof(T).GetProperties()
                .Where(p =>
                    p.GetIndexParameters().Length == 0 &&
                    p.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToArray();
        }

        // READ: получение всех записей
        public async Task<List<T>> GetAllAsync<T>(
            string tableName,
            CancellationToken cancellationToken = default
        ) where T : class, new()
        {
            string table = ValidateIdentifier(tableName);

            List<T> result = new List<T>();

            PropertyInfo[] properties = GetMappedProperties<T>()
                .Where(p => p.CanWrite)
                .ToArray();

            string sql = $"SELECT * FROM {table}";

            await using NpgsqlConnection connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            await using NpgsqlDataReader reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                T entity = new T();

                foreach (PropertyInfo property in properties)
                {
                    string columnName = GetColumnName(property);

                    int ordinal = reader.GetOrdinal(columnName);

                    if (reader.IsDBNull(ordinal))
                        continue;

                    object value = reader.GetValue(ordinal);

                    Type targetType =
                        Nullable.GetUnderlyingType(property.PropertyType)
                        ?? property.PropertyType;

                    object convertedValue =
                        targetType.IsInstanceOfType(value)
                        ? value
                        : Convert.ChangeType(value, targetType);

                    property.SetValue(entity, convertedValue);
                }

                result.Add(entity);
            }

            return result;
        }

        // CREATE: добавление записи
        public async Task InsertAsync<T>(
            string tableName,
            T entity,
            CancellationToken cancellationToken = default
        ) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string table = ValidateIdentifier(tableName);

            PropertyInfo[] properties = GetMappedProperties<T>()
                .Where(p => p.CanRead && p.Name != "Id")
                .ToArray();

            if (properties.Length == 0)
                throw new ArgumentException("Нет свойств для добавления.");

            string columns = string.Join(", ",
                properties.Select(p =>
                    ValidateIdentifier(GetColumnName(p))));

            string parameters = string.Join(", ",
                properties.Select((p, i) => "@p" + i));

            string sql =
                $"INSERT INTO {table} ({columns}) VALUES ({parameters})";

            await using NpgsqlConnection connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            for (int i = 0; i < properties.Length; i++)
            {
                object value =
                    properties[i].GetValue(entity) ?? DBNull.Value;

                command.Parameters.AddWithValue("@p" + i, value);
            }

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        // UPDATE: изменение записи
        public async Task UpdateAsync<T>(
            string tableName,
            int id,
            T entity,
            CancellationToken cancellationToken = default
        ) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            string table = ValidateIdentifier(tableName);

            PropertyInfo[] properties = GetMappedProperties<T>()
                .Where(p => p.CanRead && p.Name != "Id")
                .ToArray();

            if (properties.Length == 0)
                throw new ArgumentException("Нет свойств для изменения.");

            string setClause = string.Join(", ",
                properties.Select((p, i) =>
                    ValidateIdentifier(GetColumnName(p)) + " = @p" + i));

            string sql =
                $"UPDATE {table} SET {setClause} WHERE \"id\" = @id";

            await using NpgsqlConnection connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            for (int i = 0; i < properties.Length; i++)
            {
                object value =
                    properties[i].GetValue(entity) ?? DBNull.Value;

                command.Parameters.AddWithValue("@p" + i, value);
            }

            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        // DELETE: удаление записи
        public async Task DeleteAsync(
            string tableName,
            int id,
            CancellationToken cancellationToken = default)
        {
            string table = ValidateIdentifier(tableName);

            string sql =
                $"DELETE FROM {table} WHERE \"id\" = @id";

            await using NpgsqlConnection connection =
                new NpgsqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using NpgsqlCommand command =
                new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
