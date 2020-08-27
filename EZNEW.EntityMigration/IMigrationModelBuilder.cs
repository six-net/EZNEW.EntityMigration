using EZNEW.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.EntityMigration
{
    /// <summary>
    /// Migration model builder
    /// </summary>
    public interface IMigrationModelBuilder
    {
        /// <summary>
        /// Create model
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="modelBuilder">Model builder</param>
        /// <returns></returns>
        ModelBuilder CreateModel(DatabaseServerType databaseServerType, ModelBuilder modelBuilder);
    }
}
