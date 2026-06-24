public class SendGridTransientErrorsTests
{
    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public void StatusCodes_ContainsTransientCode(HttpStatusCode code)
        => Assert.Contains(code, SendGridTransientErrors.StatusCodes);

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Accepted)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    public void StatusCodes_DoesNotContainNonTransientCode(HttpStatusCode code)
        => Assert.DoesNotContain(code, SendGridTransientErrors.StatusCodes);

    [Fact]
    public void StatusCodes_HasThreeEntries()
        => Assert.Equal(3, SendGridTransientErrors.StatusCodes.Count);

    [Fact]
    public void IsTransient_IsNotNull()
        => Assert.NotNull(SendGridTransientErrors.IsTransient);

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task IsTransient_RetriesSendGridTransientException(HttpStatusCode code)
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = SendGridTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<SendGridTransientException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new SendGridTransientException(code); }).AsTask());

        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task IsTransient_RetriesHttpRequestException()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = SendGridTransientErrors.IsTransient })
            .Build();

        var attempts = 0;
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new HttpRequestException("network"); }).AsTask());

        Assert.Equal(2, attempts);
    }
}
