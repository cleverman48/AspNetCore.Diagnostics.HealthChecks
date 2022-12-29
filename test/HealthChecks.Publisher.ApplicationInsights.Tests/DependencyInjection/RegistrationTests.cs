using Microsoft.ApplicationInsights.Extensibility;

namespace HealthChecks.Publisher.ApplicationInsights.Tests.DependencyInjection
{
    public class application_insights_publisher_registration_should
    {
        [Fact]
        public void add_healthcheck_when_properly_configured_with_instrumentation_key_parameter()
        {
            var services = new ServiceCollection();
            services
                .AddHealthChecks()
                .AddApplicationInsightsPublisher(instrumentationKey: "telemetrykey");

            using var serviceProvider = services.BuildServiceProvider();
            var publisher = serviceProvider.GetService<IHealthCheckPublisher>();

            Assert.NotNull(publisher);
        }

        [Fact]
        public void add_healthcheck_when_properly_configured_with_connection_string_parameter()
        {
            var services = new ServiceCollection();
            services
                .AddHealthChecks()
                .AddApplicationInsightsPublisher(connectionString: "InstrumentationKey=telemetrykey;EndpointSuffix=example.com;");

            using var serviceProvider = services.BuildServiceProvider();
            var publisher = serviceProvider.GetService<IHealthCheckPublisher>();

            Assert.NotNull(publisher);
        }

        [Fact]
        public void add_healthcheck_when_application_insights_is_properly_configured_with_IOptions()
        {
            var services = new ServiceCollection();
            services.Configure<TelemetryConfiguration>(config => config.InstrumentationKey = "telemetrykey");

            services
                .AddHealthChecks()
                .AddApplicationInsightsPublisher();

            using var serviceProvider = services.BuildServiceProvider();
            var publisher = serviceProvider.GetService<IHealthCheckPublisher>();

            Assert.NotNull(publisher);
        }
    }
}
