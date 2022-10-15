using Microsoft.Extensions.Logging;
using static ChuckNorrisApp.JokesClient;

namespace ChuckNorrisApp;

public interface IJokesApp
{
    Task<string> New();
    string Previous();
    string Next();
}

public class JokesApp : IJokesApp
{
    private readonly ILogger _logger;
    private readonly IJokesClient _jokesClient;
    private readonly IList<Joke> _jokes;
    private int _currentIndex = 0;

    public JokesApp(ILogger<IJokesApp> logger, IJokesClient jokesClient)
    {
        _logger = logger;
        _jokesClient = jokesClient;
        _jokes = new List<Joke>();
    }

    public async Task<string> New()
    {
        _logger.LogInformation("Retrieving a new joke at {dateTime}", DateTime.UtcNow);

        var joke = await _jokesClient.Get();

        if (joke != null)
        {
            _jokes.Add(joke);

            _currentIndex = _jokes.Count - 1;

            return joke.Value;
        }

        return "Could not retrieve a new joke";
    }

    public string Previous()
    {
        if (_currentIndex > 0)
        {
            return _jokes[--_currentIndex].Value;
        }

        return "No more jokes!";
    }

    public string Next()
    {
        if (_currentIndex < _jokes.Count - 1)
        {
            return _jokes[++_currentIndex].Value;
        }

        return "No more jokes!";
    }
}
