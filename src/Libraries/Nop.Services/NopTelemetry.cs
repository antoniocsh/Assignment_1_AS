using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Nop.Services;

/// <summary>
/// Core telemetry classes for custom instrumentation
/// </summary>
public static class NopTelemetry
{
    // ActivitySource for Distributed Tracing
    public static readonly ActivitySource Source = new("NopCommerce.Catalog", "1.0.0");

    // Meter for Custom Metrics
    public static readonly Meter Meter = new("NopCommerce.Metrics", "1.0.0");

    // Custom Metric 1: Search Duration
    public static readonly Histogram<double> SearchDurationToHistogram = Meter.CreateHistogram<double>(
        "nopcommerce.catalog.search_duration",
        unit: "ms",
        description: "Duration of product search operations. Indicates latency or degraded performance in catalog search.");

    // Custom Metric 2: Product Not Found Count
    public static readonly Counter<long> ProductNotFoundCount = Meter.CreateCounter<long>(
        "nopcommerce.catalog.product_not_found_count",
        description: "Number of times a requested product was not found. High amount implies broken links or bad caching.");
        
    // Custom Metric 3: Product cache hits/misses
    public static readonly Counter<long> ProductCacheHits = Meter.CreateCounter<long>(
        "nopcommerce.catalog.cache_hits_count",
        description: "Number of times a product was served from the cache.");
        
    public static readonly Counter<long> ProductCacheMisses = Meter.CreateCounter<long>(
        "nopcommerce.catalog.cache_misses_count",
        description: "Number of times a product was not found in cache and had to be generated.");
}
