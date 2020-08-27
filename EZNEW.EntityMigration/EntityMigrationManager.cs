using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Query;
using EZNEW.Data;
using System.Runtime.CompilerServices;

namespace EZNEW.EntityMigration
{
    /// <summary>
    /// Entity migration manager
    /// </summary>
    public static class EntityMigrationManager
    {
        internal static readonly Dictionary<DatabaseServerType, Dictionary<Guid, string>> ColumnTypeMaps = null;

        internal static readonly Dictionary<DatabaseServerType, Dictionary<Guid, int>> ColumnMaxLength = null;

        internal static readonly Dictionary<DatabaseServerType, IMigrationModelBuilder> ModelBuilderMaps = null;

        internal static readonly IMigrationModelBuilder DefaultModelBuilder = new DefaultMigrationModelBuilder();

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
                },
                {
                    DatabaseServerType.MySQL,
                    new Dictionary<Guid, string>()
                    {
                        { typeof(bool).GUID,"bit"}
                    }
                }
            };

            ColumnMaxLength = new Dictionary<DatabaseServerType, Dictionary<Guid, int>>()
            {
                {
                    DatabaseServerType.MySQL,
                    new Dictionary<Guid, int>()
                    {
                        { typeof(string).GUID,50}
                    }
                },
                {
                    DatabaseServerType.SQLServer,
                    new Dictionary<Guid, int>()
                    {
                        { typeof(string).GUID,50}
                    }
                },
                {
                    DatabaseServerType.Oracle,
                    new Dictionary<Guid, int>()
                    {
                        { typeof(string).GUID,50}
                    }
                }
            };
            ModelBuilderMaps = new Dictionary<DatabaseServerType, IMigrationModelBuilder>()
            {
                { DatabaseServerType.Oracle,new UppercaseMigrationModelBuilder()}
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

        #region Column maxlength

        /// <summary>
        /// Configure maxlength
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="columnMaxLengths">Column maxlengths</param>
        public static void ConfigureMaxLength(DatabaseServerType databaseServerType, IEnumerable<KeyValuePair<Guid, int>> columnMaxLengths)
        {
            if (columnMaxLengths.IsNullOrEmpty())
            {
                return;
            }
            if (ColumnMaxLength.TryGetValue(databaseServerType, out var typeMaxLengths))
            {
                foreach (var maxLengthItem in columnMaxLengths)
                {
                    typeMaxLengths[maxLengthItem.Key] = maxLengthItem.Value;
                }
            }
            else
            {
                ColumnMaxLength[databaseServerType] = columnMaxLengths.ToDictionary(c => c.Key, c => c.Value);
            }
        }

        /// <summary>
        /// Configure maxlength
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="clrType">Column type</param>
        /// <param name="maxLength">Maxlength</param>
        public static void ConfigureMaxLength(DatabaseServerType databaseServerType, Type clrType, int maxLength)
        {
            if (clrType == null)
            {
                return;
            }
            ConfigureMaxLength(databaseServerType, new Dictionary<Guid, int>(1)
            {
                { clrType.GUID,maxLength}
            });
        }

        /// <summary>
        /// Get column maxlength
        /// </summary>
        /// <param name="databaseServerType"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static int GetColumnMaxLength(DatabaseServerType databaseServerType, Type clrType)
        {
            if (clrType == null)
            {
                return -1;
            }
            if (ColumnMaxLength.TryGetValue(databaseServerType, out var maxLengths) && maxLengths != null && maxLengths.TryGetValue(clrType.GUID, out var maxLength))
            {
                return maxLength;
            }
            return -1;
        }

        #endregion

        #region Model builder

        /// <summary>
        /// Model builder
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="migrationModelBuilder">Model builder</param>
        public static void ConfigureModelBuilder(DatabaseServerType databaseServerType, IMigrationModelBuilder migrationModelBuilder)
        {
            ModelBuilderMaps[databaseServerType] = migrationModelBuilder;
        }

        /// <summary>
        /// Get model builder
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <returns>Return IMigrationModelBuilder</returns>
        public static IMigrationModelBuilder GetModelBuilder(DatabaseServerType databaseServerType)
        {
            ModelBuilderMaps.TryGetValue(databaseServerType, out var modelBuilder);
            return modelBuilder ?? DefaultModelBuilder;
        }

        #endregion
    }
}
