using Microsoft.AspNetCore.Mvc.RazorPages;
using PactDemo.ConsumerWeb.Models;
using PactDemo.ConsumerWeb.Services;

namespace PactDemo.ConsumerWeb.Pages;

public class UsersModel : PageModel
{
    private readonly FakeApiClient _api;
    private readonly ILogger<UsersModel> _logger;

    public IReadOnlyList<User> Users { get; private set; } = Array.Empty<User>();
    public string? ErrorMessage { get; private set; }

    public UsersModel(FakeApiClient api, ILogger<UsersModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Users = await _api.GetUsersAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users");
            ErrorMessage = "We couldn't load users right now. Please try again later.";
        }
    }
}
