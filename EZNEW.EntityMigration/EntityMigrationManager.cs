using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using EZNEW.Data;

namespace EZNEW.EntityMigration
{
    /// <summary>
    /// Entity migration manager
    /// </summary>
    public static class EntityMigrationManager
    {
        internal static readonly Dictionary<DatabaseServerType, Dictionary<Guid, string>> ColumnTypeMaps = new Dictionary<DatabaseServerType, Dictionary<Guid, string>>();

        public const string MigrationCommandObjectName = "EZNEWMigration";

        static EntityMigrationManager()
        {
            ColumnTypeMaps = new Dictionary<DatabaseServerType, Dictionary<Guid, string>>()
            {
                {
                    DatabaseServerType.SQLServer,
                    new Dictionary<Guid, string>()
                    {
                        { typeof(string).GUID,"varchar(50)"}
                    }
                }
            };
        }

        #region Column type

        /// <summary>
        /// Configure column type
        /// </summary>
        /// <param name="databaseServerType">Database type</param>
        /// <param name="clrType">Clr type</param>
        /// <param name="dbTypeName">Database type name</param>
        public static void ConfigureColumnType(DatabaseServerType databaseServerType, Type clrType, string dbTypeName)
        {
            if (clrType == null)
            {
                return;
            }
            ConfigureColumnType(databaseServerType, new Dictionary<Guid, string>()
            {
                { clrType.GUID,dbTypeName}
            });
        }

        /// <summary>
        /// Configure column type
        /// </summary>
        /// <param name="databaseServerType">Database type</param>
        /// <param name="columnTypeMaps">Column type maps</param>
        public static void ConfigureColumnType(DatabaseServerType databaseServerType, IEnumerable<KeyValuePair<Guid, string>> columnTypeMaps)
        {
            if (columnTypeMaps.IsNullOrEmpty())
            {
                return;
            }
            if (ColumnTypeMaps.TryGetValue(databaseServerType, out var typeMaps))
            {
                foreach (var mapItem in columnTypeMaps)
                {
                    typeMaps[mapItem.Key] = mapItem.Value;
                }
            }
            else
            {
                ColumnTypeMaps[databaseServerType] = columnTypeMaps.ToDictionary(c => c.Key, c => c.Value);
            }
        }

        /// <summary>
        /// Get column type name
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="clrType">Clr type</param>
        public static string GetColumnTypeName(DatabaseServerType databaseServerType, Type clrType)
        {
            if (clrType == null)
            {
                return string.Empty;
            }
            if (ColumnTypeMaps.TryGetValue(databaseServerType, out var typeMaps) && typeMaps != null && typeMaps.TryGetValue(clrType.GUID, out var typeName))
            {
                return typeName;
            }
            return string.Empty;
        }

        #endregion
    }
}
