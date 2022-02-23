using System;
using System.Collections.Generic;
using HealthChecks.DocumentDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DocumentDbHealthCheckBuilderExtensions
    {
        private const string NAME = "documentdb";

        /// <summary>
        /// Add a health check for Azure DocumentDb database.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup">The action to configure the DocumentDb connection parameters.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'documentdb' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddDocumentDb(this IHealthChecksBuilder builder, Action<DocumentDbOptions> setup, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
        {
            var documentDbOptions = new DocumentDbOptions();
            setup?.Invoke(documentDbOptions);

            return builder.Add(new HealthCheckRegistration(
               name ?? NAME,
               sp => new DocumentDbHealthCheck(documentDbOptions),
               failureStatus,
               tags,
               timeout));
        }
    }
}
