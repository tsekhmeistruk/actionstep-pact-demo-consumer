using System.Net;
using FluentAssertions;
using PactDemo.Consumer.Tests.Infrastructure;
using PactNet.Matchers;

namespace PactDemo.Consumer.Tests.Users;

public class UsersConsumerTests : PactTestBase
{
    [Fact]
    public async Task GetUsersAsync_ReturnsUsers_WithCompanyAndAddress()
    {
        // The consumer (Users page + dashboard count) reads:
        //   name, username, email, company.name, address.city.
        // Other User model fields (phone, website, address.street/zipcode)
        // are deserialized but not displayed, so the contract intentionally
        // does not constrain them.
        PactBuilder
            .UponReceiving("a request to list all users")
                .Given(ProviderStates.UsersExist)
                .WithRequest(HttpMethod.Get, "/users")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Match.MinType(new
                {
                    id = Match.Type("1"),
                    name = Match.Type("Leanne Graham"),
                    username = Match.Type("Bret"),
                    email = Match.Regex("user@example.com", EmailRegex),
                    company = new
                    {
                        name = Match.Type("Acme Corp"),
                    },
                    address = new
                    {
                        city = Match.Type("Gwenborough"),
                    },
                }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var users = await client.GetUsersAsync();

            users.Should().NotBeEmpty();
            var user = users[0];
            user.Id.Should().NotBeNullOrWhiteSpace();
            user.Name.Should().NotBeNullOrWhiteSpace();
            user.Username.Should().NotBeNullOrWhiteSpace();
            user.Email.Should().NotBeNullOrWhiteSpace();
            user.Company.Name.Should().NotBeNullOrWhiteSpace();
            user.Address.City.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        PactBuilder
            .UponReceiving("a request to list all users when none exist")
                .Given(ProviderStates.NoUsersExist)
                .WithRequest(HttpMethod.Get, "/users")
            .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithHeader("Content-Type", JsonContentType)
                .WithJsonBody(Array.Empty<object>());

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var client = CreateClient(ctx.MockServerUri);

            var users = await client.GetUsersAsync();

            users.Should().BeEmpty();
        });
    }
}
