﻿using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.HealthChecks.RabbitMQ
{
    [Collection("execution")]
    public class rabbitmq_healthcheck_should
    {
        private readonly ExecutionFixture _fixture;

        public rabbitmq_healthcheck_should(ExecutionFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [SkipOnAppVeyor]
        public async Task be_healthy_if_rabbitmq_is_available()
        {
            var connectionString = @"amqp://localhost:5672";

            var webHostBuilder = new WebHostBuilder()
            .UseStartup<DefaultStartup>()
            .ConfigureServices(services =>
            {
                services.AddHealthChecks()
                 .AddRabbitMQ(connectionString);
            })
            .Configure(app =>
            {
                app.UseHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = r => r.Tags.Contains("rabbitmq")
                });
            });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"/health")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        [SkipOnAppVeyor]
        public async Task be_unhealthy_if_rabbitmq_is_not_available()
        {
            var webHostBuilder = new WebHostBuilder()
           .UseStartup<DefaultStartup>()
           .ConfigureServices(services =>
           {
               services.AddHealthChecks()
                .AddRabbitMQ("amqp://nonexistingdomain:5672");
           })
           .Configure(app =>
           {
               app.UseHealthChecks("/health", new HealthCheckOptions()
               {
                   Predicate = r => r.Tags.Contains("rabbitmq")
               });
           });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"/health")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);
        }
    }
}
