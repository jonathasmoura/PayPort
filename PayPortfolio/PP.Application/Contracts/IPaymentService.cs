using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PP.Application.Contracts
{
	public interface IPaymentService
	{
		Task<PaymentResponseDto> ReceiveAsync(PaymentRequestDto request, string payloadRaw, CancellationToken ct = default);

		Task<IReadOnlyList<ContractStatusDto>> ListContractStatusesAsync(CancellationToken ct = default);

		Task<IReadOnlyList<EventLogDto>> ListLatestEventsAsync(int quantity, CancellationToken ct = default);
	}
}
