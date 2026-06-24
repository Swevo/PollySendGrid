public class PollySendGridExtensionsTests
{
    private static readonly ISendGridClient _client = new SendGridClient("SG.fake-key-for-unit-tests-only");
    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder().Build();

    [Fact]
    public void WithPolly_Pipeline_ReturnsResilientClient()
    {
        var resilient = _client.WithPolly(_pipeline);
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void WithPolly_Configure_ReturnsResilientClient()
    {
        var resilient = _client.WithPolly(p => p.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3, Delay = TimeSpan.Zero,
            ShouldHandle = SendGridTransientErrors.IsTransient,
        }));
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }
}
