using Microsoft.EntityFrameworkCore;
using Queue.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Queue.QueryBuilder.Data
{
    public class QueryBuilderDbContext : DbContext
    {
        public QueryBuilderDbContext(DbContextOptions<QueryBuilderDbContext> options) : base(options)
        {
        }

        protected QueryBuilderDbContext()
        {

        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Database> Databases { get; set; }
        public DbSet<Schema> Schemas { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Query> Queries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region TenantDb

            var ortakModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module { Id = ortakModuleId, Name = "Ortak" });

            var tenantDbId = Guid.Parse("9300b4a1-1686-4cdb-88d8-2d55dc52f8da");
            modelBuilder.Entity<Database>().HasData(
               new Database
               {
                   Id = tenantDbId,
                   ModuleId = ortakModuleId,
                   Name = "tenant_dev",
                   Server = "nctestdb01.e-cozum.com",
                   User = "tenant_dev",
                   Password = "P5-4x/vR+"
               });

            var tenantDboSchemaId = Guid.NewGuid();
            modelBuilder.Entity<Schema>().HasData(new Schema()
            {
                Id = tenantDboSchemaId,
                DatabaseId = tenantDbId,
                Name = "dbo"
            });

            var tenantCurrencyTableId = Guid.NewGuid();
            modelBuilder.Entity<Table>().HasData(new Table
            {
                Id = tenantCurrencyTableId,
                Name = "Currencies",
                SchemaId = tenantDboSchemaId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = true,
                Name = "Id",
                TableId = tenantCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Code",
                TableId = tenantCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Name",
                TableId = tenantCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Symbol",
                TableId = tenantCurrencyTableId
            });

            #endregion

            #region NTEDb

            var nteModuleId = Guid.NewGuid();
            modelBuilder.Entity<Module>().HasData(new Module { Id = nteModuleId, Name = "NTE" });

            var nteDbId = Guid.Parse("2be20fd1-411a-4a48-bf0b-1404c5ee312d");
            modelBuilder.Entity<Database>().HasData(
                new Database
                {
                    Id = nteDbId,
                    ModuleId = nteModuleId,
                    Name = "nte_dev",
                    Server = "localhost\\SQLEXPRESS",
                    User = "sa",
                    Password = "Htc0212!"
                });

            var nteDboSchemaId = Guid.NewGuid();
            modelBuilder.Entity<Schema>().HasData(new Schema()
            {
                Id = nteDboSchemaId,
                DatabaseId = nteDbId,
                Name = "dbo"
            });

            var nteCurrencyTableId = Guid.NewGuid();
            modelBuilder.Entity<Table>().HasData(new Table
            {
                Id = nteCurrencyTableId,
                Name = "Currencies",
                SchemaId = nteDboSchemaId,
                OriginalTableId = tenantCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = true,
                Name = "Id",
                TableId = nteCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Code",
                TableId = nteCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Name",
                TableId = nteCurrencyTableId
            });

            modelBuilder.Entity<Column>().HasData(new Column
            {
                Id = Guid.NewGuid(),
                IsNumeric = false,
                Name = "Symbol",
                TableId = nteCurrencyTableId
            });

            #endregion

            #region Queries

            modelBuilder.Entity<Query>().HasData(new Query
            {
                Id = Guid.NewGuid(),
                AuditType = AuditType.Insert,
                CommandText = "INSERT INTO [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] ( {#COLUMNS#} ) VALUES ( {#VALUES#} )"
            });

            modelBuilder.Entity<Query>().HasData(new Query
            {
                Id = Guid.NewGuid(),
                AuditType = AuditType.Update,
                CommandText = "UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMNS#} WHERE {#CONDITION#}"
            });

            modelBuilder.Entity<Query>().HasData(new Query
            {
                Id = Guid.NewGuid(),
                AuditType = AuditType.SoftDelete,
                CommandText = "UPDATE [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] SET {#COLUMN#} = {#VALUE#} WHERE {#CONDITION#}"
            });

            modelBuilder.Entity<Query>().HasData(new Query
            {
                Id = Guid.NewGuid(),
                AuditType = AuditType.HardDelete,
                CommandText = "DELETE FROM [{#DATABASE#}].[{#SCHEMA#}].[{#TABLE#}] WHERE {#CONDITION#}"
            });

            #endregion
        }
    }
}
