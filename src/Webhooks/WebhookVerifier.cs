using System.Security.Cryptography;
using System.Text;

namespace Laneful.Webhooks;

/// <summary>
/// Utility class for verifying webhook signatures.
/// </summary>
public static class WebhookVerifier
{
    private const string Algorithm = "HmacSHA256";

    /// <summary>
    /// Verifies a webhook signature.
    /// </summary>
    /// <param name="secret">The webhook secret</param>
    /// <param name="payload">The webhook payload</param>
    /// <param name="signature">The signature to verify</param>
    /// <returns>true if the signature is valid, false otherwise</returns>
    public static bool VerifySignature(string secret, string payload, string signature)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret cannot be empty", nameof(secret));

        if (payload == null)
            throw new ArgumentException("Payload cannot be null", nameof(payload));

        if (string.IsNullOrWhiteSpace(signature))
            return false;

        try
        {
            var expectedSignature = GenerateSignature(secret, payload);
            return ConstantTimeEquals(expectedSignature, signature);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a signature for the given payload.
    /// </summary>
    /// <param name="secret">The webhook secret</param>
    /// <param name="payload">The payload to sign</param>
    /// <returns>The generated signature</returns>
    public static string GenerateSignature(string secret, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Compares two strings in constant time to prevent timing attacks.
    /// </summary>
    /// <param name="a">First string</param>
    /// <param name="b">Second string</param>
    /// <returns>true if strings are equal, false otherwise</returns>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}

