using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;

namespace HealthChecks.MySql;

/// <summary>
/// A health check for MySQL databases.
/// </summary>
public class MySqlHealthCheck : IHealthCheck
{
    private readonly MySqlHealthCheckOptions _options;

    public MySqlHealthCheck(MySqlHealthCheckOptions options)
    {
        Guard.ThrowIfNull(options);
        if (options.DataSource is null && options.ConnectionString is null)
            throw new InvalidOperationException("One of options.DataSource or options.ConnectionString must be specified.");
        if (options.DataSource is not null && options.ConnectionString is not null)
            throw new InvalidOperationException("Only one of options.DataSource or options.ConnectionString must be specified.");
        _options = options;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _options.DataSource is not null ?
                _options.DataSource.CreateConnection() :
                new MySqlConnection(_options.ConnectionString);

            _options.Configure?.Invoke(connection);
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            if (_options.CommandText is { } commandText)
            {
                using var command = connection.CreateCommand();
                command.CommandText = _options.CommandText;
                object? result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                return _options.HealthCheckResultBuilder == null
                    ? HealthCheckResult.Healthy()
                    : _options.HealthCheckResultBuilder(result);
            }
            else
            {
                var success = await connection.PingAsync(cancellationToken).ConfigureAwait(false);
                return _options.HealthCheckResultBuilder is null
                    ? (success ? HealthCheckResult.Healthy() : new HealthCheckResult(context.Registration.FailureStatus)) :
                    _options.HealthCheckResultBuilder(success);
            }
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
