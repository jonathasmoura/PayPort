using Microsoft.AspNetCore.Mvc;
using PP.Application.Contracts;
using PP.Application.DTOs;
using System.Text;

namespace PP.API.Controllers
{
	[ApiController]
	[Route("webhooks")]
	public class WebhooksController : ControllerBase
	{
		private readonly IPaymentService _service;
		private readonly ILogger<WebhooksController> _logger;

		public WebhooksController(IPaymentService service, ILogger<WebhooksController> logger)
		{
			_service = service;
			_logger = logger;
		}

		[HttpPost("pagamento")]
		public async Task<IActionResult> ReceivePayment(
			[FromBody] PaymentRequestDto request, CancellationToken ct)
		{
			Request.Body.Position = 0;
			using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
			var payloadRaw = await reader.ReadToEndAsync(ct);

			Request.Body.Position = 0;
			try
			{
				var result = await _service.ReceiveAsync(request, payloadRaw, ct);

				if (result.Result == EReceivedResultDto.InvalidData)
				{
					_logger.LogWarning("Webhook de pagamento inválido. Message: {Message}. Errors: {Errors}. Payload: {Payload}", result.Message, string.Join(';', result.Errors), payloadRaw);
					return BadRequest(result);
				}

				if (result.Result == EReceivedResultDto.AlreadyProcessed)
				{
					_logger.LogInformation("Webhook já processado. EventId: {EventId}. Message: {Message}", result.EventId, result.Message);
					return Ok(result);
				}

				_logger.LogInformation("Webhook recebido e aceito para processamento. EventId: {EventId}. Message: {Message}", result.EventId, result.Message);
				return Accepted(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao processar webhook de pagamento. Payload: {Payload}", payloadRaw);
				return StatusCode(500, new PaymentResponseDto { Result = EReceivedResultDto.InvalidData, Message = "Erro interno ao processar webhook." });
			}
		}

		[HttpGet("contracts/status")]
		public async Task<IActionResult> ListContractStatus(CancellationToken ct)
		{
			var contracts = await _service.ListContractStatusesAsync(ct);
			return Ok(contracts);
		}

		[HttpGet("events")]
		public async Task<IActionResult> ListLatestEvents([FromQuery] int quantity, [FromQuery] string? status, CancellationToken ct)
		{
			var eventos = await _service.ListLatestEventsAsync(quantity <= 0 ? 50 : quantity, status, ct);
			return Ok(eventos);
		}
	}
}

