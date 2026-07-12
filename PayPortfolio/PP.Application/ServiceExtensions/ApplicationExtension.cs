using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Application.Contracts;
using PP.Application.Contracts.Services;
using System;

namespace PP.Application.ServiceExtensions
{
	public static class ApplicationExtension
	{
		public static IServiceCollection AddDIApplicationExtensions(this IServiceCollection services, IConfiguration configuration)
		{
			// Application services
			services.AddScoped<IPaymentService, PaymentService>();

			// Processing helper (stateless) — singleton is fine for a simple delay implementation
			services.AddSingleton<IProcessingService, ProcessingDelayService>();

			// Observação: repositórios e infra serviços (IPaymentRepository, IContractRepository, IPaymentProcessingQueueService, validadores, etc.)
			// devem ser registrados na camada de Infra/Startup para manter separação de responsabilidades.
			// Aqui registramos apenas serviços de aplicação/dominio pertencentes a este assembly.

			return services;
		}
	}
}
