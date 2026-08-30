[![](https://img.shields.io/nuget/v/soenneker.close.httpclients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.httpclients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.close.httpclients/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.close.httpclients/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.close.httpclients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.httpclients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.close.httpclients/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.close.httpclients/actions/workflows/codeql.yml)

# Soenneker.Close.HttpClients

Provides a reusable `HttpClient` configured for the Close CRM API and dependency injection.

## Installation

```bash
dotnet add package Soenneker.Close.HttpClients
```

## API-key configuration

```json
{
  "Close": {
    "ApiKey": "<Close API key>",
    "ClientBaseUrl": "https://api.close.com/api/v1"
  }
}
```

`Close:ApiKey` is required; the base URL above is the default. The client applies Close's required HTTP Basic authorization by encoding the API key as the username with an empty password. See [Close's API-key authentication documentation](https://developer.close.com/api/overview/api-key-authentication).

## OAuth or custom authorization

Override the header template when `Close:ApiKey` contains an OAuth access token:

```json
{
  "Close": {
    "ApiKey": "<OAuth access token>",
    "AuthHeaderName": "Authorization",
    "AuthHeaderValueTemplate": "Bearer {token}"
  }
}
```

`{token}` is replaced with the configured value. `AuthHeaderName` defaults to `Authorization`.

Keep API keys and OAuth tokens in a secret provider rather than source control or checked-in settings files.

## Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Soenneker.Close.HttpClients.Registrars;

services.AddCloseOpenApiHttpClientAsSingleton();
```

`AddCloseOpenApiHttpClientAsScoped()` creates one wrapper and cached client per dependency-injection scope.

## Usage

```csharp
using Soenneker.Close.HttpClients.Abstract;

public sealed class CloseProfileClient
{
    private readonly ICloseOpenApiHttpClient _closeHttpClient;

    public CloseProfileClient(ICloseOpenApiHttpClient closeHttpClient)
    {
        _closeHttpClient = closeHttpClient;
    }

    public async ValueTask<string> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        HttpClient client = await _closeHttpClient.Get(cancellationToken);

        using HttpResponseMessage response = await client.GetAsync(
            "/api/v1/me/",
            cancellationToken);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

For a typed request-builder API, use this package through `Soenneker.Close.OpenApiClientUtil`.

## Lifecycle and behavior

- `Get` returns the same `HttpClient` for the lifetime of the wrapper.
- Do not dispose the returned client. Let dependency injection dispose `ICloseOpenApiHttpClient` and its cache entry.
- Configuration and credentials are captured when the client is first created. Recreate the owning scope or application instance after rotating them.
- The cancellation token passed to `Get` applies to initialization. Pass a token separately to each HTTP operation.
- Non-success responses remain ordinary `HttpResponseMessage` instances until the caller inspects them or calls `EnsureSuccessStatusCode`.
