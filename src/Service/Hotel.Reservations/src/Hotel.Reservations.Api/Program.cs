using FluentValidation;
using Hotel.Reservations.Api.Consumers;
using Hotel.Reservations.Api.Middleware;
using Hotel.Reservations.Api.Sagas;
using Microsoft.EntityFrameworkCore;
using Hotel.Reservations.Application.Commands.CreateReservation;
using Hotel.Reservations.Domain.Interfaces;

using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Hotel.Reservations.Infrastructure.Configuration;
using Hotel.Reservations.Infrastructure.Services;
using Hotel.Reservations.Infrastructure;
using Hotel.Reservations.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));

builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));

// RabbitMq/MassTransit deshabilitado temporalmente (requiere Docker/RabbitMQ corriendo).
// Descomentar junto con el bloque AddMassTransit de más abajo para reactivar la SAGA.
// builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
    return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
});

builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
});

builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateReservationValidator>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateReservationCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMq/MassTransit deshabilitado temporalmente (requiere Docker/RabbitMQ corriendo).
// Descomentar para reactivar la SAGA (junto con el Configure<RabbitMqOptions> de más arriba).
//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<ReservationConfirmedConsumer>();
//    x.AddConsumer<ReservationRejectedConsumer>();
//    x.AddConsumer<ReservationCancelledConsumer>();
//
//    x.AddSagaStateMachine<ReservationStateMachine, ReservationState>()
//        .InMemoryRepository();
//
//    x.UsingRabbitMq((context, cfg) =>
//    {
//        var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
//        cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.VirtualHost, h =>
//        {
//            h.Username(rabbitMqOptions.Username);
//            h.Password(rabbitMqOptions.Password);
//        });
//
//        cfg.ConfigureEndpoints(context);
//    });
//});
//builder.Services.AddMassTransitHostedService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapControllers();
app.Run();

public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new FluentValidation.ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).ToList();

            if (failures.Count != 0)
                throw new FluentValidation.ValidationException(failures);
        }

        return await next();
    }
}