﻿using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Xunit;

namespace HealthChecks.SqlServer.Tests.DependencyInjection
{
    public class sql_server_registration_should
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddSqlServer("connectionstring");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("sqlserver");
            check.GetType().Should().Be(typeof(SqlServerHealthCheck));
        }

        [Fact]
        public void invoke_beforeOpen_when_defined()
        {
            var services = new ServiceCollection();
            bool invoked = false;
            const string connectionstring = "Server=(local);Database=foo;User Id=bar;Password=baz;Connection Timeout=1";
            Action<SqlConnection> beforeOpen = connection =>
            {
                invoked = true;
                Assert.Equal(connectionstring, connection.ConnectionString);
            };
            services.AddHealthChecks()
                .AddSqlServer(connectionstring, beforeOpenConnectionConfigurer: beforeOpen);

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            Record.ExceptionAsync(() => check.CheckHealthAsync(new HealthCheckContext())).GetAwaiter().GetResult();
            Assert.True(invoked);
        }

        [Fact]
        public void add_named_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddSqlServer("connectionstring", name: "my-sql-server-1");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("my-sql-server-1");
            check.GetType().Should().Be(typeof(SqlServerHealthCheck));
        }

        [Fact]
        public void add_health_check_with_connection_string_factory_when_properly_configured()
        {
            var services = new ServiceCollection();
            var factoryCalled = false;
            services.AddHealthChecks()
                .AddSqlServer(_ =>
                {
                    factoryCalled = true;
                    return "connectionstring";
                });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("sqlserver");
            check.GetType().Should().Be(typeof(SqlServerHealthCheck));
            factoryCalled.Should().BeTrue();
        }
    }
}
