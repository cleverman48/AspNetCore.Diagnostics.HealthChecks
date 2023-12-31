using Microsoft.Extensions.Logging;

namespace HealthChecks.UI.K8s.Operator.Diagnostics;

internal class OperatorDiagnostics
{
    private readonly ILogger _logger;
    public OperatorDiagnostics(ILoggerFactory loggerFactory)
    {
        _logger = Guard.ThrowIfNull(loggerFactory).CreateLogger(nameof(K8sOperator));
    }

    public void OperatorStarting()
    {
        _logger.LogInformation("The operator is starting");
    }

    public void OperatorShuttingDown()
    {
        _logger.LogInformation("The operator is shutting down");
    }

    public void ServiceWatcherStarting(string @namespace)
    {
        _logger.LogInformation("Service watcher started for namespace {namespace}", @namespace);
    }

    public void ServiceWatcherStopped(string @namespace)
    {
        _logger.LogInformation("Service watcher stopped for namespace {namespace}", @namespace);
    }

    public void OperatorThrow(Exception exception)
    {
        _logger.LogError(exception, "The operator threw an unhandled exception");
    }

    public void ServiceWatcherThrow(Exception exception)
    {
        _logger.LogError(exception.Message, "The operator service watcher threw an unhandled exception");
    }

    public void UiPathConfigured(string path, string value)
    {
        _logger.LogInformation("The UI Path {path} has been configured to {value}", path, value);
    }

    public void DeploymentCreated(string deployment)
    {
        _logger.LogInformation("Deployment {deployment} has been created", deployment);
    }

    public void DeploymentError(string deployment, string error)
    {
        _logger.LogError("Error creating deployment {name} : {error}", deployment, error);
    }

    public void DeploymentOperationError(string deployment, string operation, string error)
    {
        _logger.LogError("Error executing deployment operation: {operation} for hc resource: {name} - err: {error}", operation, deployment, error);
    }
}
