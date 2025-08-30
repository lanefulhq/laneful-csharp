using System.Text.Json.Serialization;
using Laneful.Exceptions;

namespace Laneful.Models;

/// <summary>
/// Represents an email address with an optional name.
/// </summary>
public record Address
{
    [JsonPropertyName("email")]
    public string Email { get; }

    [JsonPropertyName("name")]
    public string? Name { get; }

    /// <summary>
    /// Creates a new Address with email and optional name.
    /// </summary>
    /// <param name="email">The email address (required)</param>
    /// <param name="name">The display name (optional)</param>
    /// <exception cref="ValidationException">Thrown when email is null or invalid</exception>
    public Address(string email, string? name = null)
    {
        Email = email;
        Name = name;
        Validate();
    }

    /// <summary>
    /// Creates an Address from a dictionary representation.
    /// </summary>
    /// <param name="data">Dictionary containing email and optional name</param>
    /// <returns>New Address instance</returns>
    /// <exception cref="ValidationException">Thrown when data is invalid</exception>
    public static Address FromDictionary(Dictionary<string, object> data)
    {
        var email = data.GetValueOrDefault("email")?.ToString();
        var name = data.GetValueOrDefault("name")?.ToString();
        
        if (email == null)
            throw new ValidationException("Email address is required");
            
        return new Address(email, name);
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Email))
            throw new ValidationException("Email address cannot be empty");

        // Basic email validation
        var emailRegex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9+_.-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
        if (!emailRegex.IsMatch(Email))
            throw new ValidationException($"Invalid email address format: {Email}");
    }

    public override string ToString()
    {
        return !string.IsNullOrWhiteSpace(Name) ? $"{Name} <{Email}>" : Email;
    }
}

