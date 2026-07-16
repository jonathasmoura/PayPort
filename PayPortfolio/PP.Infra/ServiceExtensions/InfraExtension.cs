using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Application.Contracts;
using PP.Application.Contracts.Services.BackgroudProcessing;
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

			services.AddDbContext<DbPayContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddTransient<IUnitOfWork, UnitOfWork>();
			services.AddTransient<IContractRepository, ContractRepository>();
			services.AddTransient<IPaymentRepository, PaymentRepository>();
			services.AddSingleton<IPaymentProcessingQueueService, PaymentProcessingQueueRepository>();

			return services;
		}
	}
}
