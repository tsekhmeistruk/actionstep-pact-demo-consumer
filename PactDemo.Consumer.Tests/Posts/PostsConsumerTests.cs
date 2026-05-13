using System.Net;
using FluentAssertions;
using PactDemo.Consumer.Tests.Infrastructure;
using PactNet.Matchers;

namespace PactDemo.Consumer.Tests.Posts;

public class PostsConsumerTests : PactTestBase
{
    [Fact]
    public async Task GetPostsAsync_ReturnsPosts_WhenPostsExist()
    {
        PactBuilder
            .UponReceiving("a request to list all posts")
                .Given(ProviderStates.PostsExist)
                .WithRequest(HttpMethod.Get, "/posts")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    userId = Match.Type("1"),
                    title = Match.Type("a post title"),
                    body = Match.Type("a post body"),
                }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var posts = await client.GetPostsAsync();

            posts.Should().NotBeEmpty();
            posts[0].Id.Should().NotBeNullOrWhiteSpace();
            posts[0].UserId.Should().NotBeNullOrWhiteSpace();
            posts[0].Title.Should().NotBeNullOrWhiteSpace();
            posts[0].Body.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetPostsAsync_ReturnsEmptyList_WhenNoPostsExist()
    {
        PactBuilder
            .UponReceiving("a request to list all posts when none exist")
                .Given(ProviderStates.NoPostsExist)
                .WithRequest(HttpMethod.Get, "/posts")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Array.Empty<object>());

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var posts = await client.GetPostsAsync();

            posts.Should().BeEmpty();
        });
    }

    [Fact]
    public async Task GetPostAsync_ReturnsPost_WhenPostExists()
    {
        PactBuilder
            .UponReceiving("a request to get post with id 1")
                .Given(ProviderStates.PostWithId1Exists)
                .WithRequest(HttpMethod.Get, "/posts/1")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(new
                {
                    id = "1",
                    userId = Match.Type("1"),
                    title = Match.Type("a post title"),
                    body = Match.Type("a post body"),
                });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var post = await client.GetPostAsync("1");

            post.Should().NotBeNull();
            post!.Id.Should().Be("1");
            post.UserId.Should().NotBeNullOrWhiteSpace();
            post.Title.Should().NotBeNullOrWhiteSpace();
            post.Body.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetPostAsync_ReturnsNull_WhenPostDoesNotExist()
    {
        PactBuilder
            .UponReceiving("a request to get post with id 999 that does not exist")
                .Given(ProviderStates.PostWithId999DoesNotExist)
                .WithRequest(HttpMethod.Get, "/posts/999")
            .WillRespond()
                .WithStatus(HttpStatusCode.NotFound);

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var post = await client.GetPostAsync("999");

            post.Should().BeNull();
        });
    }
}
