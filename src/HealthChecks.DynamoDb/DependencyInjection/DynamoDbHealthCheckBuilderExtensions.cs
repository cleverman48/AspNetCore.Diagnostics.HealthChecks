using HealthChecks.DynamoDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure <see cref="DynamoDbHealthCheck"/>.
    /// </summary>
    public static class DynamoDbHealthCheckBuilderExtensions
    {
        private const string NAME = "dynamodb";

        /// <summary>
        /// Add a health check for AWS DynamoDb database.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup">The action to configure the DynamoDb connection parameters.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'dynamodb' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddDynamoDb(
            this IHealthChecksBuilder builder,
            Action<DynamoDBOptions>? setup,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            var options = new DynamoDBOptions();
            setup?.Invoke(options);

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new DynamoDbHealthCheck(options),
                failureStatus,
                tags,
                timeout));
        }
    }
}
