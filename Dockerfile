FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore as a distinct layer for cacheability
COPY PactDemo.ConsumerWeb/*.csproj PactDemo.ConsumerWeb/
RUN dotnet restore PactDemo.ConsumerWeb/PactDemo.ConsumerWeb.csproj

# Copy the rest of the web app sources and publish
COPY PactDemo.ConsumerWeb/ PactDemo.ConsumerWeb/
RUN dotnet publish PactDemo.ConsumerWeb/PactDemo.ConsumerWeb.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["sh", "-c", "dotnet PactDemo.ConsumerWeb.dll --urls http://0.0.0.0:${PORT:-8080}"]
