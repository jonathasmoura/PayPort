using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using PP.Infra.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.ServiceExtensions
{
	public static class InfraExtension
	{
		public static IServiceCollection AddDIInfrastuctureExtension(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<DbPayContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddScoped<DbPayContext, DbPayContext>();
			services.AddTransient<IUnitOfWork, UnitOfWork>();
			services.AddTransient<IContractRepository, ContractRepository>();
			services.AddTransient<IPaymentRepository, PaymentRepository>();

			return services;
		}
	}
}
