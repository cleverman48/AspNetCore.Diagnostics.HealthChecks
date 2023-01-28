using HealthChecks.AzureServiceBus.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.AzureServiceBus
{
    public class AzureServiceBusSubscriptionHealthCheck : AzureServiceBusHealthCheck<AzureServiceBusSubscriptionHealthCheckHealthCheckOptions>, IHealthCheck
    {
        private string? _connectionKey;

        protected override string ConnectionKey =>
            _connectionKey ??= $"{Prefix}_{Options.TopicName}_{Options.SubscriptionName}";

        public AzureServiceBusSubscriptionHealthCheck(AzureServiceBusSubscriptionHealthCheckHealthCheckOptions options)
            : base(options)
        {
            Guard.ThrowIfNull(options.TopicName, true);
            Guard.ThrowIfNull(options.SubscriptionName, true);
        }

        /// <inheritdoc />
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (Options.UsePeekMode)
                    await CheckWithReceiver().ConfigureAwait(false);
                else
                    await CheckWithManagement().ConfigureAwait(false);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }

            Task CheckWithReceiver()
            {
                var client = ClientConnections.GetOrAdd(ConnectionKey, _ => CreateClient());
                var receiver = ServiceBusReceivers.GetOrAdd(
                    $"{nameof(AzureServiceBusSubscriptionHealthCheck)}_{ConnectionKey}",
                    client.CreateReceiver(Options.TopicName, Options.SubscriptionName));

                return receiver.PeekMessageAsync(cancellationToken: cancellationToken);
            }

            Task CheckWithManagement()
            {
                var managementClient = ManagementClientConnections.GetOrAdd(ConnectionKey, _ => CreateManagementClient());

                return managementClient.GetSubscriptionRuntimePropertiesAsync(
                    Options.TopicName, Options.SubscriptionName, cancellationToken);
            }
        }
    }
}
