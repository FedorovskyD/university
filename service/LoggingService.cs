using Serilog;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace University
{
    public class LoggingService
    {
        // Используем библиотеку Serilog для логирования
        private static readonly ILogger log = Log.ForContext<LoggingService>();

        /// <summary>
        /// Логирование добавленной сущности.
        /// </summary>
        public void LogAddedEntity(DbEntityEntry entry, string entityName)
        {
            var entityProperties = entry.CurrentValues.PropertyNames.Where(property => property != "Id").ToList();

            var entityPropertiesString = string.Join(Environment.NewLine, entityProperties.Select(property =>
               $"                              {property}: {entry.CurrentValues[property]}"));

            log.Information($"Добавлен новый объект {entityName}.\n{entityPropertiesString}");
        }

        /// <summary>
        /// Логирование измененной сущности.
        /// </summary>
        public void LogModifiedEntity(DbEntityEntry entry, string entityName)
        {
            var entityId = entry.OriginalValues["Id"];

            var modifiedProperties = entry.OriginalValues.PropertyNames
                .Where(propertyName => !Equals(entry.OriginalValues[propertyName], entry.CurrentValues[propertyName]))
                .ToList();

            if (modifiedProperties.Any())
            {
                var modifiedPropertiesString = string.Join(Environment.NewLine, modifiedProperties.Select(property =>
                $"                              {property}: {entry.OriginalValues[property]} => {entry.CurrentValues[property]}"));

                log.Information($"Объект {entityName} с Id={entityId} был изменен.\n{modifiedPropertiesString}");
            }
        }

        /// <summary>
        /// Логирование удаленной сущности.
        /// </summary>
        public void LogDeletedEntity(DbEntityEntry entry, string entityName)
        {
            var entityId = entry.OriginalValues["Id"];

            log.Information($"Удален объект {entityName}. Id={entityId}");
        }
    }
}
