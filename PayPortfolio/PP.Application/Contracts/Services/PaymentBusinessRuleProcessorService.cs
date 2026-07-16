using System.Text.Json;
using PP.Application.Contracts;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services
{
	public class PaymentBusinessRuleProcessorService : IPaymentBusinessRuleProcessorService
	{
		private static readonly TimeSpan SimulatedDuration = TimeSpan.FromSeconds(2);

		private readonly IPaymentRepository _paymentRepository;
		private readonly IContractRepository _contractRepository;
		private readonly ILogger<PaymentBusinessRuleProcessorService> _logger;
		private readonly IProcessingService _processingService;
		private readonly IWebSocketNotifier _webSocketNotifier;

		public PaymentBusinessRuleProcessorService(IPaymentRepository paymentRepository, IContractRepository contractRepository, ILogger<PaymentBusinessRuleProcessorService> logger, IProcessingService processingService, IWebSocketNotifier webSocketNotifier)
		{
			_paymentRepository = paymentRepository;
			_contractRepository = contractRepository;
			_logger = logger;
			_processingService = processingService;
			_webSocketNotifier = webSocketNotifier;
		}

		public async Task ProcessAsync(Guid eventId, CancellationToken ct)
		{
			var evento = await _paymentRepository.GetByIdAsync(eventId, ct);
			if (evento == null)
			{
				_logger.LogWarning("Evento {EventId} não encontrado.", eventId);
				return;
			}

			try
			{
				evento.CheckInProcess();
				await _paymentRepository.UpdateAsync(evento, ct);

				await _processingService.AwaitAsync(SimulatedDuration, ct);

				var contract = await _contractRepository.GetByContractAsync(evento.IdContract, ct);
				if (contract == null)
				{
					var created = ContractStatus.CreateNew(evento.IdContract, evento.ReceivedStatus, evento.Amount, evento.PaymentDate, evento.IdTransaction);
					await _contractRepository.IncludeAsync(created, ct);
				}
				else
				{
					contract.UpdateWithNewPayment(evento.ReceivedStatus, evento.Amount, evento.PaymentDate, evento.IdTransaction);
					await _contractRepository.UpdateAsync(contract, ct);
				}

				evento.CheckProcessed();
			await _paymentRepository.UpdateAsync(evento, ct);

				try
				{
					var payload = JsonSerializer.Serialize(new
					{
						EventId = evento.Id,
						IdTransaction = evento.IdTransaction,
						IdContract = evento.IdContract,
						ProcessingStatus = evento.ProcessingStatus?.ToString(),
						ProcessedAt = evento.ProcessedAt,
						Amount = evento.Amount,
						PaymentDate = evento.PaymentDate
					});
					await _webSocketNotifier.BroadcastAsync(payload, ct);
				}
				catch (Exception exNotify)
				{
					_logger.LogWarning(exNotify, "Falha ao notificar via WebSocket para evento {EventId}", eventId);
				}
			}
			catch (OperationCanceledException) when (ct.IsCancellationRequested)
			{
				_logger.LogInformation("Processamento do evento {EventId} cancelado pelo token.", eventId);
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao processar evento {EventId}", eventId);
				try
				{
					evento.CheckFailed(ex.Message);
					await _paymentRepository.UpdateAsync(evento, CancellationToken.None);
				}
				catch (Exception inner)
				{
					_logger.LogError(inner, "Erro ao persistir falha do evento {EventId}", eventId);
				}
			}
		}
	}
}
