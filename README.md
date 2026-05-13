# PactDemo.ConsumerWeb

[![CI](https://github.com/tsekhmeistruk/actionstep-pact-demo-consumer/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/tsekhmeistruk/actionstep-pact-demo-consumer/actions/workflows/ci.yml)

PactNET consumer demo: an ASP.NET Core Razor Pages app that calls a fake REST API, with consumer-driven Pact contract tests.

## Projects

- `PactDemo.ConsumerWeb` — Razor Pages web app (the consumer).
- `PactDemo.Consumer.Tests` — xUnit + PactNet consumer contract tests for `FakeApiClient`.

## Run locally

```powershell
dotnet run --project PactDemo.ConsumerWeb
```

## Consumer Pact tests

### How to run

```powershell
dotnet test PactDemo.Consumer.Tests
```

Or from the solution root:

```powershell
dotnet test
```

### Test layout

```
PactDemo.Consumer.Tests/
├── Infrastructure/
│   ├── PactTestBase.cs        Shared Pact V3 setup + HttpClient factory.
│   ├── ProviderStates.cs      Provider-state name constants.
│   └── AssemblyAttributes.cs  Disables xUnit parallelism (single pact file).
├── Posts/      PostsConsumerTests.cs
├── Comments/   CommentsConsumerTests.cs
├── Users/      UsersConsumerTests.cs
└── Todos/      TodosConsumerTests.cs
```

### Where pact files go

The test base writes to `pacts/` at the repo root (resolved via `AppContext.BaseDirectory/../../../../pacts`). The folder is `.gitignore`d; CI generates it fresh on each run and publishes it to the broker.

A single pact file is produced: `pacts/PactDemo.ConsumerWeb-FakeRestApi.json`.

### Provider used for verification

The pact identifies the provider as **`FakeRestApi`**, which corresponds to the deployment at:

```
https://fake-rest-api-actionstep-demo.up.railway.app/
```

The consumer tests themselves never call this URL — they hit a PactNet mock server on `localhost`. The URL only matters for provider-side verification, which lives in a separate provider repo.

### What's covered

Every test corresponds to a real call made by `FakeApiClient` (see [`Services/FakeApiClient.cs`](PactDemo.ConsumerWeb/Services/FakeApiClient.cs)):

| Resource | Interaction | Provider state |
| --- | --- | --- |
| Posts | `GET /posts` returns list | `posts exist` |
| Posts | `GET /posts` returns empty | `no posts exist` |
| Posts | `GET /posts/1` returns one post | `post with id 1 exists` |
| Posts | `GET /posts/999` → 404 | `post with id 999 does not exist` |
| Comments | `GET /comments?postId=1` returns list | `comments exist for post with id 1` |
| Comments | `GET /comments?postId=999` returns empty | `no comments exist for post with id 999` |
| Users | `GET /users` returns list w/ company + address | `users exist` |
| Users | `GET /users` returns empty | `no users exist` |
| Todos | `GET /todos` returns list w/ completed flag | `todos exist` |
| Todos | `GET /todos` returns empty | `no todos exist` |

### What's intentionally NOT covered, and why

The original task listed 30 suggested tests. Many describe consumer behaviors that don't exist in this codebase. Per the rule "*only implement tests that match real consumer behavior*", these were skipped:

- **`GET /posts?userId=…`** — `FakeApiClient` has no such method.
- **`POST /posts`, `PUT /posts/:id`, `PATCH /posts/:id`, `DELETE /posts/:id`** — the consumer is read-only.
- **`GET /comments/:id`** — only the by-post-id query is consumed.
- **`POST /comments`** — read-only consumer.
- **`GET /users/:id`** — the consumer lists users (dashboard + Users page) but never fetches one by id.
- **`GET /users?email=…`** — no email search in the consumer.
- **`GET /todos/:id`, `PATCH /todos/:id`, `GET /todos?userId=…`, `GET /todos?completed=true`** — the consumer fetches all todos and filters in memory.
- **All `/albums` and `/photos` interactions** — `FakeApiClient` does not call these endpoints; no models, no pages reference them.

If the consumer grows to use any of these, the contract should grow with it — add tests in the corresponding folder when the consumer method lands.

### Matcher choices

- **IDs / string fields** — `Match.Type` (any string). `id` is strict-matched (`"1"`) only on the single-resource fetch where it must equal the request path.
- **Emails** — `Match.Regex` with a permissive email shape, since the consumer treats email as display text.
- **`completed`** — `Match.Type(false)` (any boolean).
- **Arrays with items** — `Match.MinType(template, 1)`. Min=1 is enough to express "non-empty"; PactNet's mock server generates a single example.
- **Empty arrays** — literal `[]`, so verification requires the provider to return exactly `[]` (not `null`).

## CI

On every push to `main`, GitHub Actions builds, runs all consumer tests, and publishes the generated pact to the broker via the `pactfoundation/pact-cli` Docker image. If any test fails, the pact is not published. Deployment to Railway is handled by Railway's GitHub integration.
