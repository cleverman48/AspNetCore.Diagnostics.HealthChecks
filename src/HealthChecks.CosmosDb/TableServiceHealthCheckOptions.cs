namespace HealthChecks.CosmosDb;

/// <summary>
/// Represents a collection of settings that configure an
/// <see cref="TableServiceHealthCheck">Azure Storage Table Service health check</see>.
/// </summary>
public sealed class TableServiceHealthCheckOptions
{
    /// <summary>
    /// Gets or sets the name of the Azure Storage table whose health should be checked.
    /// </summary>
    /// <remarks>
    /// If the value is <see langword="null"/>, then no health check is performed for a specific table.
    /// </remarks>
    /// <value>An optional Azure Storage table name.</value>
    public string? TableName { get; set; }
}
