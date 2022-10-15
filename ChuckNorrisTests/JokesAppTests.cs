
using static ChuckNorrisApp.JokesClient;

namespace ChuckNorrisTests;

public class JokesAppTests
{
    private readonly IJokesApp _subject;
    private readonly Mock<ILogger<IJokesApp>> _mockLogger = new();
    private readonly Mock<IJokesClient> _mockJokesClient = new();

    public JokesAppTests()
    {
        _subject = new JokesApp(_mockLogger.Object, _mockJokesClient.Object);
    }

    [Fact]
    public async Task New_HappyPath()
    {
        var expectedCalls = 0;

        // No jokes in cache
        var prev1 = _subject.Previous();

        Assert.Equal("No more jokes!", prev1);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        var next1 = _subject.Next();

        Assert.Equal("No more jokes!", next1);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        // Get the first joke
        _mockJokesClient.Setup(o => o.Get())
           .ReturnsAsync(new Joke("jokeId1", "Joke 1"));
        expectedCalls++;

        var new1 = await _subject.New();

        Assert.Equal("Joke 1", new1);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        // Only one joke in cache, can't go back or forward
        var prev2 = _subject.Previous();

        Assert.Equal("No more jokes!", prev2);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        var next2 = _subject.Next();

        Assert.Equal("No more jokes!", next2);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        // Get the second joke
        _mockJokesClient.Setup(o => o.Get())
           .ReturnsAsync(new Joke("jokeId2", "Joke 2"));
        expectedCalls++;

        var new2 = await _subject.New();

        Assert.Equal("Joke 2", new2);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        // Go back
        var prev3 = _subject.Previous();

        Assert.Equal("Joke 1", prev3);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));

        // Go forward
        var next3 = _subject.Next();

        Assert.Equal("Joke 2", next3);
        _mockJokesClient.Verify(o => o.Get(), Times.Exactly(expectedCalls));
    }

    [Fact]
    public async Task New_NoJoke()
    {
        _mockJokesClient.Setup(o => o.Get())
            .ReturnsAsync((Joke?)null);

        var result = await _subject.New();

        Assert.NotNull(result);
        Assert.Equal("Could not retrieve a new joke", result);
        _mockJokesClient.Verify(o => o.Get(), Times.Once);
    }
}