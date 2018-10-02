﻿using HealthChecks.NpgSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NphSqlHealthCheckBuilderExtensions
    {
        const string NAME = "npgsql";

        /// <summary>
        /// Add a health check for Postgres databases.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="npgsqlConnectionString">The Postgres connection string to be used.</param>
        /// /// <param name="healthQuery">The query to be used in check. Optional. If <c>null</c> SELECT 1 is used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'npgsql' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns></param>
        public static IHealthChecksBuilder AddNpgSql(this IHealthChecksBuilder builder, string npgsqlConnectionString, string healthQuery = "SELECT 1;", string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => new NpgSqlHealthCheck(npgsqlConnectionString, healthQuery),
                failureStatus,
                tags));
        }
    }
}
