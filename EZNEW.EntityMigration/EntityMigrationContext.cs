using EZNEW.Application;
using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EZNEW.Configuration;
using EZNEW.Develop.Entity;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using EZNEW.ValueType;
using EZNEW.Data;

namespace EZNEW.EntityMigration
{
    public class EntityMigrationContext : DbContext
    {
        /// <summary>
        /// Database type
        /// </summary>
        protected DatabaseServerType DatabaseType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityConfigurations = EntityManager.GetAllEntityConfigurations();
            foreach (var entityCfg in entityConfigurations)
            {
                var entityBuilder = modelBuilder.Entity(entityCfg.EntityType)
                    .ToTable(entityCfg.TableName)
                    .HasComment(entityCfg.Comment);
                foreach (var field in entityCfg.AllFields.Values)
                {
                    var propertyBuilder = entityBuilder.Property(field.DataType, field.FieldName)
                        .HasComment(field.Comment);

                    //column type
                    var columnTypeName = string.IsNullOrWhiteSpace(field.DbTypeName) ? EntityMigrationManager.GetColumnTypeName(DatabaseType, field.DataType) : field.DbTypeName;
                    if (!string.IsNullOrWhiteSpace(columnTypeName))
                    {
                        propertyBuilder = propertyBuilder.HasColumnType(columnTypeName);
                    }
                    //maxlength
                    if (field.MaxLength > 0)
                    {
                        propertyBuilder = propertyBuilder.HasMaxLength(field.MaxLength);
                    }
                    //allow null
                    propertyBuilder = propertyBuilder.IsRequired(field.IsRequired || !field.DataType.AllowNull());
                    //fixed length
                    propertyBuilder = propertyBuilder.IsFixedLength(field.IsFixedLength);
                    if (field.IsPrimaryKey)
                    {
                        entityBuilder.HasKey(field.FieldName);
                    }
                };
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
