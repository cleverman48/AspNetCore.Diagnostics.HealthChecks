using HealthChecks.NpgSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure <see cref="NpgSqlHealthCheck"/>.
/// </summary>
public static class NpgSqlHealthCheckBuilderExtensions
{
    private const string NAME = "npgsql";
    internal const string HEALTH_QUERY = "SELECT 1;";

    /// <summary>
    /// Add a health check for Postgres databases.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="connectionString">The Postgres connection string to be used.</param>
    /// <param name="healthQuery">The query to be used in check.</param>
    /// <param name="configure">An optional action to allow additional Npgsql specific configuration.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddNpgSql(
        this IHealthChecksBuilder builder,
        string connectionString,
        string healthQuery = HEALTH_QUERY,
        Action<NpgsqlConnection>? configure = null,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        return builder.AddNpgSql(_ => connectionString, healthQuery, configure, name, failureStatus, tags, timeout);
    }

    /// <summary>
    /// Add a health check for Postgres databases.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="connectionStringFactory">A factory to build the Postgres connection string to use.</param>
    /// <param name="healthQuery">The query to be used in check.</param>
    /// <param name="configure">An optional action to allow additional Npgsql specific configuration.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddNpgSql(
        this IHealthChecksBuilder builder,
        Func<IServiceProvider, string> connectionStringFactory,
        string healthQuery = HEALTH_QUERY,
        Action<NpgsqlConnection>? configure = null,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        return builder.AddNpgSql(sp => new NpgsqlDataSourceBuilder(connectionStringFactory(sp)).Build(), healthQuery, configure, name, failureStatus, tags, timeout);
    }

    /// <summary>
    /// Add a health check for Postgres databases.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="dbDataSourceFactory">A factory to build the NpgsqlDataSource to use.</param>
    /// <param name="healthQuery">The query to be used in check.</param>
    /// <param name="configure">An optional action to allow additional Npgsql specific configuration.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddNpgSql(
        this IHealthChecksBuilder builder,
        Func<IServiceProvider, NpgsqlDataSource> dbDataSourceFactory,
        string healthQuery = HEALTH_QUERY,
        Action<NpgsqlConnection>? configure = null,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        Guard.ThrowIfNull(dbDataSourceFactory);

        NpgsqlDataSource? dataSource = null;
        NpgSqlHealthCheckOptions options = new()
        {
            CommandText = healthQuery,
            Configure = configure,
        };

        return builder.Add(new HealthCheckRegistration(
            name ?? NAME,
            sp =>
            {
                // The Data Source needs to be created only once,
                // as each instance has it's own connection pool.
                // See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/1993 for more details.

                // Perform an atomic read of the current value.
                NpgsqlDataSource? existingDataSource = Volatile.Read(ref dataSource);
                if (existingDataSource is null)
                {
                    // Create a new Data Source
                    NpgsqlDataSource fromFactory = dbDataSourceFactory(sp);
                    // Try to resolve the Data Source from DI.
                    NpgsqlDataSource? fromDI = sp.GetService<NpgsqlDataSource>();

                    if (fromDI is not null && fromDI.ConnectionString.Equals(fromFactory.ConnectionString))
                    {
                        // If they are using the same ConnectionString, we can reuse the instance from DI.
                        // So there is only ONE NpgsqlDataSource per the whole app and ONE connection pool.

                        if (!ReferenceEquals(fromDI, fromFactory))
                        {
                            // Dispose it, as long as it's not the same instance.
                            fromFactory.Dispose();
                        }
                        Interlocked.Exchange(ref dataSource, fromDI);
                        options.DataSource = fromDI;
                    }
                    else
                    {
                        // Perform an atomic exchange, but only if the value is still null.
                        existingDataSource = Interlocked.CompareExchange(ref dataSource, fromFactory, null);
                        if (existingDataSource is not null)
                        {
                            // Some other thread has created the data source in the meantime,
                            // we dispose our own copy, and use the existing instance.
                            fromFactory.Dispose();
                            options.DataSource = existingDataSource;
                        }
                        else
                        {
                            options.DataSource = fromFactory;
                        }
                    }
                }

                return new NpgSqlHealthCheck(options);
            },
            failureStatus,
            tags,
            timeout));
    }

    /// <summary>
    /// Add a health check for Postgres databases.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="options">Options for health check.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
    /// <returns>The specified <paramref name="builder"/>.</returns>
    public static IHealthChecksBuilder AddNpgSql(
        this IHealthChecksBuilder builder,
        NpgSqlHealthCheckOptions options,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        Guard.ThrowIfNull(options);

        return builder.Add(new HealthCheckRegistration(
            name ?? NAME,
            _ => new NpgSqlHealthCheck(options),
            failureStatus,
            tags,
            timeout));
    }
}
