/// <summary>
/// Wraps an <see cref="ISendGridClient"/> with a Polly v8 <see cref="ResiliencePipeline"/>,
/// applying retry, timeout, and circuit-breaker to every send operation.
/// Automatically throws <see cref="SendGridTransientException"/> for 429, 500, and 503
/// responses so Polly can retry them.
/// </summary>
public sealed class ResilientSendGridClient(ISendGridClient client, ResiliencePipeline pipeline)
{
    /// <summary>The underlying <see cref="ISendGridClient"/>.</summary>
    public ISendGridClient Inner => client;

    /// <summary>
    /// Sends an email, protected by the resilience pipeline.
    /// Throws <see cref="SendGridTransientException"/> when the response status code
    /// is in <see cref="SendGridTransientErrors.StatusCodes"/> so Polly can retry it.
    /// </summary>
    public Task<Response> SendEmailAsync(
        SendGridMessage msg,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(async ct =>
        {
            var response = await client.SendEmailAsync(msg, ct);
            if (SendGridTransientErrors.StatusCodes.Contains(response.StatusCode))
                throw new SendGridTransientException(response.StatusCode);
            return response;
        }, cancellationToken).AsTask();

    /// <summary>
    /// Executes any <see cref="ISendGridClient"/> operation, protected by the resilience pipeline.
    /// </summary>
    public Task<T> ExecuteAsync<T>(
        Func<ISendGridClient, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await operation(client, ct),
            cancellationToken).AsTask();
}
