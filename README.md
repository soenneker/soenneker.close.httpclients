[![](https://img.shields.io/nuget/v/soenneker.close.httpclients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.httpclients/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.close.httpclients/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.close.httpclients/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.close.httpclients.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.close.httpclients/)

# Soenneker.Close.HttpClients

A .NET thread-safe singleton HttpClient for.

## Install

```bash
dotnet add package Soenneker.Close.HttpClients
```

## Quick start

```csharp
using Soenneker.Close.HttpClients.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddCloseOpenApiHttpClientAsSingleton();
```

Adds `CloseOpenApiHttpClient` as a singleton service.

## What you get

- `ICloseOpenApiHttpClient` — A .NET thread-safe singleton HttpClient for.
- `CloseOpenApiHttpClientRegistrar` — Registers the OpenAPI HttpClient wrapper for dependency injection.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `CloseOpenApiHttpClientRegistrar.AddCloseOpenApiHttpClientAsSingleton(services)` | Adds `CloseOpenApiHttpClient` as a singleton service. | The same service collection, so additional registrations can be chained. |
| `CloseOpenApiHttpClientRegistrar.AddCloseOpenApiHttpClientAsScoped(services)` | Adds `CloseOpenApiHttpClient` as a scoped service. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Reuse the registered client instead of constructing one per operation.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.
