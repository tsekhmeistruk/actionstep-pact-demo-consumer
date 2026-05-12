using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PactDemo.ConsumerWeb.Models;

namespace PactDemo.ConsumerWeb.Services;

public class FakeApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private readonly HttpClient _httpClient;

    public FakeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<Post>> GetPostsAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _httpClient.GetFromJsonAsync<List<Post>>("posts", JsonOptions, cancellationToken);
        return posts ?? new List<Post>();
    }

    public async Task<Post?> GetPostAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"posts/{Uri.EscapeDataString(id)}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Post>(JsonOptions, cancellationToken);
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsByPostIdAsync(string postId, CancellationToken cancellationToken = default)
    {
        var comments = await _httpClient.GetFromJsonAsync<List<Comment>>(
            $"comments?postId={Uri.EscapeDataString(postId)}",
            JsonOptions,
            cancellationToken);
        return comments ?? new List<Comment>();
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>("users", JsonOptions, cancellationToken);
        return users ?? new List<User>();
    }

    public async Task<IReadOnlyList<Todo>> GetTodosAsync(CancellationToken cancellationToken = default)
    {
        var todos = await _httpClient.GetFromJsonAsync<List<Todo>>("todos", JsonOptions, cancellationToken);
        return todos ?? new List<Todo>();
    }
}
