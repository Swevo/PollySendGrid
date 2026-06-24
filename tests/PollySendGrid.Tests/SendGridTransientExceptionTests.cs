public class SendGridTransientExceptionTests
{
    [Fact]
    public void Constructor_SetsStatusCode()
        => Assert.Equal(HttpStatusCode.TooManyRequests, new SendGridTransientException(HttpStatusCode.TooManyRequests).StatusCode);

    [Fact]
    public void Constructor_MessageContainsStatusCode()
        => Assert.Contains("429", new SendGridTransientException(HttpStatusCode.TooManyRequests).Message);

    [Fact]
    public void Constructor_WithInner_SetsInnerException()
    {
        var inner = new Exception("original");
        var ex = new SendGridTransientException(HttpStatusCode.TooManyRequests, inner);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void IsException()
        => Assert.IsAssignableFrom<Exception>(new SendGridTransientException(HttpStatusCode.TooManyRequests));
}
