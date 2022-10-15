using Moq.Protected;
using System.Net;
using static ChuckNorrisApp.JokesClient;

namespace ChuckNorrisTests;

public class JokesClientTests
{
    private readonly IJokesClient _subject;
    private readonly Mock<ILogger<IJokesClient>> _mockLogger = new();
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler = new();

    public JokesClientTests()
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_mockHttpMessageHandler.Object));
        _subject = new JokesClient(_mockLogger.Object, mockFactory.Object);
    }

    [Fact]
    public async Task Get_RetrieveJoke()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"id\":\"jokeId\",\"value\":\"joke content\"}"),
            });

        var result = await _subject.Get();

        Assert.NotNull(result);
        Assert.Equal(new Joke("jokeId", "joke content"), result);
    }

    [Fact]
    public async Task Get_BadRequest()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
            });

        var result = await _subject.Get();

        Assert.Null(result);
    }
}