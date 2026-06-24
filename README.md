# PollySendGrid

[![NuGet](https://img.shields.io/nuget/v/PollySendGrid.svg)](https://www.nuget.org/packages/PollySendGrid/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollySendGrid.svg)](https://www.nuget.org/packages/PollySendGrid/)
[![CI](https://github.com/Swevo/PollySendGrid/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollySendGrid/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Polly v8 resilience for SendGrid** — add retry, timeout, and circuit-breaker to any email send in two lines.

```csharp
var client = new SendGridClient(apiKey);

var resilient = client.WithPolly(pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = SendGridTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(30)));

var response = await resilient.SendEmailAsync(msg);
```

## Why PollySendGrid?

SendGrid imposes rate limits and occasionally returns transient errors. Dropped emails mean missed notifications, failed password resets, and lost revenue. This library wraps the response check in Polly v8 so every transient error is retried automatically:

| Problem | Solution |
|---------|----------|
| HTTP 429 rate limit exceeded | Auto-thrown as `SendGridTransientException` and retried |
| HTTP 500 transient server error | Auto-thrown as `SendGridTransientException` and retried |
| HTTP 503 service unavailable | Auto-thrown as `SendGridTransientException` and retried |
| `HttpRequestException` network failure | Caught by `SendGridTransientErrors.IsTransient` |
| Cascading failures during an outage | Wrap with `AddCircuitBreaker` |

## Installation

```
dotnet add package PollySendGrid
dotnet add package Polly.Core
```

## Quick-start

### 1. Manual wiring

```csharp
var client = new SendGridClient(apiKey);

var resilient = client.WithPolly(p => p
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = SendGridTransientErrors.IsTransient,
    }));

var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
var response = await resilient.SendEmailAsync(msg);
```

### 2. Dependency injection

```csharp
builder.Services.AddPollySendGrid(apiKey, pipeline => pipeline
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(2),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        ShouldHandle = SendGridTransientErrors.IsTransient,
    })
    .AddTimeout(TimeSpan.FromSeconds(30)));

public class EmailService(ResilientSendGridClient client)
{
    public Task<Response> SendAsync(SendGridMessage msg, CancellationToken ct)
        => client.SendEmailAsync(msg, ct);
}
```

## Transient error reference

| Condition | Why it's transient |
|-----------|-------------------|
| `SendGridTransientException` (HTTP 429) | Rate limit — back off and retry |
| `SendGridTransientException` (HTTP 500) | Transient server error |
| `SendGridTransientException` (HTTP 503) | Service maintenance or overload |
| `HttpRequestException` | Network failure |

> **Note:** `SendGridTransientException` is thrown automatically by `ResilientSendGridClient` when the response status code is in `SendGridTransientErrors.StatusCodes`. You do not throw it yourself.

## API reference

| Member | Description |
|--------|-------------|
| `ResilientSendGridClient.Inner` | The underlying `ISendGridClient` |
| `SendEmailAsync(msg, ct)` | Sends an email through the pipeline |
| `ExecuteAsync<T>(operation, ct)` | Runs any `ISendGridClient` operation through the pipeline |
| `SendGridTransientErrors.IsTransient` | `PredicateBuilder` for 429/500/503 + `HttpRequestException` |
| `SendGridTransientErrors.StatusCodes` | `IReadOnlySet<HttpStatusCode>` — TooManyRequests, InternalServerError, ServiceUnavailable |
| `client.WithPolly(pipeline)` | Wraps `ISendGridClient` with a pre-built pipeline |
| `client.WithPolly(configure)` | Builds pipeline inline and wraps the client |
| `services.AddPollySendGrid(configure)` | DI registration (requires `ISendGridClient` in DI) |
| `services.AddPollySendGrid(apiKey, configure)` | DI registration with API key shortcut |

## Target frameworks

.NET 6 ✅ · .NET 8 ✅ · .NET 9 ✅

## Related packages

| Package | Description |
|---------|-------------|
| [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) | Polly v8 for Azure Blob Storage |
| [PollyAzureServiceBus](https://github.com/Swevo/PollyAzureServiceBus) | Polly v8 for Azure Service Bus |
| [PollyAzureKeyVault](https://github.com/Swevo/PollyAzureKeyVault) | Polly v8 for Azure Key Vault |
| [PollyAzureEventHub](https://github.com/Swevo/PollyAzureEventHub) | Polly v8 for Azure Event Hubs |
| [PollyCosmosDb](https://github.com/Swevo/PollyCosmosDb) | Polly v8 for Azure Cosmos DB |
| [PollyElasticsearch](https://github.com/Swevo/PollyElasticsearch) | Polly v8 for Elasticsearch |
| [PollyRedis](https://github.com/Swevo/PollyRedis) | Polly v8 for StackExchange.Redis |
| [PollyEFCore](https://github.com/Swevo/PollyEFCore) | Polly v8 for Entity Framework Core |
| [PollyDapper](https://github.com/Swevo/PollyDapper) | Polly v8 for Dapper |
| [PollyMongo](https://github.com/Swevo/PollyMongo) | Polly v8 for MongoDB |
| [PollyNpgsql](https://github.com/Swevo/PollyNpgsql) | Polly v8 for Npgsql (PostgreSQL) |
| [PollySqlClient](https://github.com/Swevo/PollySqlClient) | Polly v8 for Microsoft.Data.SqlClient |
| [PollyGrpc](https://github.com/Swevo/PollyGrpc) | Polly v8 for gRPC |
| [PollyRabbitMQ](https://github.com/Swevo/PollyRabbitMQ) | Polly v8 for RabbitMQ |
| [PollyKafka](https://github.com/Swevo/PollyKafka) | Polly v8 for Confluent.Kafka |
| [PollySignalR](https://github.com/Swevo/PollySignalR) | Polly v8 for SignalR |
| [PollyOpenAI](https://github.com/Swevo/PollyOpenAI) | Polly v8 for OpenAI .NET SDK |
| [PollyMediatR](https://github.com/Swevo/PollyMediatR) | Polly v8 for MediatR |
| [PollyHealthChecks](https://github.com/Swevo/PollyHealthChecks) | Polly v8 for ASP.NET Core Health Checks |
| [PollyMassTransit](https://github.com/Swevo/PollyMassTransit) | Polly v8 for MassTransit |
| [PollyAzureTableStorage](https://github.com/Swevo/PollyAzureTableStorage) | Polly v8 for Azure Table Storage |
| [PollyMailKit](https://github.com/Swevo/PollyMailKit) | MailKit SMTP email client |
| [PollyAzureQueueStorage](https://github.com/Swevo/PollyAzureQueueStorage) | Azure Queue Storage QueueClient |
| [PollyHangfire](https://github.com/Swevo/PollyHangfire) | Hangfire IBackgroundJobClient |
| [PollyBackoff](https://github.com/Swevo/PollyBackoff) | Polly v8 backoff helpers |

## License

MIT © [Justin Bannister](https://github.com/Swevo)