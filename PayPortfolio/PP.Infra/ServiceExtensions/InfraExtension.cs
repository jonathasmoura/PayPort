using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using PP.Infra.Repositories;
using System;

namespace PP.Infra.ServiceExtensions
{
	public static class InfraExtension
	{
		public static IServiceCollection AddDIInfrastuctureExtension(this IServiceCollection services, IConfiguration configuration)
		{
			if (services is null) throw new ArgumentNullException(nameof(services));
			if (configuration is null) throw new ArgumentNullException(nameof(configuration));

			services.AddDbContext<DbPayContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddTransient<IUnitOfWork, UnitOfWork>();
			services.AddTransient<IContractRepository, ContractRepository>();
			services.AddTransient<IPaymentRepository, PaymentRepository>();


			return services;
		}
	}
}
