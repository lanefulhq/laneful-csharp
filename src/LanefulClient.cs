using System.Text.Json;
using Laneful.Exceptions;
using Laneful.Models;
using Microsoft.Extensions.Logging;

namespace Laneful;

/// <summary>
/// Main client for communicating with the Laneful email API.
/// </summary>
public class LanefulClient
{
    private const string ApiVersion = "v1";
    private const string UserAgent = "laneful-csharp/1.0.0";
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private readonly string _baseUrl;
    private readonly string _authToken;
    private readonly HttpClient _httpClient;
    private readonly ILogger<LanefulClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new LanefulClient with the specified configuration.
    /// </summary>
    /// <param name="baseUrl">The base URL of the Laneful API</param>
    /// <param name="authToken">The authentication token</param>
    /// <param name="httpClient">Optional custom HttpClient</param>
    /// <param name="logger">Optional logger for debugging</param>
    /// <exception cref="ValidationException">Thrown when input validation fails</exception>
    public LanefulClient(string baseUrl, string authToken, HttpClient? httpClient = null, ILogger<LanefulClient>? logger = null)
        : this(baseUrl, authToken, DefaultTimeout, httpClient, logger)
    {
    }

    /// <summary>
    /// Creates a new LanefulClient with custom timeout.
    /// </summary>
    /// <param name="baseUrl">The base URL of the Laneful API</param>
    /// <param name="authToken">The authentication token</param>
    /// <param name="timeout">The request timeout</param>
    /// <param name="httpClient">Optional custom HttpClient</param>
    /// <param name="logger">Optional logger for debugging</param>
    /// <exception cref="ValidationException">Thrown when input validation fails</exception>
    public LanefulClient(string baseUrl, string authToken, TimeSpan timeout, HttpClient? httpClient = null, ILogger<LanefulClient>? logger = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ValidationException("Base URL cannot be empty");

        if (string.IsNullOrWhiteSpace(authToken))
            throw new ValidationException("Auth token cannot be empty");

        _baseUrl = baseUrl.Trim();
        _authToken = authToken.Trim();
        _logger = logger;

        // Initialize JSON options
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false
        };

        // Initialize HTTP client
        _httpClient = httpClient ?? new HttpClient();
        _httpClient.Timeout = timeout;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_authToken}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        // Note: Content-Type is set on the content object, not in default headers
    }

    /// <summary>
    /// Sends a single email.
    /// </summary>
    /// <param name="email">The email to send</param>
    /// <returns>API response data</returns>
    /// <exception cref="ApiException">When the API returns an error</exception>
    /// <exception cref="HttpException">When HTTP communication fails</exception>
    /// <exception cref="ValidationException">When input validation fails</exception>
    public async Task<Dictionary<string, object>> SendEmailAsync(Email email)
    {
        return await SendEmailsAsync(new[] { email });
    }

    /// <summary>
    /// Sends multiple emails.
    /// </summary>
    /// <param name="emails">List of emails to send</param>
    /// <returns>API response data</returns>
    /// <exception cref="ApiException">When the API returns an error</exception>
    /// <exception cref="HttpException">When HTTP communication fails</exception>
    /// <exception cref="ValidationException">When input validation fails</exception>
    public async Task<Dictionary<string, object>> SendEmailsAsync(IEnumerable<Email> emails)
    {
        var emailList = emails.ToList();
        
        if (emailList.Count == 0)
            throw new ValidationException("Emails list cannot be empty");

        // Validate all emails are Email instances
        foreach (var email in emailList)
        {
            if (email == null)
                throw new ValidationException("Email cannot be null");
        }

        try
        {
            // Prepare request data
            var requestData = new Dictionary<string, object>
            {
                ["emails"] = emailList
            };

            var jsonBody = JsonSerializer.Serialize(requestData, _jsonOptions);
            _logger?.LogDebug("Request body: {RequestBody}", jsonBody);

            // Build request
            var url = BuildUrl("/email/send");
            var content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            
            // Explicitly set Content-Type header
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            // Execute request
            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            return await HandleResponseAsync(response, responseBody, url);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpException($"HTTP request failed: {ex.Message}", 0, ex);
        }
    }

    /// <summary>
    /// Builds the full API URL for an endpoint.
    /// </summary>
    /// <param name="endpoint">The API endpoint</param>
    /// <returns>Full URL</returns>
    private string BuildUrl(string endpoint)
    {
        var cleanBaseUrl = _baseUrl.EndsWith("/") ? _baseUrl[..^1] : _baseUrl;
        var cleanEndpoint = endpoint.StartsWith("/") ? endpoint : $"/{endpoint}";
        return $"{cleanBaseUrl}/{ApiVersion}{cleanEndpoint}";
    }

    /// <summary>
    /// Handles the HTTP response and converts it to a dictionary.
    /// </summary>
    /// <param name="response">The HTTP response</param>
    /// <param name="responseBody">The response body</param>
    /// <param name="url">The request URL</param>
    /// <returns>Response data as dictionary</returns>
    /// <exception cref="ApiException">When the API returns an error</exception>
    /// <exception cref="HttpException">When response parsing fails</exception>
    private Task<Dictionary<string, object>> HandleResponseAsync(HttpResponseMessage response, string responseBody, string url)
    {
        var statusCode = (int)response.StatusCode;

        // Handle 404 specifically as it likely means wrong URL
        if (statusCode == 404)
        {
            throw new HttpException(
                $"API endpoint not found (404). Check your base URL. Requested: {url}",
                statusCode
            );
        }

        // Try to decode JSON response
        Dictionary<string, object>? data = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody, _jsonOptions);
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

        // Handle successful responses
        if (statusCode >= 200 && statusCode < 300)
        {
            return Task.FromResult(data ?? new Dictionary<string, object>());
        }

        // Handle API errors
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
