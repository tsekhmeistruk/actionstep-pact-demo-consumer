using Microsoft.AspNetCore.Mvc.RazorPages;
using PactDemo.ConsumerWeb.Models;
using PactDemo.ConsumerWeb.Services;

namespace PactDemo.ConsumerWeb.Pages;

public class TodosModel : PageModel
{
    private readonly FakeApiClient _api;
    private readonly ILogger<TodosModel> _logger;

    public IReadOnlyList<Todo> Todos { get; private set; } = Array.Empty<Todo>();
    public string? ErrorMessage { get; private set; }

    public int CompletedCount => Todos.Count(t => t.Completed);
    public int PendingCount => Todos.Count - CompletedCount;

    public TodosModel(FakeApiClient api, ILogger<TodosModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Todos = await _api.GetTodosAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load todos");
            ErrorMessage = "We couldn't load todos right now. Please try again later.";
        }
    }
}
