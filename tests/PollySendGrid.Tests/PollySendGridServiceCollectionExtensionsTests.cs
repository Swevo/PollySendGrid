public class PollySendGridServiceCollectionExtensionsTests
{
    private static readonly ISendGridClient _client = new SendGridClient("SG.fake-key-for-unit-tests-only");

    [Fact]
    public void AddPollySendGrid_RegistersResiliencePipeline()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollySendGrid(p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResiliencePipeline>());
    }

    [Fact]
    public void AddPollySendGrid_RegistersResilientSendGridClient()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollySendGrid(p => { });
        var resilient = services.BuildServiceProvider().GetRequiredService<ResilientSendGridClient>();
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void AddPollySendGrid_WithApiKey_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddPollySendGrid("SG.fake-key", p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResilientSendGridClient>());
    }

    [Fact]
    public void AddPollySendGrid_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        Assert.Same(services, services.AddPollySendGrid(p => { }));
    }
}
