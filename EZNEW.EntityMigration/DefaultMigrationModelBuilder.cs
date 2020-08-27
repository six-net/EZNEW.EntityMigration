using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using EZNEW.Develop.Entity;
using EZNEW.Data;

namespace EZNEW.EntityMigration
{
    public class DefaultMigrationModelBuilder : IMigrationModelBuilder
    {
        public ModelBuilder CreateModel(DatabaseServerType databaseServerType, ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                return null;
            }
            var entityConfigurations = EntityManager.GetAllEntityConfigurations();
            foreach (var entityCfg in entityConfigurations)
            {
                var entityBuilder = modelBuilder.Entity(entityCfg.EntityType)
                    .ToTable(entityCfg.TableName)
                    .HasAnnotation(RelationalAnnotationNames.Prefix + "Comment", entityCfg.Comment ?? entityCfg.TableName);
                List<string> primaryKeys = new List<string>();
                foreach (var field in entityCfg.AllFields.Values)
                {
                    var propertyBuilder = entityBuilder.Property(field.DataType, field.FieldName)
                        .HasAnnotation(RelationalAnnotationNames.Prefix + "Comment", field.Comment ?? field.PropertyName);

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
                    var keyBuilder = entityBuilder.HasKey(primaryKeys.ToArray());
                }
            }
            return modelBuilder;
        }
    }
}
