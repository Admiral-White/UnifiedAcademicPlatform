using Microsoft.Extensions.Configuration;

namespace UAP.Shared.Infrastructure.HttpHandlers;

public class ApiKeyDelegatingHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    private readonly string _serviceName;

    public ApiKeyDelegatingHandler(IConfiguration configuration, string serviceName)
    {
        _configuration = configuration;
        _serviceName = serviceName;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var headerName = _configuration["ApiKey:HeaderName"] ?? "X-API-Key";
        var apiKey = _configuration[$"ApiKey:ValidKeys:{_serviceName}"];

        if (!string.IsNullOrEmpty(apiKey))
        {
            request.Headers.Add(headerName, apiKey);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
