using System;
using System.Collections.Generic;
using HealthChecks.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure <see cref="RabbitMQHealthCheck"/>.
    /// </summary>
    public static class RabbitMQHealthCheckBuilderExtensions
    {
        private const string NAME = "rabbitmq";

        /// <summary>
        /// Add a health check for RabbitMQ services using connection string (amqp uri).
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="rabbitConnectionString">The RabbitMQ connection string to be used.</param>
        /// <param name="sslOption">The RabbitMQ ssl options. Optional. If <c>null</c>, the ssl option will counted as disabled and not used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            string rabbitConnectionString,
            SslOption? sslOption = default,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            return builder.AddRabbitMQ(new Uri(rabbitConnectionString), sslOption, name, failureStatus, tags, timeout);
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using connection string (amqp uri).
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="rabbitConnectionString">The RabbitMQ connection string to be used.</param>
        /// <param name="sslOption">The RabbitMQ ssl options. Optional. If <c>null</c>, the ssl option will counted as disabled and not used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            Uri rabbitConnectionString,
            SslOption? sslOption = default,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            builder.Services
                .AddSingleton(sp => new RabbitMQHealthCheck(rabbitConnectionString, sslOption));

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using <see cref="IConnection"/> from service provider
        /// or <see cref="IConnectionFactory"/> from service provider if none is found. At least one must be configured.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            builder.Services.AddSingleton(sp =>
            {
                var connection = sp.GetService<IConnection>();
                var connectionFactory = sp.GetService<IConnectionFactory>();

                if (connection != null)
                {
                    return new RabbitMQHealthCheck(connection);
                }
                else if (connectionFactory != null)
                {
                    return new RabbitMQHealthCheck(connectionFactory);
                }
                else
                {
                    throw new ArgumentException($"Either an IConnection or IConnectionFactory must be registered with the service provider");
                }
            });

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using <see cref="IConnection"/> factory function.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionFactory"> A factory function to provide the rabbitMQ connection </param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, IConnection> connectionFactory,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            builder.Services
                .AddSingleton(sp => new RabbitMQHealthCheck(connectionFactory(sp)));

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using <see cref="IConnectionFactory"/> factory function.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionFactoryFactory"> A factory function to provide the rabbitMQ connection factory</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, IConnectionFactory> connectionFactoryFactory,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            builder.Services
                .AddSingleton(sp => new RabbitMQHealthCheck(connectionFactoryFactory(sp)));

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using connection string (amqp uri) factory.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionStringFactory">A factory function to provide the RabbitMQ connection string (amqp uri).</param>
        /// <param name="sslOption">The RabbitMQ ssl options. Optional. If <c>null</c>, the ssl option will counted as disabled and not used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, Uri> connectionStringFactory,
            SslOption? sslOption = default,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            builder.Services
                .AddSingleton(sp => new RabbitMQHealthCheck(connectionStringFactory(sp), sslOption));

            return builder.Add(new HealthCheckRegistration(
                name ?? NAME,
                sp => sp.GetRequiredService<RabbitMQHealthCheck>(),
                failureStatus,
                tags,
                timeout));
        }

        /// <summary>
        /// Add a health check for RabbitMQ services using connection string (amqp uri) factory.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="connectionStringFactory">A factory function to provide the RabbitMQ connection string (amqp uri).</param>
        /// <param name="sslOption">The RabbitMQ ssl options. Optional. If <c>null</c>, the ssl option will counted as disabled and not used.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'rabbitmq' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddRabbitMQ(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, string> connectionStringFactory,
            SslOption? sslOption = default,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            return builder.AddRabbitMQ(sp => new Uri(connectionStringFactory(sp)), sslOption, name, failureStatus, tags, timeout);
        }
    }
}
