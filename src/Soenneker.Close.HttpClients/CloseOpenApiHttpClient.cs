using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.Configuration;
using Soenneker.Close.HttpClients.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Close.HttpClients;

/// <inheritdoc cref="ICloseOpenApiHttpClient" />
public sealed class CloseOpenApiHttpClient : ICloseOpenApiHttpClient
{
    private readonly IHttpClientCache _httpClientCache;
    private readonly IConfiguration _config;
    private readonly string _httpClientCacheKey = $"{nameof(CloseOpenApiHttpClient)}:{Guid.NewGuid():N}";

    private const string _prodBaseUrl = "https://api.close.com/api/v1";

    public CloseOpenApiHttpClient(IHttpClientCache httpClientCache, IConfiguration config)
    {
        _httpClientCache = httpClientCache;
        _config = config;
    }

    public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default)
    {
        return _httpClientCache.Get(_httpClientCacheKey, (config: _config, baseUrl: _config["Close:ClientBaseUrl"] ?? _prodBaseUrl), static state =>
        {
            var apiKey = state.config.GetValueStrict<string>("Close:ApiKey");
            string authHeaderName = state.config["Close:AuthHeaderName"] ?? "Authorization";
            string? authHeaderValueTemplate = state.config["Close:AuthHeaderValueTemplate"];
            string authHeaderValue = authHeaderValueTemplate is null
                ? $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:"))}"
                : authHeaderValueTemplate.Replace("{token}", apiKey, StringComparison.Ordinal);

            return new HttpClientOptions
            {
                BaseAddress = new Uri(state.baseUrl),
                DefaultRequestHeaders = new Dictionary<string, string>
                {
                    {authHeaderName, authHeaderValue},
                }
            };
        }, cancellationToken);
    }

    public void Dispose()
    {
        _httpClientCache.RemoveSync(_httpClientCacheKey);
    }

    public ValueTask DisposeAsync()
    {
        return _httpClientCache.Remove(_httpClientCacheKey);
    }
}
