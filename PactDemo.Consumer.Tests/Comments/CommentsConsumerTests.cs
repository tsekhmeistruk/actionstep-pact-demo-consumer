using System.Net;
using FluentAssertions;
using PactDemo.Consumer.Tests.Infrastructure;
using PactNet.Matchers;

namespace PactDemo.Consumer.Tests.Comments;

public class CommentsConsumerTests : PactTestBase
{
    [Fact]
    public async Task GetCommentsByPostIdAsync_ReturnsComments_WhenCommentsExist()
    {
        PactBuilder
            .UponReceiving("a request to list comments for post with id 1")
                .Given(ProviderStates.CommentsExistForPost1)
                .WithRequest(HttpMethod.Get, "/comments")
                .WithQuery("postId", "1")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    postId = "1",
                    name = Match.Type("a commenter name"),
                    email = Match.Regex("user@example.com", EmailRegex),
                    body = Match.Type("a comment body"),
                }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var comments = await client.GetCommentsByPostIdAsync("1");

            comments.Should().NotBeEmpty();
            var first = comments[0];
            first.Id.Should().NotBeNullOrWhiteSpace();
            first.PostId.Should().Be("1");
            first.Name.Should().NotBeNullOrWhiteSpace();
            first.Email.Should().NotBeNullOrWhiteSpace();
            first.Body.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetCommentsByPostIdAsync_ReturnsEmptyList_WhenPostHasNoComments()
    {
        PactBuilder
            .UponReceiving("a request to list comments for post with no comments")
                .Given(ProviderStates.NoCommentsExistForPost999)
                .WithRequest(HttpMethod.Get, "/comments")
                .WithQuery("postId", "999")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Array.Empty<object>());

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var comments = await client.GetCommentsByPostIdAsync("999");

            comments.Should().BeEmpty();
        });
    }
}
