﻿using System.Threading.Tasks;

namespace HealthChecks.UI.K8s.Operator.Controller
{
    internal interface IHealthChecksController
    {
        Task<DeploymentResult> DeployAsync(HealthCheckResource resource);
        ValueTask DeleteDeploymentAsync(HealthCheckResource resource);
    }
}
