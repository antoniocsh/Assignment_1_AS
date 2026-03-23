using System.Diagnostics;
using OpenTelemetry;

namespace Nop.Services.Infrastructure;

/// <summary>
/// Processor to sanitize PII (Personally Identifiable Information) from traces
/// </summary>
public class PiiSanitiserProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        // Sanitize tags that might contain PII
        foreach (var tag in activity.TagObjects)
        {
            if (IsPiiTag(tag.Key))
            {
                // Simple redaction logic
                activity.SetTag(tag.Key, "[REDACTED]");
            }
        }

        // Specifically check for emails in any tag value
        foreach (var tag in activity.TagObjects)
        {
            if (tag.Value is string stringValue && stringValue.Contains('@'))
            {
                activity.SetTag(tag.Key, "[REDACTED EMAIL]");
            }
        }
    }

    private static bool IsPiiTag(string key)
    {
        var lowerKey = key.ToLowerInvariant();
        return lowerKey.Contains("email") || 
               lowerKey.Contains("customer.name") || 
               lowerKey.Contains("customer.email") ||
               lowerKey.Contains("phone") ||
               lowerKey.Contains("address") ||
               lowerKey.Contains("password");
    }
}
