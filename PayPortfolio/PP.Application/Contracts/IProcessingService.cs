using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts
{
	public interface IProcessingService
	{
		Task AwaitAsync(TimeSpan duration, CancellationToken ct);
	}
}
