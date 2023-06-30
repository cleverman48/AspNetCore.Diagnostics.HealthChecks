using HealthChecks.Publisher.Seq;

namespace Microsoft.Extensions.DependencyInjection;

public class SeqOptions
{
    public string Endpoint { get; set; } = null!;

    public string? ApiKey { get; set; }

    public SeqInputLevel DefaultInputLevel { get; set; }

    /// <summary>
    /// An optional action executed before the metrics are pushed to Seq.
    /// Useful to push additional static properties to Seq.
    /// </summary>
    public Action<RawEvents>? Configure { get; set; } = null;
}
