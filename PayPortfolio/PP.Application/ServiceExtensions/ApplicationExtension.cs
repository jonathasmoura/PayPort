using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Application.Contracts;
using PP.Application.Contracts.Services;
using PP.Application.DTOs;
using PP.Application.Utils.Validation;

namespace PP.Application.ServiceExtensions
{
	public static class ApplicationExtension
	{
		public static IServiceCollection AddDIApplicationExtensions(this IServiceCollection services, IConfiguration configuration)
		{

			services.AddScoped<IPaymentService, PaymentService>();
			services.AddTransient<IValidator<PaymentRequestDto>, PaymentRequestValidator>();

			services.AddSingleton<IProcessingService, ProcessingDelayService>();
			services.AddScoped<IPaymentBusinessRuleProcessorService, PaymentBusinessRuleProcessorService>();

			return services;
		}
	}
}
