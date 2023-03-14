using System.Net.Sockets;
#if !NET5_0_OR_GREATER
using HealthChecks.Network.Extensions;
#endif
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Network;

public class TcpHealthCheck : IHealthCheck
{
    private readonly TcpHealthCheckOptions _options;

    public TcpHealthCheck(TcpHealthCheckOptions options)
    {
        _options = Guard.ThrowIfNull(options);
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            List<string>? errorList = null;
            foreach (var (host, port) in _options.ConfiguredHosts)
            {
                using var tcpClient = new TcpClient(_options.AddressFamily);
#if NET5_0_OR_GREATER
                await tcpClient.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
#else
                await tcpClient.ConnectAsync(host, port).WithCancellationTokenAsync(cancellationToken).ConfigureAwait(false);
#endif
                if (!tcpClient.Connected)
                {
                    (errorList ??= new()).Add($"Connection to host {host}:{port} failed");
                    if (!_options.CheckAllHosts)
                    {
                        break;
                    }
                }
            }

            return errorList.GetHealthState(context);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
