using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Services
{
	public sealed class ProcessingDelayService : IProcessingService
	{
		public async Task AwaitAsync(TimeSpan duration, CancellationToken ct)
		{
			await Task.Delay(duration, ct);
		}
	}
}
