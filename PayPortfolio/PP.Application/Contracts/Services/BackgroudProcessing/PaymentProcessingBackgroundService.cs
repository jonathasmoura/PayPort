using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PP.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services.BackgroudProcessing
{
	public class PaymentProcessingBackgroundService : BackgroundService
	{
		private readonly IPaymentProcessingQueueService _queue;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<PaymentProcessingBackgroundService> _logger;

		public PaymentProcessingBackgroundService(
			IPaymentProcessingQueueService queue,
			IServiceScopeFactory scopeFactory,
			ILogger<PaymentProcessingBackgroundService> logger)
		{
			_queue = queue;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("PaymentProcessingBackgroundService iniciado.");

			await foreach (var eventId in _queue.ReadAsync(stoppingToken))
			{
				try
				{
					// Escopo novo por mensagem: DbContext não é thread-safe/reutilizável entre chamadas concorrentes.
					using var scope = _scopeFactory.CreateScope();
					var processor = scope.ServiceProvider.GetRequiredService<IPaymentBusinessRuleProcessorService>();

					await processor.ProcessAsync(eventId, stoppingToken);
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break; // shutdown normal da aplicação
				}
				catch (Exception ex)
				{
					// Erro isolado por evento: não pode derrubar o worker inteiro, ou nenhum
					// outro evento na fila seria processado.
					_logger.LogError(ex, "Falha ao processar evento {eventId}", eventId);
				}
			}

			_logger.LogInformation("PaymentProcessingBackgroundService finalizado.");
		}
	}
}
