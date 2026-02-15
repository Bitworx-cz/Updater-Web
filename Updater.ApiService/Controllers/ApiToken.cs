using Microsoft.AspNetCore.Mvc;

namespace Updater.ApiService.Controllers;

internal static class ApiToken
{
    internal static string? GetToken(HttpRequest request, string? routeToken = null)
    {
        if (request.Headers.TryGetValue("Authorization", out var authorization) &&
            authorization.Count > 0)
        {
            var value = authorization.ToString();
            const string prefix = "Bearer ";
            if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return value[prefix.Length..].Trim();
            }
        }

        if (request.Headers.TryGetValue("X-Api-Token", out var tokenHeader) &&
            tokenHeader.Count > 0 &&
            !string.IsNullOrWhiteSpace(tokenHeader.ToString()))
        {
            return tokenHeader.ToString();
        }

        return string.IsNullOrWhiteSpace(routeToken) ? null : routeToken;
    }
}