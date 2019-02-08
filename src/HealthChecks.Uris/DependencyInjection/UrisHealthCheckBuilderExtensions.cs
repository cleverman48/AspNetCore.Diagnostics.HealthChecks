﻿using HealthChecks.Uris;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UrisHealthCheckBuilderExtensions
    {
        const string NAME = "uri-group";

        /// <summary>
        /// Add a health check for single uri.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="uri">The uri to check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'uri-group' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns></param>
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Uri uri, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            builder.Services.AddHttpClient();

            var options = new UriHealthCheckOptions();
            options.AddUri(uri);

            var registrationName = name ?? NAME;
            return builder.Add(new HealthCheckRegistration(
                registrationName,
                sp => CreateHealthCheck(sp, registrationName, options),
                failureStatus,
                tags));
        }
        /// <summary>
        /// Add a health check for single uri.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="uri">The uri to check.</param>
        /// <param name="httpMethod">The http method to use on check.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'uri-group' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns></param>
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Uri uri, HttpMethod httpMethod, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            builder.Services.AddHttpClient();

            var options = new UriHealthCheckOptions();
            options.AddUri(uri);
            options.UseHttpMethod(httpMethod);

            var registrationName = name ?? NAME;
            return builder.Add(new HealthCheckRegistration(
                registrationName,
                sp => CreateHealthCheck(sp, registrationName, options),
                failureStatus,
                tags));
        }
        /// <summary>
        /// Add a health check for multiple uri's.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="uris">The collection of uri's to be checked.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'uri-group' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, IEnumerable<Uri> uris, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            builder.Services.AddHttpClient();

            var options = UriHealthCheckOptions.CreateFromUris(uris);

            var registrationName = name ?? NAME;
            return builder.Add(new HealthCheckRegistration(
                registrationName,
                sp => CreateHealthCheck(sp, registrationName, options),
                failureStatus,
                tags));
        }
        /// <summary>
        /// Add a health check for multiple uri's.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="uris">The collection of uri's to be checked.</param>
        /// <param name="httpMethod">The http method to be used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'uri-group' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, IEnumerable<Uri> uris, HttpMethod httpMethod, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            builder.Services.AddHttpClient();

            var options = UriHealthCheckOptions.CreateFromUris(uris);
            options.UseHttpMethod(httpMethod);

            var registrationName = name ?? NAME;
            return builder.Add(new HealthCheckRegistration(
                registrationName,
                sp => CreateHealthCheck(sp, registrationName, options),
                failureStatus,
                tags));
        }
        /// <summary>
        /// Add a health check for multiple uri's.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="uriOptions">The action used to configured uri values and specified http methods to be checked.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'uri-group' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        public static IHealthChecksBuilder AddUrlGroup(this IHealthChecksBuilder builder, Action<UriHealthCheckOptions> uriOptions, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default)
        {
            builder.Services.AddHttpClient();

            var options = new UriHealthCheckOptions();
            uriOptions?.Invoke(options);

            var registrationName = name ?? NAME;
            return builder.Add(new HealthCheckRegistration(
                registrationName,
                sp => CreateHealthCheck(sp, registrationName, options),
                failureStatus,
                tags));
        }
        static UriHealthCheck CreateHealthCheck(IServiceProvider sp, string name, UriHealthCheckOptions options)
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new UriHealthCheck(options, () => httpClientFactory.CreateClient(name));
        }
    }
}
