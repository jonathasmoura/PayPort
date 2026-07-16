using PP.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class PaymentProcessingQueueRepository : IPaymentProcessingQueueService
	{
		private readonly Channel<Guid> _channel;

		public PaymentProcessingQueueRepository()
		{
			_channel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions
			{
				SingleReader = false,
				SingleWriter = false
			});
		}

		public ValueTask QueueAsync(Guid eventId, CancellationToken ct = default)
		{
			return _channel.Writer.WriteAsync(eventId, ct);
		}

		public async IAsyncEnumerable<Guid> ReadAsync([EnumeratorCancellation] CancellationToken ct)
		{
			var reader = _channel.Reader;

			while (await reader.WaitToReadAsync(ct).ConfigureAwait(false))
			{
				while (reader.TryRead(out var item))
				{
					yield return item;
				}
			}
		}
	}
}
