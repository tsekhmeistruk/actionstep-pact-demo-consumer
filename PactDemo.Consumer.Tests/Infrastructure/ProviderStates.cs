namespace PactDemo.Consumer.Tests.Infrastructure;

public static class ProviderStates
{
    // Posts
    public const string PostsExist = "posts exist";
    public const string NoPostsExist = "no posts exist";
    public const string PostWithId1Exists = "post with id 1 exists";
    public const string PostWithId999DoesNotExist = "post with id 999 does not exist";

    // Comments
    public const string CommentsExistForPost1 = "comments exist for post with id 1";
    public const string NoCommentsExistForPost999 = "no comments exist for post with id 999";

    // Users
    public const string UsersExist = "users exist";
    public const string NoUsersExist = "no users exist";

    // Todos
    public const string TodosExist = "todos exist";
    public const string NoTodosExist = "no todos exist";
}
