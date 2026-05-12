# PactDemo.ConsumerWeb

[![CI](https://github.com/tsekhmeistruk/actionstep-pact-demo-consumer/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/tsekhmeistruk/actionstep-pact-demo-consumer/actions/workflows/ci.yml)

PactNET consumer demo: an ASP.NET Core Razor Pages app that calls a fake REST API, with consumer-driven Pact contract tests.

## Projects

- `PactDemo.ConsumerWeb` — Razor Pages web app (the consumer).
- `PactDemo.Consumer.Tests` — xUnit + PactNet tests for `FakeApiClient`.

## Run locally

```powershell
dotnet run --project PactDemo.ConsumerWeb
```

## Test

```powershell
dotnet test
```

Generates `pacts/PactDemo.ConsumerWeb-FakeRestApi.json`.

## CI

On every push to `main`, the workflow builds, runs the tests, and publishes the generated pact to the Pact Broker. If tests fail, the pact is not published. Deployment to Railway is handled separately by Railway's GitHub integration.
