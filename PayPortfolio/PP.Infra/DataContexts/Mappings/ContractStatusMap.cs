using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.DataContexts.Mappings
{	
	public class ContractStatusMap : EntityBaseConfiguration<ContractStatus>
	{
		public override void Configure(EntityTypeBuilder<ContractStatus> builder)
		{
			base.Configure(builder);

			builder.ToTable("ContractsStatus");

			builder.Property(c => c.IdContract)
				.IsRequired()
				.HasMaxLength(100);

			builder.HasIndex(c => c.IdContract)
				.IsUnique()
				.HasDatabaseName("UX_ContractsStatus_IdContract");

			builder.Property(c => c.CurrentStatus)
				.IsRequired()
				.HasMaxLength(50);

			builder.Property(c => c.AmountPaid)
				.HasColumnType("decimal(18,2)");

			builder.Property(c => c.LastTransactionId)
				.IsRequired()
				.HasMaxLength(100);
		}
	}
}
