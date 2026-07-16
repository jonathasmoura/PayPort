using Microsoft.OpenApi.Models;
using PP.Infra.ServiceExtensions;
using PP.Application.ServiceExtensions;
using PP.API.Middleware;
using PP.API.ServicesNotifier;
using PP.Application.Contracts.Services.BackgroudProcessing;
using PP.Application.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDIInfrastuctureExtension(builder.Configuration)
	.AddDIApplicationExtensions(builder.Configuration);

builder.Services.AddHostedService<PP.Application.Contracts.Services.BackgroudProcessing.PaymentProcessingBackgroundService>();

builder.Services.AddSingleton<IWebSocketNotifier, WebSocketNotifier>();

builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
	{
		Type = SecuritySchemeType.ApiKey,
		In = ParameterLocation.Header,
		Name = "X-Api-Key",
		Description = "ApiKey para validar webhooks"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
				In = ParameterLocation.Header
			},
			Array.Empty<string>()
		}
	});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll",
		policy =>
		{
			policy.AllowAnyOrigin()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
	context.Request.EnableBuffering();
	await next();
});

app.UseWebSockets();

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Map("/ws/notifications", async context =>
{
	if (!context.WebSockets.IsWebSocketRequest)
	{
		context.Response.StatusCode = StatusCodes.Status400BadRequest;
		return;
	}

	var notifier = context.RequestServices.GetRequiredService<IWebSocketNotifier>();
	using var ctSource = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
	var socket = await context.WebSockets.AcceptWebSocketAsync();
	await notifier.RegisterSocketAsync(socket, ctSource.Token);
});

app.Run();