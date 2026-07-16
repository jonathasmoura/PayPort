using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PP.Application.Contracts
{
	public interface IWebSocketNotifier
	{
		Task RegisterSocketAsync(WebSocket socket, CancellationToken ct = default);

		Task BroadcastAsync(string message, CancellationToken ct = default);
	}
}
