using System.Net;
using FluentAssertions;
using PactDemo.ConsumerWeb.Services;
using PactNet;
using PactNet.Matchers;

namespace PactDemo.Consumer.Tests;

public class FakeApiClientPactTests
{
    private readonly IPactBuilderV3 _pactBuilder;

    public FakeApiClientPactTests()
    {
        var pactDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "pacts"));

        var pact = Pact.V3(
            "PactDemo.ConsumerWeb",
            "FakeRestApi",
            new PactConfig { PactDir = pactDir });

        _pactBuilder = pact.WithHttpInteractions();
    }

    private static FakeApiClient CreateClient(Uri baseUri) =>
        new(new HttpClient { BaseAddress = baseUri });

    [Fact]
    public async Task GetPostsAsync_ReturnsPosts()
    {
        _pactBuilder
            .UponReceiving("a request to list all posts")
                .Given("posts exist")
                .WithRequest(HttpMethod.Get, "/posts")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    userId = Match.Type("1"),
                    title = Match.Type("a post title"),
                    body = Match.Type("a post body"),
                }, 2));

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var posts = await client.GetPostsAsync();

            posts.Should().NotBeEmpty();
            posts[0].Id.Should().NotBeNullOrWhiteSpace();
            posts[0].Title.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetPostAsync_ReturnsOnePost()
    {
        _pactBuilder
            .UponReceiving("a request to get post 1")
                .Given("a post with id 1 exists")
                .WithRequest(HttpMethod.Get, "/posts/1")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(new
                {
                    id = "1",
                    userId = "1",
                    title = Match.Type("a post title"),
                    body = Match.Type("a post body"),
                });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var post = await client.GetPostAsync("1");

            post.Should().NotBeNull();
            post!.Id.Should().Be("1");
            post.UserId.Should().Be("1");
            post.Title.Should().NotBeNullOrWhiteSpace();
            post.Body.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetCommentsByPostIdAsync_ReturnsComments()
    {
        _pactBuilder
            .UponReceiving("a request to list comments for post 1")
                .Given("comments exist for post 1")
                .WithRequest(HttpMethod.Get, "/comments")
                .WithQuery("postId", "1")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json; charset=utf-8")
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    postId = "1",
                    name = Match.Type("a commenter name"),
                    email = Match.Type("commenter@example.com"),
                    body = Match.Type("a comment body"),
                }, 1));

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var comments = await client.GetCommentsByPostIdAsync("1");

            comments.Should().NotBeEmpty();
            comments[0].PostId.Should().Be("1");
            comments[0].Email.Should().NotBeNullOrWhiteSpace();
        });
    }
}
