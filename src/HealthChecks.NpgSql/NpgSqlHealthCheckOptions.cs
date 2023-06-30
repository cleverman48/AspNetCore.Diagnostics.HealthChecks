using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace HealthChecks.NpgSql;

/// <summary>
/// Options for <see cref="NpgSqlHealthCheck"/>.
/// </summary>
public class NpgSqlHealthCheckOptions
{
    /// <summary>
    /// The Postgres connection string to be used.
    /// Use <see cref="DataSource"/> property for advanced configuration.
    /// </summary>
    public string? ConnectionString
    {
        get => DataSource?.ConnectionString;
        set => DataSource = value is not null ? NpgsqlDataSource.Create(value) : null;
    }

    /// <summary>
    /// The Postgres data source to be used.
    /// </summary>
    public NpgsqlDataSource? DataSource { get; set; }

    /// <summary>
    /// The query to be executed.
    /// </summary>
    public string CommandText { get; set; } = NpgSqlHealthCheckBuilderExtensions.HEALTH_QUERY;

    /// <summary>
    /// An optional action executed before the connection is opened in the health check.
    /// </summary>
    public Action<NpgsqlConnection>? Configure { get; set; }

    /// <summary>
    /// An optional delegate to build health check result.
    /// </summary>
    public Func<object?, HealthCheckResult>? HealthCheckResultBuilder { get; set; }
}
