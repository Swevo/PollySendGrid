/// <summary>
/// Pre-built Polly <see cref="PredicateBuilder"/> for transient SendGrid errors.
/// Covers rate limiting (429), internal server errors (500), and service unavailability (503).
/// </summary>
public static class SendGridTransientErrors
{
    /// <summary>
    /// HTTP status codes returned by SendGrid that indicate a transient failure safe to retry.
    /// </summary>
    public static readonly IReadOnlySet<HttpStatusCode> StatusCodes = new HashSet<HttpStatusCode>
    {
        HttpStatusCode.TooManyRequests,       // 429 — rate limited; back off and retry
        HttpStatusCode.InternalServerError,   // 500 — transient server-side error
        HttpStatusCode.ServiceUnavailable,    // 503 — SendGrid maintenance or overload
    };

    /// <summary>
    /// A <see cref="PredicateBuilder"/> that handles <see cref="SendGridTransientException"/>
    /// (thrown by <see cref="ResilientSendGridClient"/> for 429/500/503 responses)
    /// and <see cref="HttpRequestException"/> (thrown for network-level failures).
    /// Assign to <c>ShouldHandle</c> on any Polly strategy.
    /// </summary>
    public static readonly PredicateBuilder IsTransient =
        (PredicateBuilder)new PredicateBuilder()
            .Handle<SendGridTransientException>()
            .Handle<HttpRequestException>();
}
