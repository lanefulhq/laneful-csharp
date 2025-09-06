using System.Text.Json.Serialization;
using Laneful.Exceptions;

namespace Laneful.Models;

/// <summary>
/// Represents a single email to be sent.
/// </summary>
public class Email
{
    [JsonPropertyName("from")]
    public Address From { get; } = null!;

    [JsonPropertyName("to")]
    public List<Address> To { get; }

    [JsonPropertyName("cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Address>? Cc { get; }

    [JsonPropertyName("bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Address>? Bcc { get; }

    [JsonPropertyName("subject")]
    public string Subject { get; }

    [JsonPropertyName("text_content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TextContent { get; }

    [JsonPropertyName("html_content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? HtmlContent { get; }

    [JsonPropertyName("template_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemplateId { get; }

    [JsonPropertyName("template_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? TemplateData { get; }

    [JsonPropertyName("attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Attachment>? Attachments { get; }

    [JsonPropertyName("headers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Headers { get; }

    [JsonPropertyName("reply_to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Address? ReplyTo { get; }

    [JsonPropertyName("send_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? SendTime { get; }

    [JsonPropertyName("webhook_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? WebhookData { get; }

    [JsonPropertyName("tag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Tag { get; }

    [JsonPropertyName("tracking")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TrackingSettings? Tracking { get; }

    private Email(Builder builder)
    {
        From = builder.FromAddress ?? throw new ArgumentNullException(nameof(builder.FromAddress), "From address is required");
        To = new List<Address>(builder.ToAddresses);
        Cc = builder.CcAddresses?.Count > 0 ? new List<Address>(builder.CcAddresses) : null;
        Bcc = builder.BccAddresses?.Count > 0 ? new List<Address>(builder.BccAddresses) : null;
        Subject = builder.SubjectText;
        TextContent = builder.TextContentValue;
        HtmlContent = builder.HtmlContentValue;
        TemplateId = builder.TemplateIdValue;
        TemplateData = builder.TemplateDataValue;
        Attachments = builder.AttachmentsList?.Count > 0 ? new List<Attachment>(builder.AttachmentsList) : null;
        Headers = builder.HeadersValue;
        ReplyTo = builder.ReplyToAddress;
        SendTime = builder.SendTimeValue;
        WebhookData = builder.WebhookDataValue;
        Tag = builder.TagValue;
        Tracking = builder.TrackingSettings;

        Validate();
    }

    /// <summary>
    /// Creates an Email from a dictionary representation.
    /// </summary>
    /// <param name="data">Dictionary containing email data</param>
    /// <returns>New Email instance</returns>
    /// <exception cref="ValidationException">Thrown when data is invalid</exception>
    public static Email FromDictionary(Dictionary<string, object> data)
    {
        var builder = new Builder();

        if (data.TryGetValue("from", out var fromData) && fromData is Dictionary<string, object> fromDict)
        {
            builder.From(Address.FromDictionary(fromDict));
        }

        if (data.TryGetValue("to", out var toData) && toData is List<object> toList)
        {
            foreach (var toItem in toList)
            {
                if (toItem is Dictionary<string, object> toDict)
                {
                    builder.To(Address.FromDictionary(toDict));
                }
            }
        }

        if (data.TryGetValue("cc", out var ccData) && ccData is List<object> ccList)
        {
            foreach (var ccItem in ccList)
            {
                if (ccItem is Dictionary<string, object> ccDict)
                {
                    builder.Cc(Address.FromDictionary(ccDict));
                }
            }
        }

        if (data.TryGetValue("bcc", out var bccData) && bccData is List<object> bccList)
        {
            foreach (var bccItem in bccList)
            {
                if (bccItem is Dictionary<string, object> bccDict)
                {
                    builder.Bcc(Address.FromDictionary(bccDict));
                }
            }
        }

        if (data.TryGetValue("subject", out var subjectData))
        {
            builder.Subject(subjectData.ToString() ?? string.Empty);
        }

        if (data.TryGetValue("text_content", out var textContentData))
        {
            builder.TextContent(textContentData.ToString());
        }

        if (data.TryGetValue("html_content", out var htmlContentData))
        {
            builder.HtmlContent(htmlContentData.ToString());
        }

        if (data.TryGetValue("template_id", out var templateIdData))
        {
            builder.TemplateId(templateIdData.ToString());
        }

        if (data.TryGetValue("template_data", out var templateDataData) && templateDataData is Dictionary<string, object> templateDataDict)
        {
            builder.TemplateData(templateDataDict);
        }

        if (data.TryGetValue("attachments", out var attachmentsData) && attachmentsData is List<object> attachmentsList)
        {
            foreach (var attachmentItem in attachmentsList)
            {
                if (attachmentItem is Dictionary<string, object> attachmentDict)
                {
                    builder.Attachment(Attachment.FromDictionary(attachmentDict));
                }
            }
        }

        if (data.TryGetValue("headers", out var headersData) && headersData is Dictionary<string, object> headersDict)
        {
            var headers = headersDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString() ?? string.Empty);
            builder.Headers(headers);
        }

        if (data.TryGetValue("reply_to", out var replyToData) && replyToData is Dictionary<string, object> replyToDict)
        {
            builder.ReplyTo(Address.FromDictionary(replyToDict));
        }

        if (data.TryGetValue("send_time", out var sendTimeData) && sendTimeData is long sendTime)
        {
            builder.SendTime(sendTime);
        }

        if (data.TryGetValue("webhook_data", out var webhookDataData) && webhookDataData is Dictionary<string, object> webhookDataDict)
        {
            var webhookData = webhookDataDict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString() ?? string.Empty);
            builder.WebhookData(webhookData);
        }

        if (data.TryGetValue("tag", out var tagData))
        {
            builder.Tag(tagData.ToString());
        }

        if (data.TryGetValue("tracking", out var trackingData) && trackingData is Dictionary<string, object> trackingDict)
        {
            builder.Tracking(TrackingSettings.FromDictionary(trackingDict));
        }

        return builder.Build();
    }

    private void Validate()
    {
        if (From == null)
            throw new ValidationException("From address is required");

        // Must have at least one recipient
        if ((To?.Count ?? 0) == 0 && (Cc?.Count ?? 0) == 0 && (Bcc?.Count ?? 0) == 0)
            throw new ValidationException("Email must have at least one recipient (to, cc, or bcc)");

        // Must have either content or template
        var hasContent = !string.IsNullOrWhiteSpace(TextContent) || !string.IsNullOrWhiteSpace(HtmlContent);
        var hasTemplate = !string.IsNullOrWhiteSpace(TemplateId);

        if (!hasContent && !hasTemplate)
            throw new ValidationException("Email must have either content (text/HTML) or a template ID");

        // Validate send time
        if (SendTime.HasValue && SendTime.Value <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            throw new ValidationException("Send time must be in the future");
    }

    public override string ToString()
    {
        return $"Email{{from={From}, to={To?.Count ?? 0}, subject='{Subject}', hasTextContent={!string.IsNullOrWhiteSpace(TextContent)}, hasHtmlContent={!string.IsNullOrWhiteSpace(HtmlContent)}, templateId='{TemplateId}', attachments={Attachments?.Count ?? 0}}}";
    }

    /// <summary>
    /// Builder for creating Email instances.
    /// </summary>
    public class Builder
    {
        internal Address? FromAddress { get; private set; }
        internal List<Address> ToAddresses { get; } = new();
        internal List<Address> CcAddresses { get; } = new();
        internal List<Address> BccAddresses { get; } = new();
        internal string SubjectText { get; private set; } = string.Empty;
        internal string? TextContentValue { get; private set; }
        internal string? HtmlContentValue { get; private set; }
        internal string? TemplateIdValue { get; private set; }
        internal Dictionary<string, object>? TemplateDataValue { get; private set; }
        internal List<Attachment> AttachmentsList { get; } = new();
        internal Dictionary<string, string>? HeadersValue { get; private set; }
        internal Address? ReplyToAddress { get; private set; }
        internal long? SendTimeValue { get; private set; }
        internal Dictionary<string, string>? WebhookDataValue { get; private set; }
        internal string? TagValue { get; private set; }
        internal TrackingSettings? TrackingSettings { get; private set; }

        public Builder From(Address from)
        {
            FromAddress = from;
            return this;
        }

        public Builder To(Address to)
        {
            ToAddresses.Add(to);
            return this;
        }

        public Builder To(string email, string? name = null)
        {
            return To(new Address(email, name));
        }

        public Builder Cc(Address cc)
        {
            CcAddresses.Add(cc);
            return this;
        }

        public Builder Cc(string email, string? name = null)
        {
            return Cc(new Address(email, name));
        }

        public Builder Bcc(Address bcc)
        {
            BccAddresses.Add(bcc);
            return this;
        }

        public Builder Bcc(string email, string? name = null)
        {
            return Bcc(new Address(email, name));
        }

        public Builder Subject(string subject)
        {
            SubjectText = subject;
            return this;
        }

        public Builder TextContent(string? textContent)
        {
            TextContentValue = textContent;
            return this;
        }

        public Builder HtmlContent(string? htmlContent)
        {
            HtmlContentValue = htmlContent;
            return this;
        }

        public Builder TemplateId(string? templateId)
        {
            TemplateIdValue = templateId;
            return this;
        }

        public Builder TemplateData(Dictionary<string, object>? templateData)
        {
            TemplateDataValue = templateData;
            return this;
        }

        public Builder Attachment(Attachment attachment)
        {
            AttachmentsList.Add(attachment);
            return this;
        }

        public Builder Headers(Dictionary<string, string>? headers)
        {
            HeadersValue = headers;
            return this;
        }

        public Builder ReplyTo(Address? replyTo)
        {
            ReplyToAddress = replyTo;
            return this;
        }

        public Builder ReplyTo(string email, string? name = null)
        {
            return ReplyTo(new Address(email, name));
        }

        public Builder SendTime(long? sendTime)
        {
            SendTimeValue = sendTime;
            return this;
        }

        public Builder WebhookData(Dictionary<string, string>? webhookData)
        {
            WebhookDataValue = webhookData;
            return this;
        }

        public Builder Tag(string? tag)
        {
            TagValue = tag;
            return this;
        }

        public Builder Tracking(TrackingSettings? tracking)
        {
            TrackingSettings = tracking;
            return this;
        }

        public Email Build()
        {
            return new Email(this);
        }
    }
}
