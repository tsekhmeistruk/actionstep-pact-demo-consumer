using System.Net;
using FluentAssertions;
using PactDemo.Consumer.Tests.Infrastructure;
using PactNet.Matchers;

namespace PactDemo.Consumer.Tests.Todos;

public class TodosConsumerTests : PactTestBase
{
    [Fact]
    public async Task GetTodosAsync_ReturnsTodos_WithCompletedFlag()
    {
        PactBuilder
            .UponReceiving("a request to list all todos")
                .Given(ProviderStates.TodosExist)
                .WithRequest(HttpMethod.Get, "/todos")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    userId = Match.Type("1"),
                    title = Match.Type("a todo title"),
                    completed = Match.Type(false),
                }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var todos = await client.GetTodosAsync();

            todos.Should().NotBeEmpty();
            var first = todos[0];
            first.Id.Should().NotBeNullOrWhiteSpace();
            first.UserId.Should().NotBeNullOrWhiteSpace();
            first.Title.Should().NotBeNullOrWhiteSpace();
            // `completed` is bool — asserting it deserialized without throwing is the contract value.
            _ = first.Completed;
        });
    }

    [Fact]
    public async Task GetTodosAsync_ReturnsEmptyList_WhenNoTodosExist()
    {
        PactBuilder
            .UponReceiving("a request to list all todos when none exist")
                .Given(ProviderStates.NoTodosExist)
                .WithRequest(HttpMethod.Get, "/todos")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Array.Empty<object>());

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var todos = await client.GetTodosAsync();

            todos.Should().BeEmpty();
        });
    }
}
