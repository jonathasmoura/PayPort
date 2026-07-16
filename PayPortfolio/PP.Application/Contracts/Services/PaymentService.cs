using Microsoft.Extensions.Logging;
using PP.Application.DTOs;
using PP.Application.Utils.Validation;
using PP.Domain.Entities;
using PP.Domain.Exceptions;
using PP.Domain.Interfaces;
using PP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;	
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IValidator<PaymentRequestDto> _validator;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IPaymentProcessingQueueService _paymentProcessingQueue;
		private readonly IContractRepository _contractRepository;
		private readonly ILogger<PaymentService> _logger;

		public PaymentService(IValidator<PaymentRequestDto> validator, ILogger<PaymentService> logger, IPaymentRepository paymentRepository, IPaymentProcessingQueueService paymentProcessingQueue, IContractRepository contractRepository)
		{
			_validator = validator;
			_logger = logger;
			_paymentRepository = paymentRepository;
			_paymentProcessingQueue = paymentProcessingQueue;
			_contractRepository = contractRepository;
		}


		public async Task<IReadOnlyList<EventLogDto>> ListLatestEventsAsync(int quantity, string? statusFilter = null, CancellationToken ct = default)
		{
			EProcessingStatus? processingFilter = null;
			if (!string.IsNullOrWhiteSpace(statusFilter))
			{
				var s = statusFilter.Trim();
				if (string.Equals(s, "Processed", StringComparison.OrdinalIgnoreCase))
				{
					processingFilter = EProcessingStatus.Processed;
				}
				else if (string.Equals(s, "Failed", StringComparison.OrdinalIgnoreCase))
				{
					processingFilter = EProcessingStatus.Failed;
				}
				else
				{
					processingFilter = null;
				}
			}

			var events = await _paymentRepository.ListLatestsAsync(quantity, processingFilter, ct);
			return events.Select(e => new EventLogDto
			{
				Id = e.Id,
				IdTransaction = e.IdTransaction,
				IdContract = e.IdContract,
				Amount = e.Amount,
				ReceivedStatus = e.ReceivedStatus,
				ProcessingStatus = e.ProcessingStatus?.ToString() ?? string.Empty,
				IsActive = e.IsActive,
				ReceivedAt = e.Created,
				ProcessedAt = e.ProcessedAt,
				ProcessingError = e.ProcessingError
			}).ToList();
		}

		public async Task<PaymentResponseDto> ReceiveAsync(PaymentRequestDto request, string payloadRaw, CancellationToken ct = default)
		{
			var validation = _validator.Validate(request);
			if (!validation.IsValid)
			{
				var errors = string.Join(" | ", validation.Errors);
				_logger.LogWarning(
					"Payload inválido para id_transaction '{IdTransaction}': {Errors}",
					request.IdTransaction, errors);

				var failedEvent = PaymentWebHookEvent.CreateFailed(
					request.IdTransaction, request.IdContract, request.Amount, request.PaymentDate, request.Status, payloadRaw, errors);

			try
			{
				await _paymentRepository.IncludeAsync(failedEvent, ct);
				_logger.LogInformation("Evento de webhook inválido salvo como Failed. EventId: {EventId}", failedEvent.Id);
			}
			catch (DuplicateTransactionException)
			{
				_logger.LogInformation("Corrida detectada ao salvar falha: transação {IdTransaction} duplicada no INSERT.", request.IdTransaction);
				return new PaymentResponseDto
				{
					Result = EReceivedResultDto.AlreadyProcessed,
					Message = "Evento já recebido anteriormente (concorrência detectada)."
				};
			}

				return new PaymentResponseDto
				{
					Result = EReceivedResultDto.InvalidData,
					Message = "Um ou mais campos do payload são inválidos.",
					Errors = validation.Errors
				};
			}

			var isExists = await _paymentRepository.ExistsTransactionAsync(request.IdTransaction, ct);
			if (isExists)
			{
				_logger.LogInformation("Transação {IdTransaction} já registrada. Ignorando reenvio.", request.IdTransaction);
				return new PaymentResponseDto
				{
					Result = EReceivedResultDto.AlreadyProcessed,
					Message = "Evento já recebido anteriormente. Nenhuma ação adicional necessária."
				};
			}

			var evento = PaymentWebHookEvent.Create(
				request.IdTransaction, request.IdContract, request.Amount, request.PaymentDate, request.Status, payloadRaw);

			try
			{
				await _paymentRepository.IncludeAsync(evento, ct);
			}
			catch (DuplicateTransactionException)
			{
				_logger.LogInformation("Corrida detectada: transação {IdTransaction} duplicada no INSERT.", request.IdTransaction);
				return new PaymentResponseDto
				{
					Result = EReceivedResultDto.AlreadyProcessed,
					Message = "Evento já recebido anteriormente (concorrência detectada)."
				};
			}

			await _paymentProcessingQueue.QueueAsync(evento.Id, CancellationToken.None);

			return new PaymentResponseDto
			{
				Result = EReceivedResultDto.AcceptedForProcessing,
				EventId = evento.Id,
				Message = "Evento recebido e enfileirado para processamento."
			};
		}

		public async Task<IReadOnlyList<ContractStatusDto>> ListContractStatusesAsync(CancellationToken ct = default)
		{
			var contracts = await _contractRepository.GetAllAsync(ct);
			return contracts.Select(c => new ContractStatusDto
			{
				IdContract = c.IdContract,
				CurrentStatus = c.CurrentStatus,
				AmountPaid = c.AmountPaid,
				LastPaymentDate = c.LastPaymentDate,
				LastTransactionId = c.LastTransactionId,
				IsActive = c.IsActive,
				UpdatedAt = c.UpdatedAt
			}).ToList();
		}
	}
}
