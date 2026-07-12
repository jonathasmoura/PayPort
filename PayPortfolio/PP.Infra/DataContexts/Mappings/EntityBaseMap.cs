using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PP.Domain.Entities.EntityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.DataContexts.Mappings
{	
	public abstract class EntityBaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
	   where TEntity : Base
	{
		public virtual void Configure(EntityTypeBuilder<TEntity> builder)
		{
			builder.HasKey(e => e.Id);

			builder.Property(e => e.Id)
				.ValueGeneratedNever();

			builder.Property(e => e.IsActive)
				.IsRequired()
				.HasDefaultValue(true);

			builder.Property(e => e.ActivationDate);
			builder.Property(e => e.InactivationDate);
			builder.Property(e => e.UpdatedAt);

			builder.Property(e => e.Created)
				.IsRequired();

			builder.HasQueryFilter(e => e.IsActive);

			builder.HasIndex(e => e.IsActive)
				.HasDatabaseName($"IX_{typeof(TEntity).Name}_IsActive");
		}
	}
}
