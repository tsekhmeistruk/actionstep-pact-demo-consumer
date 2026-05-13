using PactDemo.ConsumerWeb.Services;
using PactNet;

namespace PactDemo.Consumer.Tests.Infrastructure;

public abstract class PactTestBase
{
    protected const string ConsumerName = "PactDemo.ConsumerWeb";
    protected const string ProviderName = "FakeRestApi";
    protected const string JsonContentType = "application/json; charset=utf-8";
    protected const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    protected IPactBuilderV3 PactBuilder { get; }

    protected PactTestBase()
    {
        var pactDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "pacts"));

        var pact = Pact.V3(ConsumerName, ProviderName, new PactConfig { PactDir = pactDir });
        PactBuilder = pact.WithHttpInteractions();
    }

    protected static FakeApiClient CreateClient(Uri baseUri) =>
        new(new HttpClient { BaseAddress = baseUri });
}
