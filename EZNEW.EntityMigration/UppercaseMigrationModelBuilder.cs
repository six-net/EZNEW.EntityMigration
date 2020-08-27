using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EZNEW.Data;
using EZNEW.Develop.Entity;
using System.Linq;

namespace EZNEW.EntityMigration
{
    public class UppercaseMigrationModelBuilder : IMigrationModelBuilder
    {
        public ModelBuilder CreateModel(DatabaseServerType databaseServerType, ModelBuilder modelBuilder)
        {
            var entityConfigurations = EntityManager.GetAllEntityConfigurations();
            foreach (var entityCfg in entityConfigurations)
            {
                string tableName = entityCfg.TableName.ToUpper();
                var entityBuilder = modelBuilder.Entity(entityCfg.EntityType)
                    .ToTable(tableName)
                    .HasAnnotation(RelationalAnnotationNames.Prefix + "Comment", entityCfg.Comment ?? entityCfg.TableName);
                List<string> primaryKeys = new List<string>();
                foreach (var field in entityCfg.AllFields.Values)
                {
                    var propertyBuilder = entityBuilder.Property(field.DataType, field.FieldName)
                        .HasAnnotation(RelationalAnnotationNames.Prefix + "Comment", field.Comment ?? field.PropertyName)
                        .HasColumnName(field.PropertyName.ToUpper());

                    //column type
                    var columnTypeName = string.IsNullOrWhiteSpace(field.DbTypeName) ? EntityMigrationManager.GetColumnTypeName(databaseServerType, field.DataType) : field.DbTypeName;
                    if (!string.IsNullOrWhiteSpace(columnTypeName))
                    {
                        propertyBuilder = propertyBuilder.HasColumnType(columnTypeName);
                    }
                    //maxlength
                    var maxLength = field.MaxLength;
                    if (maxLength < 1)
                    {
                        maxLength = EntityMigrationManager.GetColumnMaxLength(databaseServerType, field.DataType);
                    }
                    if (maxLength > 0)
                    {
                        propertyBuilder = propertyBuilder.HasMaxLength(maxLength);
                    }
                    //allow null
                    propertyBuilder = propertyBuilder.IsRequired(field.IsRequired || !field.DataType.AllowNull());
                    //fixed length
                    propertyBuilder = propertyBuilder.IsFixedLength(field.IsFixedLength);
                    if (field.IsPrimaryKey)
                    {
                        primaryKeys.Add(field.FieldName);
                    }
                    //auto increment
                    propertyBuilder.ValueGeneratedNever();
                };
                if (primaryKeys.Count > 0)
                {
                    var keyNames = $"{entityCfg.TableName}_{string.Join("_", primaryKeys)}".Take(30);
                    entityBuilder.HasKey(primaryKeys.ToArray()).HasName(string.Join("", keyNames));
                }
            }
            return modelBuilder;
        }
    }
}
