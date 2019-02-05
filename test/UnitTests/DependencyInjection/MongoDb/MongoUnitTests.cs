﻿using FluentAssertions;
using HealthChecks.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Linq;
using MongoDB.Driver;
using Xunit;

namespace UnitTests.HealthChecks.DependencyInjection.MongoDb
{
    public class mongodb_registration_should
    {
        [Fact]
        public void add_health_check_when_properly_configured_connectionString()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddMongoDb("mongodb://connectionstring");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("mongodb");
            check.GetType().Should().Be(typeof(MongoDbHealthCheck));
        }
        [Fact]
        public void add_health_check_when_properly_configured_mongoClientSettings()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddMongoDb(new MongoClient("mongodb://connectionstring").Settings.Clone());

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("mongodb");
            check.GetType().Should().Be(typeof(MongoDbHealthCheck));
        }
        [Fact]
        public void add_named_health_check_when_properly_configured_connectionString()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddMongoDb("mongodb://connectionstring", name: "my-mongodb-group");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("my-mongodb-group");
            check.GetType().Should().Be(typeof(MongoDbHealthCheck));
        }
        [Fact]
        public void add_named_health_check_when_properly_configured_mongoClientSettings()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddMongoDb(new MongoClient("mongodb://connectionstring").Settings.Clone(), name: "my-mongodb-group");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("my-mongodb-group");
            check.GetType().Should().Be(typeof(MongoDbHealthCheck));
        }
    }
}
