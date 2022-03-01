# NATS Health Check

This health check verifies the ability to communicate with a [NATS server](https://nats.io/about/). \
It relies on `NATS.Client` package. \
Latest tag for the [official dockerhub image](https://hub.docker.com/_/nats/) is `2.6.6`.

## Builder Extension

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddHealthChecks()
        .AddNats(options =>
        {
            options.Url = "nats://demo.nats.io:4222";
            options.CredentialsPath = "The full path to a chained credentials file.";
            options.Jwt = "The path to a user's public JWT credentials.";
            options.PrivateNKey = "The path to a file for user user's private Nkey seed.";
        });
}
```

`Url` property is NATS server url and is **mandatory**. \
There is a demo instance `nats://demo.nats.io:4222` managed by nats.io and this is the default value for the url property. \
The rest of the properties in `NatsOptions` are optional. \
Docker image produces `nats://localhost:4222`. \
Url might also be a string containing multiple URLs to the NATS Server, e.g. `nats://localhost:4222, nats://localhost:8222`.

See [NKeys](https://docs.nats.io/running-a-nats-service/configuration/securing_nats/auth_intro/nkey_auth) for reference to the `PrivateNKey` and `Jwt` properties.

See [Authenticating with a Credentials File](https://docs.nats.io/using-nats/developer/connecting/creds) for details related to the `CredentialsPath` property.

The setup action used by the extension method caters for all three overloads supplied by the Nats client to create a connection off of a connection factory.

```csharp
namespace NATS.Client
{
    public sealed class ConnectionFactory
    {
        public IConnection CreateConnection(string url);
        public IConnection CreateConnection(string url, string credentialsPath);
        public IConnection CreateConnection(string url, string jwt, string privateNkey);
    }
}
```

Like all `IHealthChecksBuilder` extensions, all the following parameters have type `default` values and may be overridden:

- `name`: The health check name. Default if not specified is `nats`.
- `failureStatus`: The `HealthStatus` that should be reported when the health check fails. Default is `HealthStatus.Unhealthy`.
- `tags`: A list of tags that can be used to filter sets of health checks.
- `timeout`: A `System.TimeSpan` representing the timeout of the check.

[<<](../../README.md)
