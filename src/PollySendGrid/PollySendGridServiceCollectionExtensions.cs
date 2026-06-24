/// <summary>Dependency-injection extensions for <c>PollySendGrid</c>.</summary>
public static class PollySendGridServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="ResiliencePipeline"/> built by <paramref name="configure"/>
    /// and a transient <see cref="ResilientSendGridClient"/> that wraps the
    /// <see cref="ISendGridClient"/> already registered in the DI container.
    /// </summary>
    public static IServiceCollection AddPollySendGrid(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        services.AddSingleton(pipeline);
        services.AddTransient<ResilientSendGridClient>(sp =>
            sp.GetRequiredService<ISendGridClient>().WithPolly(pipeline));

        return services;
    }

    /// <summary>
    /// Registers a singleton <see cref="SendGridClient"/> for <paramref name="apiKey"/>,
    /// then registers the resilience pipeline and <see cref="ResilientSendGridClient"/>.
    /// </summary>
    public static IServiceCollection AddPollySendGrid(
        this IServiceCollection services,
        string apiKey,
        Action<ResiliencePipelineBuilder> configure)
    {
        services.AddSingleton<ISendGridClient>(new SendGridClient(apiKey));
        return services.AddPollySendGrid(configure);
    }
}
