using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using static ChuckNorrisApp.JokesClient;

namespace ChuckNorrisApp;

/*
Things to improve in future:
1- Add polly retry policy
2- Better error handling
 */

public interface IJokesClient
{
    Task<Joke?> Get();
}

public class JokesClient : IJokesClient
{
    private const string Url = "https://api.chucknorris.io/jokes/random";
    private readonly ILogger<IJokesClient> _logger;
    private readonly IHttpClientFactory _httpFactory;

    public JokesClient(ILogger<IJokesClient> logger, IHttpClientFactory httpFactory)
    {
        _logger = logger;
        _httpFactory = httpFactory;
    }

    public async Task<Joke?> Get()
    {
        using var client = _httpFactory.CreateClient();
        var response = await client.GetAsync(Url);

        if (response?.IsSuccessStatusCode == true)
        {
            return await response.Content.ReadFromJsonAsync<Joke>();
        }
        else
        {
            _logger.LogWarning($"GetAsync failed. {response}");
            return null;
        }
    }

    public record Joke(string Id, string Value);
}
