using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using PP.Infra.DataContexts.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.DataContexts
{
	public class DbPayContext : DbContext
	{
		public DbPayContext(DbContextOptions<DbPayContext> options)
			: base(options) { }
		public DbSet<PaymentWebHookEvent> PaymentWebHookEvents { get; set; }
		public DbSet<ContractStatus> ContractsStatus { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new PaymentWebHookEventMap());
			modelBuilder.ApplyConfiguration(new ContractStatusMap());
		}
	}
}
