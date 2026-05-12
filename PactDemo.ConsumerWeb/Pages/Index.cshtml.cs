using Microsoft.AspNetCore.Mvc.RazorPages;
using PactDemo.ConsumerWeb.Services;

namespace PactDemo.ConsumerWeb.Pages;

public class IndexModel : PageModel
{
    private readonly FakeApiClient _api;
    private readonly ILogger<IndexModel> _logger;

    public int PostCount { get; private set; }
    public int UserCount { get; private set; }
    public int TodoCount { get; private set; }
    public int CompletedTodoCount { get; private set; }
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
            var postsTask = _api.GetPostsAsync(cancellationToken);
            var usersTask = _api.GetUsersAsync(cancellationToken);
            var todosTask = _api.GetTodosAsync(cancellationToken);

            await Task.WhenAll(postsTask, usersTask, todosTask);

            PostCount = postsTask.Result.Count;
            UserCount = usersTask.Result.Count;
            TodoCount = todosTask.Result.Count;
            CompletedTodoCount = todosTask.Result.Count(t => t.Completed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data");
            ErrorMessage = "We couldn't load dashboard data right now. Please try again later.";
        }
    }
}
