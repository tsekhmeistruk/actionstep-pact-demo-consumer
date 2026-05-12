using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PactDemo.ConsumerWeb.Models;
using PactDemo.ConsumerWeb.Services;

namespace PactDemo.ConsumerWeb.Pages.Posts;

public class DetailsModel : PageModel
{
    private readonly FakeApiClient _api;
    private readonly ILogger<DetailsModel> _logger;

    [FromRoute]
    public string Id { get; set; } = string.Empty;

    public Post? Post { get; private set; }
    public IReadOnlyList<Comment> Comments { get; private set; } = Array.Empty<Comment>();
    public string? ErrorMessage { get; private set; }
    public bool PostNotFound { get; private set; }

    public DetailsModel(FakeApiClient api, ILogger<DetailsModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Post = await _api.GetPostAsync(Id, cancellationToken);
            if (Post is null)
            {
                PostNotFound = true;
                return;
            }

            Comments = await _api.GetCommentsByPostIdAsync(Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load post {PostId}", Id);
            ErrorMessage = "We couldn't load this post right now. Please try again later.";
        }
    }
}
