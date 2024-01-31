using Serilog;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using University.Properties;

namespace University.model
{
    class UniversityContext : DbContext
    {
        private readonly LoggingService loggingService = new LoggingService();
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public UniversityContext() : base(Settings.Default.DbConnection)
        {
            
        }

        public override int SaveChanges()
        {
            LogChanges();
            return base.SaveChanges();
        }

        private void LogChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                var entityName = entry.Entity.GetType().Name;

                switch (entry.State)
                {
                    case EntityState.Added:
                        loggingService.LogAddedEntity(entry, entityName);
                        break;

                    case EntityState.Modified:
                        loggingService.LogModifiedEntity(entry, entityName);
                        break;

                    case EntityState.Deleted:
                        loggingService.LogDeletedEntity(entry, entityName);
                        break;
                }
            }
        }
    }
}
