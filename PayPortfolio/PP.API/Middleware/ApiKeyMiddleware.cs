using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PP.API.Middleware
{
	public class ApiKeyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ApiKeyMiddleware> _logger;
		private readonly string? _expectedKey;

		public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
		{
			_next = next;
			_logger = logger;
			_expectedKey = configuration.GetValue<string>("Webhooks:ApiKey");
			if (string.IsNullOrEmpty(_expectedKey))
			{
				_logger.LogWarning("Webhooks:ApiKey não configurado. Requisições serão rejeitadas por segurança.");
			}
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var path = context.Request.Path;
			if (!path.StartsWithSegments("/webhooks", StringComparison.OrdinalIgnoreCase))
			{
				await _next(context);
				return;
			}

			if (string.IsNullOrEmpty(_expectedKey))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsJsonAsync(new { message = "ApiKey do servidor não configurada." });
				return;
			}

			if (!context.Request.Headers.TryGetValue("X-Api-Key", out var provided) || string.IsNullOrWhiteSpace(provided))
			{
				_logger.LogWarning("X-Api-Key ausente para rota de webhooks.");
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Unauthorized");
				return;
			}

			if (!StringComparer.Ordinal.Equals(provided.ToString(), _expectedKey))
			{
				_logger.LogWarning("X-Api-Key inválida para rota de webhooks.");
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Unauthorized");
				return;
			}

			await _next(context);
		}
	}
}