using Microsoft.Extensions.Logging;
using PP.Application.DTOs;
using PP.Application.Utils.Validation;
using PP.Domain.Entities;
using PP.Domain.Exceptions;
using PP.Domain.Interfaces;
using System;
using System.Collections;
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

		public async Task<IReadOnlyList<EventLogDto>> ListLatestEventsAsync(int quantity, CancellationToken ct = default)
		{
			var events = await _paymentRepository.ListLatestsAsync(quantity, ct);
			return events.Select(e => new EventLogDto
			{
				Id = e.Id,
				IdTransaction = e.IdTransaction,
				IdContract = e.IdContract,
				Amount = e.Amount,
				ReceivedStatus = e.ReceivedStatus,
				ProcessingStatus = e.ProcessingStatus.ToString(),
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
				_logger.LogWarning(
					"Payload inválido para id_transaction '{IdTransaction}': {Errors}",
					request.IdTransaction, string.Join(" | ", validation.Errors));

				return new PaymentResponseDto
				{
					Result = EReceivedResultDto.InvalidData,
					Message = "Um ou mais campos do payload são inválidos.",
					Errors = validation.Errors
				};
			}

			// 1) Checagem otimista (rápida, via índice) — evita trabalho desnecessário no
			//    caminho feliz, que é o mais comum (reenvio de rede é a exceção, não a regra).
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
				// 2) Persistência do log bruto — protegida por constraint UNIQUE no banco,
				//    que é quem de fato garante a idempotência sob concorrência.
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

			// 3) Resposta rápida ao banco: o processamento pesado roda em background,
			//    fora do ciclo de vida desta requisição HTTP.
			await _paymentProcessingQueue.QueueAsync(evento.Id, ct);

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
