/// <summary>
/// Exception thrown by <see cref="ResilientSendGridClient"/> when SendGrid returns
/// a transient HTTP status code (429, 500, 503) so that Polly can handle it.
/// </summary>
public sealed class SendGridTransientException : Exception
{
    /// <summary>The HTTP status code returned by SendGrid.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <inheritdoc cref="SendGridTransientException"/>
    public SendGridTransientException(HttpStatusCode statusCode)
        : base($"SendGrid transient error: HTTP {(int)statusCode} {statusCode}. Retry is safe.")
    {
        StatusCode = statusCode;
    }

    /// <inheritdoc cref="SendGridTransientException"/>
    public SendGridTransientException(HttpStatusCode statusCode, Exception inner)
        : base($"SendGrid transient error: HTTP {(int)statusCode} {statusCode}. Retry is safe.", inner)
    {
        StatusCode = statusCode;
    }
}
