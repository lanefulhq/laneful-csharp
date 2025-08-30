using System.Text.Json.Serialization;
using Laneful.Exceptions;

namespace Laneful.Models;

/// <summary>
/// Represents a file attachment for an email.
/// </summary>
public record Attachment
{
    [JsonPropertyName("filename")]
    public string Filename { get; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; }

    [JsonPropertyName("content")]
    public string Content { get; }

    /// <summary>
    /// Creates an attachment from a file.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>New Attachment instance</returns>
    /// <exception cref="ValidationException">Thrown when file cannot be read</exception>
    public static Attachment FromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new ValidationException($"File not found: {filePath}");

        var filename = Path.GetFileName(filePath);
        var contentType = GetContentType(filePath);
        var content = Convert.ToBase64String(File.ReadAllBytes(filePath));

        return new Attachment(filename, contentType, content);
    }

    /// <summary>
    /// Creates an attachment from raw data.
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <param name="contentType">The MIME type</param>
    /// <param name="content">Base64-encoded content</param>
    /// <exception cref="ValidationException">Thrown when parameters are invalid</exception>
    public Attachment(string filename, string contentType, string content)
    {
        Filename = filename;
        ContentType = contentType;
        Content = content;
        Validate();
    }

    /// <summary>
    /// Creates an attachment from a dictionary representation.
    /// </summary>
    /// <param name="data">Dictionary containing attachment data</param>
    /// <returns>New Attachment instance</returns>
    /// <exception cref="ValidationException">Thrown when data is invalid</exception>
    public static Attachment FromDictionary(Dictionary<string, object> data)
    {
        var filename = data.GetValueOrDefault("filename")?.ToString();
        var contentType = data.GetValueOrDefault("content_type")?.ToString();
        var content = data.GetValueOrDefault("content")?.ToString();

        if (filename == null || contentType == null || content == null)
            throw new ValidationException("Filename, content_type, and content are required");

        return new Attachment(filename, contentType, content);
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Filename))
            throw new ValidationException("Filename cannot be empty");

        if (string.IsNullOrWhiteSpace(ContentType))
            throw new ValidationException("Content type cannot be empty");

        if (string.IsNullOrWhiteSpace(Content))
            throw new ValidationException("Content cannot be empty");
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" or ".htm" => "text/html",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }

    public override string ToString()
    {
        return $"Attachment{{filename='{Filename}', contentType='{ContentType}', contentLength={Content?.Length ?? 0}}}";
    }
}

