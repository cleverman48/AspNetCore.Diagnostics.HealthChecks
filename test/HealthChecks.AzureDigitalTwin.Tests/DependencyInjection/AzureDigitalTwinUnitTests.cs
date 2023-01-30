namespace HealthChecks.AzureDigitalTwin.Tests;

public class azure_digital_twin_registration_should
{
    [Fact]
    public void add_health_check_when_properly_configured()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin("MyDigitalTwinClientId", "MyDigitalTwinClientSecret", "TenantId");

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.ShouldBe("azuredigitaltwin");
        check.ShouldBeOfType<AzureDigitalTwinSubscriptionHealthCheck>();
    }

    [Fact]
    public void add_named_health_check_when_properly_configured()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin("MyDigitalTwinClientId", "MyDigitalTwinClientSecret", "TenantId", name: "azuredigitaltwincheck");

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.ShouldBe("azuredigitaltwincheck");
        check.ShouldBeOfType<AzureDigitalTwinSubscriptionHealthCheck>();
    }

    [Fact]
    public void fail_when_no_health_check_configuration_provided()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin(string.Empty, string.Empty, string.Empty);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();

        Assert.Throws<ArgumentNullException>(() => registration.Factory(serviceProvider));
    }

    [Fact]
    public void add_health_check_when_properly_configured_by_credentials()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin(credentials: new MockServiceClientCredentials());

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.ShouldBe("azuredigitaltwin");
        check.ShouldBeOfType<AzureDigitalTwinSubscriptionHealthCheck>();
    }

    [Fact]
    public void add_named_health_check_when_properly_configured_by_credentials()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin(new MockServiceClientCredentials(), name: "azuredigitaltwincheck");

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.ShouldBe("azuredigitaltwincheck");
        check.ShouldBeOfType<AzureDigitalTwinSubscriptionHealthCheck>();
    }

    [Fact]
    public void fail_when_no_health_check_configuration_provided_by_credentials()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddAzureDigitalTwin(null!);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();

        Assert.Throws<ArgumentNullException>(() => registration.Factory(serviceProvider));
    }
}
