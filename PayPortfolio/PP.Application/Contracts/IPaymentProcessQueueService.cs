using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts
{
	public interface IPaymentProcessingQueueService
	{
		ValueTask QueueAsync(Guid eventId, CancellationToken ct = default);
		IAsyncEnumerable<Guid> ReadAsync(CancellationToken ct);
	}
}
