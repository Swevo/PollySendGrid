/// <summary>Extension methods for adding Polly resilience to SendGrid clients.</summary>
public static class PollySendGridExtensions
{
    /// <summary>Wraps an <see cref="ISendGridClient"/> with the given <see cref="ResiliencePipeline"/>.</summary>
    public static ResilientSendGridClient WithPolly(
        this ISendGridClient client,
        ResiliencePipeline pipeline)
        => new(client, pipeline);

    /// <summary>Wraps an <see cref="ISendGridClient"/> with a pipeline built by <paramref name="configure"/>.</summary>
    public static ResilientSendGridClient WithPolly(
        this ISendGridClient client,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        return new(client, builder.Build());
    }
}
