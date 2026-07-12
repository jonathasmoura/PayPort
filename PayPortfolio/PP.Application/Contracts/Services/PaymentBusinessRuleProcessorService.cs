using PP.Domain.Entities;
using PP.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services
{
	public class PaymentBusinessRuleProcessorService : IPaymentBusinessRuleProcessorService
	{
		private static readonly TimeSpan SimulatedDuration = TimeSpan.FromSeconds(2);

		private readonly IPaymentRepository _eventRepository;
		private readonly IContractRepository _contractRepository;
		private readonly ILogger<PaymentBusinessRuleProcessorService> _logger;
		private readonly IProcessingService _proccessingDelay;

		public PaymentBusinessRuleProcessorService(IPaymentRepository eventRepository, IContractRepository contractRepository, ILogger<PaymentBusinessRuleProcessorService> logger, IProcessingService proccessingDelay)
		{
			_eventRepository = eventRepository;
			_contractRepository = contractRepository;
			_logger = logger;
			_proccessingDelay = proccessingDelay;
		}

		public async Task ProcessAsync(Guid eventId, CancellationToken ct)
		{
			var eventProccess = await _eventRepository.GetByIdAsync(eventId, ct);
			if (eventProccess is null)
			{
				_logger.LogWarning("Evento {eventId} não encontrado para processamento.", eventId);
				return;
			}

			eventProccess.CheckInProcess();

			try
			{
				await _eventRepository.UpdateAsync(eventProccess, ct);

				await _proccessingDelay.AwaitAsync(SimulatedDuration, ct);

				var contract = await _contractRepository.GetByContractAsync(eventProccess.IdContract, ct);
				if (contract is null)
				{
					contract = ContractStatus.CreateNew(
						eventProccess.IdContract, eventProccess.ReceivedStatus, eventProccess.Amount, eventProccess.PaymentDate, eventProccess.IdTransaction);
					await _contractRepository.IncludeAsync(contract, ct);
				}
				else
				{
					if (!contract.IsActive)
						contract.Activate();

					contract.UpdateWithNewPayment(
						eventProccess.ReceivedStatus, eventProccess.Amount, eventProccess.PaymentDate, eventProccess.IdTransaction);
					await _contractRepository.UpdateAsync(contract, ct);
				}

				eventProccess.CheckProcessed();
				await _eventRepository.UpdateAsync(eventProccess, ct);

				_logger.LogInformation(
					"Evento {eventId} (transação {IdTransaction}) processado com sucesso para o contrato {IdContract}.",
					eventId, eventProccess.IdTransaction, eventProccess.IdContract);
			}
			catch (Exception ex)
			{
				eventProccess.CheckFailed(ex.Message);
				await _eventRepository.UpdateAsync(eventProccess, ct);
				_logger.LogError(ex, "Falha ao aplicar regra de negócio para o evento {eventId}", eventId);
				throw;
			}
		}
	}
}
