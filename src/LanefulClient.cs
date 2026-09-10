using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Laneful.Exceptions;
using Laneful.Models;
using Microsoft.Extensions.Logging;

namespace Laneful;

/// <summary>
/// Main client for communicating with the Laneful API.
/// Email sending uses a send host (https://your-endpoint.send.laneful.net).
/// Domain, unsubscribe-group, and analytics endpoints use the organization
/// API host (https://api.laneful.net).
/// </summary>
public class LanefulClient
{
    private const string ApiVersion = "v1";
    private const string UserAgent = "laneful-csharp/1.2.0";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private readonly string _baseUrl;
    private readonly string _authToken;
    private readonly HttpClient _httpClient;
    private readonly ILogger<LanefulClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new LanefulClient with the specified configuration.
    /// </summary>
    public LanefulClient(string baseUrl, string authToken, HttpClient? httpClient = null, ILogger<LanefulClient>? logger = null)
        : this(baseUrl, authToken, DefaultTimeout, httpClient, logger)
    {
    }

    /// <summary>
    /// Creates a new LanefulClient with custom timeout.
    /// </summary>
    public LanefulClient(string baseUrl, string authToken, TimeSpan timeout, HttpClient? httpClient = null, ILogger<LanefulClient>? logger = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ValidationException("Base URL cannot be empty");

        if (string.IsNullOrWhiteSpace(authToken))
            throw new ValidationException("Auth token cannot be empty");

        _baseUrl = baseUrl.Trim();
        _authToken = authToken.Trim();
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false
        };

        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = timeout;
        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_authToken}");
        if (!_httpClient.DefaultRequestHeaders.Contains("Accept"))
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
    }

    /// <summary>
    /// Sends a single email.
    /// </summary>
    public Task<Dictionary<string, object>> SendEmailAsync(Email email, MailSettings? settings = null)
    {
        return SendEmailsAsync(new[] { email }, settings);
    }

    /// <summary>
    /// Sends multiple emails.
    /// </summary>
    public async Task<Dictionary<string, object>> SendEmailsAsync(IEnumerable<Email> emails, MailSettings? settings = null)
    {
        var emailList = emails.ToList();

        if (emailList.Count == 0)
            throw new ValidationException("Emails list cannot be empty");

        foreach (var email in emailList)
        {
            if (email == null)
                throw new ValidationException("Email cannot be null");
        }

        var requestData = new Dictionary<string, object?>
        {
            ["emails"] = emailList
        };

        if (settings != null)
            requestData["mail_settings"] = settings;

        var body = await SendRequestAsync(HttpMethod.Post, "/email/send", requestData);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(body, _jsonOptions)
               ?? new Dictionary<string, object>();
    }

    /// <summary>
    /// List unsubscribe groups for a workspace.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<ListUnsubscribeGroupsResponse> ListUnsubscribeGroupsAsync(
        long workspaceId,
        ListUnsubscribeGroupsParams? parameters = null)
    {
        return RequestAsync<ListUnsubscribeGroupsResponse>(
            HttpMethod.Get,
            $"/workspaces/{workspaceId}/unsubscribe-groups",
            query: parameters?.ToQuery());
    }

    /// <summary>
    /// Create an unsubscribe group in a workspace.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public async Task<UnsubscribeGroup> CreateUnsubscribeGroupAsync(long workspaceId, string name)
    {
        var response = await RequestAsync<UnsubscribeGroupResponse>(
            HttpMethod.Post,
            $"/workspaces/{workspaceId}/unsubscribe-groups",
            new { name });
        return response.UnsubscribeGroup;
    }

    /// <summary>
    /// Update an unsubscribe group.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public async Task<UnsubscribeGroup> UpdateUnsubscribeGroupAsync(
        long workspaceId,
        long unsubscribeGroupId,
        string name)
    {
        var response = await RequestAsync<UnsubscribeGroupResponse>(
            HttpMethod.Patch,
            $"/workspaces/{workspaceId}/unsubscribe-groups/{unsubscribeGroupId}",
            new { name });
        return response.UnsubscribeGroup;
    }

    /// <summary>
    /// List sending domains for a workspace.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<ListDomainsResponse> ListDomainsAsync(long workspaceId, ListDomainsParams? parameters = null)
    {
        return RequestAsync<ListDomainsResponse>(
            HttpMethod.Get,
            $"/workspaces/{workspaceId}/domains",
            query: parameters?.ToQuery());
    }

    /// <summary>
    /// Get a single sending domain by name.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<Domain> GetDomainAsync(long workspaceId, string domain)
    {
        return RequestAsync<Domain>(
            HttpMethod.Get,
            $"/workspaces/{workspaceId}/domains/{Uri.EscapeDataString(domain)}");
    }

    /// <summary>
    /// Create a sending domain in a workspace.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<Domain> CreateDomainAsync(long workspaceId, CreateDomainRequest request)
    {
        return RequestAsync<Domain>(HttpMethod.Post, $"/workspaces/{workspaceId}/domains", request);
    }

    /// <summary>
    /// Update a domain's mutable settings (currently the email track).
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<Domain> UpdateDomainAsync(long workspaceId, string domain, UpdateDomainRequest request)
    {
        return RequestAsync<Domain>(
            HttpMethod.Patch,
            $"/workspaces/{workspaceId}/domains/{Uri.EscapeDataString(domain)}",
            request);
    }

    /// <summary>
    /// Trigger DNS verification for a domain.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<Domain> VerifyDomainAsync(long workspaceId, string domain)
    {
        return RequestAsync<Domain>(
            HttpMethod.Post,
            $"/workspaces/{workspaceId}/domains/{Uri.EscapeDataString(domain)}/verify");
    }

    /// <summary>
    /// Delete a sending domain from a workspace.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<SuccessResponse> DeleteDomainAsync(long workspaceId, string domain)
    {
        return RequestAsync<SuccessResponse>(
            HttpMethod.Delete,
            $"/workspaces/{workspaceId}/domains/{Uri.EscapeDataString(domain)}");
    }

    /// <summary>
    /// List domains whose spam complaint ratio reached a critical level.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<ListDomainSpamRatioRadarResponse> ListDomainSpamRatioRadarAsync(
        ListDomainSpamRatioRadarParams? parameters = null)
    {
        return RequestAsync<ListDomainSpamRatioRadarResponse>(
            HttpMethod.Get,
            "/analytics/radar/domain-spam-ratio",
            query: parameters?.ToQuery());
    }

    /// <summary>
    /// List daily Google Postmaster Tools spam-rate reports.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<ListGooglePostmasterSpamReportsResponse> ListGooglePostmasterSpamReportsAsync(
        ListGooglePostmasterSpamReportsParams? parameters = null)
    {
        return RequestAsync<ListGooglePostmasterSpamReportsResponse>(
            HttpMethod.Get,
            "/analytics/google-postmaster/spam-reports",
            query: parameters?.ToQuery());
    }

    /// <summary>
    /// List daily Microsoft SNDS reports for the organization's sending IPs.
    /// Uses the organization API host (https://api.laneful.net).
    /// </summary>
    public Task<ListSndsReportsResponse> ListSndsReportsAsync(ListSndsReportsParams? parameters = null)
    {
        return RequestAsync<ListSndsReportsResponse>(
            HttpMethod.Get,
            "/analytics/microsoft-snds/reports",
            query: parameters?.ToQuery());
    }

    private async Task<T> RequestAsync<T>(
        HttpMethod method,
        string path,
        object? body = null,
        IReadOnlyList<KeyValuePair<string, string>>? query = null)
    {
        var json = await SendRequestAsync(method, path, body, query);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
               ?? throw new HttpException("Failed to decode JSON response: empty body", 200);
    }

    private async Task<string> SendRequestAsync(
        HttpMethod method,
        string path,
        object? body = null,
        IReadOnlyList<KeyValuePair<string, string>>? query = null)
    {
        try
        {
            var url = AppendQuery(BuildUrl(path), query);
            var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                var jsonBody = JsonSerializer.Serialize(body, _jsonOptions);
                _logger?.LogDebug("Request body: {RequestBody}", jsonBody);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            return HandleResponse(response, responseBody, url);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpException($"HTTP request failed: {ex.Message}", 0, ex);
        }
    }

    private string BuildUrl(string endpoint)
    {
        var cleanBaseUrl = _baseUrl.EndsWith("/") ? _baseUrl[..^1] : _baseUrl;
        var cleanEndpoint = endpoint.StartsWith("/") ? endpoint : $"/{endpoint}";
        return $"{cleanBaseUrl}/{ApiVersion}{cleanEndpoint}";
    }

    private static string AppendQuery(string url, IReadOnlyList<KeyValuePair<string, string>>? query)
    {
        if (query == null || query.Count == 0)
            return url;

        var parts = query
            .Where(kv => !string.IsNullOrEmpty(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}")
            .ToList();

        return parts.Count == 0 ? url : $"{url}?{string.Join("&", parts)}";
    }

    private static string HandleResponse(HttpResponseMessage response, string responseBody, string url)
    {
        var statusCode = (int)response.StatusCode;

        if (statusCode == 404)
        {
            throw new HttpException(
                $"API endpoint not found (404). Check your base URL. Requested: {url}",
                statusCode
            );
        }

        Dictionary<string, object>? data = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            }
        }
        catch (JsonException ex)
        {
            var truncatedBody = responseBody.Length > 500 ? responseBody[..500] + "..." : responseBody;
            throw new HttpException(
                $"Failed to decode JSON response: {ex.Message}. Response body: {truncatedBody}. URL: {url}",
                statusCode,
                ex
            );
        }

        if (statusCode >= 200 && statusCode < 300)
            return responseBody;

        var errorMessage = data?.GetValueOrDefault("error")?.ToString() ?? "Unknown API error";
        var details = data?.GetValueOrDefault("details")?.ToString();
        var fullError = string.IsNullOrEmpty(details) ? errorMessage : $"{errorMessage} - {details}";

        throw new ApiException(
            $"API request failed to {url}",
            statusCode,
            fullError
        );
    }

    /// <summary>
    /// Disposes the HTTP client.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
