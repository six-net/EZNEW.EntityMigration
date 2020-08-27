using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EZNEW.Develop.Entity;
using EZNEW.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.EntityMigration
{
    public class EntityMigrationContext : DbContext
    {
        /// <summary>
        /// Database server
        /// </summary>
        protected DatabaseServer DatabaseServer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (string.IsNullOrWhiteSpace(DatabaseServer?.ConnectionString))
            {
                throw new Exception($"{nameof(DatabaseServer)}.{nameof(DatabaseServer.ConnectionString)} is null or empty");
            }
            var migrationModelBuilder = EntityMigrationManager.GetModelBuilder(DatabaseServer.ServerType);
            migrationModelBuilder?.CreateModel(DatabaseServer.ServerType, modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
