using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts
{
	public interface IPaymentService
	{
		Task<PaymentResponseDto> ReceberAsync(PaymentRequestDto request, string payloadRaw, CancellationToken ct = default);

		Task<IReadOnlyList<ContractStatusDto>> ListarStatusContratosAsync(CancellationToken ct = default);

		Task<IReadOnlyList<EventLogDto>> ListarUltimosEventosAsync(int quantidade, CancellationToken ct = default);
	}
}
