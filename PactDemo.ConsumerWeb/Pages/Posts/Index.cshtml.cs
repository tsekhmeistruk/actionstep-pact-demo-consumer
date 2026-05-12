using Microsoft.AspNetCore.Mvc.RazorPages;
using PactDemo.ConsumerWeb.Models;
using PactDemo.ConsumerWeb.Services;

namespace PactDemo.ConsumerWeb.Pages.Posts;

public class IndexModel : PageModel
{
    private readonly FakeApiClient _api;
    private readonly ILogger<IndexModel> _logger;

    public IReadOnlyList<Post> Posts { get; private set; } = Array.Empty<Post>();
    public string? ErrorMessage { get; private set; }

    public IndexModel(FakeApiClient api, ILogger<IndexModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Posts = await _api.GetPostsAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load posts");
            ErrorMessage = "We couldn't load posts right now. Please try again later.";
        }
    }
}
