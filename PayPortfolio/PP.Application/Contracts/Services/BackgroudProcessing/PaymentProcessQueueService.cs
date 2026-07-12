using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services.BackgroudProcessing
{
	public class PaymentProcessingQueueService : IPaymentProcessingQueueService
	{
		private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
			new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });

		public ValueTask QueueAsync(Guid eventoId, CancellationToken ct = default)
			=> _channel.Writer.WriteAsync(eventoId, ct);

		public IAsyncEnumerable<Guid> ReadAsync(CancellationToken ct)
			=> _channel.Reader.ReadAllAsync(ct);
	}
}
