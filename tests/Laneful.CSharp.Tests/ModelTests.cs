using System.Text.Json;
using Laneful.Exceptions;
using Laneful.Models;
using Xunit;

namespace Laneful.Tests;

public class EmailModelTests
{
    [Fact]
    public void FromHeader_SerializesAsFromHeader()
    {
        var email = new Email.Builder()
            .From(new Address("sender@example.com"))
            .To(new Address("to@example.com"))
            .Subject("Test")
            .TextContent("Content")
            .FromHeader(new Address("newsletter@example.com", "Newsletter"))
            .Build();

        Assert.Equal("newsletter@example.com", email.FromHeader?.Email);

        var json = JsonSerializer.Serialize(email, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        Assert.Contains("\"from_header\"", json);
        Assert.Contains("newsletter@example.com", json);
    }

    [Fact]
    public void WebhookData_AllowsTwentyKeys()
    {
        var data = Enumerable.Range(0, 20).ToDictionary(i => $"key{i}", i => $"value{i}");
        var email = new Email.Builder()
            .From(new Address("sender@example.com"))
            .To(new Address("to@example.com"))
            .Subject("Test")
            .TextContent("Content")
            .WebhookData(data)
            .Build();

        Assert.Equal(20, email.WebhookData!.Count);
    }

    [Fact]
    public void WebhookData_RejectsTwentyOneKeys()
    {
        var data = Enumerable.Range(0, 21).ToDictionary(i => $"key{i}", i => $"value{i}");
        var ex = Assert.Throws<ValidationException>(() =>
            new Email.Builder()
                .From(new Address("sender@example.com"))
                .To(new Address("to@example.com"))
                .Subject("Test")
                .TextContent("Content")
                .WebhookData(data)
                .Build());

        Assert.Contains("20 keys", ex.Message);
    }
}

public class TrackingSettingsTests
{
    [Fact]
    public void SerializesUnsubscribeGroupName()
    {
        var tracking = new TrackingSettings(
            opens: true,
            clicks: false,
            unsubscribes: true,
            unsubscribeGroupName: "Newsletters");

        var json = JsonSerializer.Serialize(tracking);
        Assert.Contains("\"unsubscribe_group_name\":\"Newsletters\"", json);
        Assert.DoesNotContain("unsubscribe_group_id", json);
    }
}

public class MailSettingsTests
{
    [Fact]
    public void OmitsNullsAndKeepsFalse()
    {
        var empty = JsonSerializer.Serialize(new MailSettings());
        Assert.Equal("{}", empty);

        var settings = new MailSettings(sandboxMode: true, returnMessageIds: false);
        var json = JsonSerializer.Serialize(settings);
        Assert.Contains("\"sandbox_mode\":true", json);
        Assert.Contains("\"return_message_ids\":false", json);
        Assert.DoesNotContain("idempotency_key", json);
    }

    [Fact]
    public void Address_WithoutNameOmitsName()
    {
        // The send API rejects "name": null with "JSON data error".
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        Assert.Equal("{\"email\":\"user@example.com\"}",
            JsonSerializer.Serialize(new Address("user@example.com"), options));
        Assert.Equal("{\"email\":\"user@example.com\",\"name\":\"User\"}",
            JsonSerializer.Serialize(new Address("user@example.com", "User"), options));
    }

    [Fact]
    public void IdempotencyKey_SerializesAndValidates()
    {
        var json = JsonSerializer.Serialize(new MailSettings { IdempotencyKey = "order-1234" });
        Assert.Equal("{\"idempotency_key\":\"order-1234\"}", json);

        _ = new MailSettings { IdempotencyKey = new string('k', 36) };
        foreach (var key in new[] { "", new string('k', 37), new string('é', 19) })
        {
            Assert.Throws<ValidationException>(() => new MailSettings { IdempotencyKey = key });
        }
    }
}

public class DomainModelTests
{
    [Fact]
    public void UpdateDomainRequest_ThreeWayTrack()
    {
        var omit = JsonSerializer.Serialize(new UpdateDomainRequest());
        Assert.Equal("{}", omit);

        var clear = JsonSerializer.Serialize(new UpdateDomainRequest(""));
        Assert.Contains("\"email_track_id\":\"\"", clear);

        var set = JsonSerializer.Serialize(new UpdateDomainRequest("track-1"));
        Assert.Contains("\"email_track_id\":\"track-1\"", set);
    }

    // "é" is 2 bytes in UTF-8: 25 fill a 50-byte key and 50 fill a 100-byte value.
    [Theory]
    [InlineData(25, 50, null)]
    [InlineData(26, 1, "50 bytes")]
    [InlineData(1, 51, "100 bytes")]
    public void WebhookData_CountsUtf8Bytes(int keyChars, int valueChars, string? expectedError)
    {
        var data = new Dictionary<string, string>
        {
            [new string('é', keyChars)] = new string('é', valueChars)
        };
        var builder = new Email.Builder()
            .From(new Address("sender@example.com"))
            .To(new Address("to@example.com"))
            .Subject("Test")
            .TextContent("Content")
            .WebhookData(data);

        if (expectedError == null)
        {
            Assert.Equal(data, builder.Build().WebhookData);
        }
        else
        {
            var ex = Assert.Throws<ValidationException>(() => builder.Build());
            Assert.Contains(expectedError, ex.Message);
        }
    }
}
