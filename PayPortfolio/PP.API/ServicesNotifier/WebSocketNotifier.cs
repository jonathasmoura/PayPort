using PP.Application.Contracts;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace PP.API.ServicesNotifier
{
	public class WebSocketNotifier : IWebSocketNotifier
	{
		private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
		private readonly ILogger<WebSocketNotifier> _logger;

		public WebSocketNotifier(ILogger<WebSocketNotifier> logger)
		{
			_logger = logger;
		}

		public async Task RegisterSocketAsync(WebSocket socket, CancellationToken ct = default)
		{
			var id = Guid.NewGuid().ToString("N");
			_sockets.TryAdd(id, socket);
			_logger.LogInformation("WebSocket conectado: {Id}. Clientes conectados: {Count}", id, _sockets.Count);

			var buffer = new byte[1024 * 4];

			try
			{
				while (socket.State == WebSocketState.Open && !ct.IsCancellationRequested)
				{

					var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken: ct);
					if (result.MessageType == WebSocketMessageType.Close)
					{
						break;
					}
				}
			}
			catch (OperationCanceledException) { /* cancellation requested */ }
			catch (WebSocketException ex)
			{
				_logger.LogDebug(ex, "WebSocket exception para cliente {Id}", id);
			}
			finally
			{
				_sockets.TryRemove(id, out _);
				try { await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed by server", CancellationToken.None); } catch { }
				_logger.LogInformation("WebSocket desconectado: {Id}. Clientes restantes: {Count}", id, _sockets.Count);
			}
		}

		public async Task BroadcastAsync(string message, CancellationToken ct = default)
		{
			if (_sockets.IsEmpty) return;

			var payload = Encoding.UTF8.GetBytes(message);
			var segment = new ArraySegment<byte>(payload);

			foreach (var kv in _sockets)
			{
				var socket = kv.Value;
				if (socket.State != WebSocketState.Open)
				{
					_sockets.TryRemove(kv.Key, out _);
					continue;
				}

				try
				{
					await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "Falha ao enviar mensagem para WebSocket {Id}, removendo.", kv.Key);
					_sockets.TryRemove(kv.Key, out _);
				}
			}
		}
	}
}
