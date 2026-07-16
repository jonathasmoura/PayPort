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
	public class PaymentWebHookEventMap : EntityBaseConfiguration<PaymentWebHookEvent>
	{
		public override void Configure(EntityTypeBuilder<PaymentWebHookEvent> builder)
		{
			base.Configure(builder);

			builder.ToTable("PaymentWebHookEvents");

			builder.Property(e => e.IdTransaction)
				.IsRequired()
				.HasMaxLength(100);

			builder.HasIndex(e => e.IdTransaction)
				.IsUnique()
				.HasDatabaseName("UX_PaymentWebHookEvents_IdTransaction");

			builder.Property(e => e.IdContract)
				.IsRequired()
				.HasMaxLength(100);

			builder.Property(e => e.Amount)
				.HasColumnType("decimal(18,2)");

			builder.Property(e => e.ReceivedStatus)
				.IsRequired()
				.HasMaxLength(50);

			builder.Property(e => e.PayloadRaw)
				.IsRequired()
				.HasColumnType("nvarchar(max)");

		// Map enum as integer in the database to match existing migrations/schema
		builder.Property(e => e.ProcessingStatus);

			builder.Property(e => e.ProcessingError)
				.HasMaxLength(2000);

			builder.HasIndex(e => e.IdContract)
				.HasDatabaseName("IX_PaymentWebHookEvents_IdContract");
		}
	}
}
