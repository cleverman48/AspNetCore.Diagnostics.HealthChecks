using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace HealthChecks.UI.K8s.Operator.Handlers;

public class NotificationHandler
{
    private readonly IKubernetes _client;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<K8sOperator> _logger;

    public NotificationHandler(IKubernetes client, IHttpClientFactory httpClientFactory, ILogger<K8sOperator> logger)
    {
        _client = Guard.ThrowIfNull(client);
        _httpClientFactory = Guard.ThrowIfNull(httpClientFactory);
        _logger = Guard.ThrowIfNull(logger);
    }
    public async Task NotifyDiscoveredServiceAsync(WatchEventType type, V1Service service, HealthCheckResource resource)
    {
        var uiService = await _client.ListNamespacedOwnedServiceAsync(resource.Metadata.NamespaceProperty, resource.Metadata.Uid);
        var secret = await _client.ListNamespacedOwnedSecretAsync(resource.Metadata.NamespaceProperty, resource.Metadata.Uid);

        if (!service.Metadata.Labels.ContainsKey(resource.Spec.ServicesLabel))
        {
            type = WatchEventType.Deleted;
        }

        await HealthChecksPushService.PushNotification(
            type,
            resource,
            uiService!, // TODO: check
            service,
            secret!, // TODO: check
            _logger,
            _httpClientFactory);
    }
}
