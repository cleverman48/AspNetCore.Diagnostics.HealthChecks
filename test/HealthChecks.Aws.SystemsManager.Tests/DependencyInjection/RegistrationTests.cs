using Amazon;
using Amazon.Runtime;

namespace HealthChecks.Aws.SystemsManager.Tests.DependencyInjection;

public class aws_systems_manager_registration_should
{
    [Fact]
    public void add_health_check_when_properly_configured()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddSystemsManager(setup =>
            {
                setup.Credentials = new BasicAWSCredentials("access-key", "secret-key");
                setup.AddParameter("parameter-name");
            });

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("aws systems manager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
    }

    [Fact]
    public void add_health_check_with_assumerole_when_properly_configured()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddSystemsManager(setup =>
            {
                setup.Credentials = new AssumeRoleAWSCredentials(null, "role-arn", "session-name");
                setup.AddParameter("parameter-name");
            });

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("aws systems manager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
    }

    [Fact]
    public void add_named_health_check_when_properly_configured()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddSystemsManager(setup =>
            {
                setup.Credentials = new BasicAWSCredentials("access-key", "secret-key");
            }, name: "awssystemsmanager");

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("awssystemsmanager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
    }

    [Fact]
    public void add_health_check_when_no_credentials_provided()
    {
        var services = new ServiceCollection();
        var setupCalled = false;

        services.AddHealthChecks()
            .AddSystemsManager(_ => setupCalled = true, name: "awssystemsmanager");

        using var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("awssystemsmanager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
        setupCalled.Should().BeTrue();
    }

    [Fact]
    public void add_health_check_when_no_credentials_but_region_provided()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddSystemsManager(setup =>
            {
                setup.RegionEndpoint = RegionEndpoint.EUCentral1;
            });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("aws systems manager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
    }

    [Fact]
    public void add_health_check_when_credentials_and_region_provided()
    {
        var services = new ServiceCollection();
        services.AddHealthChecks()
            .AddSystemsManager(setup =>
            {
                setup.Credentials = new BasicAWSCredentials("access-key", "secret-key");
                setup.RegionEndpoint = RegionEndpoint.EUCentral1;
            });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

        var registration = options.Value.Registrations.First();
        var check = registration.Factory(serviceProvider);

        registration.Name.Should().Be("aws systems manager");
        check.GetType().Should().Be(typeof(SystemsManagerHealthCheck));
    }
}
