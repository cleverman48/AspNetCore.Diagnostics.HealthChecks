﻿namespace HealthChecks.UI.Core.Discovery.K8S
{
    class KubernetesDiscoverySettings
    {
        public bool Enabled { get; set; } = false;
        public string ClusterHost { get; set; }
        public string HealthPath { get; set; } = Keys.HEALTHCHECKS_DEFAULT_PATH;
        public string ServicesLabel { get; set; } = Keys.HEALTHCHECKS_DEFAULT_DISCOVERY_LABEL;
        public string Token { get; set; }
        public int RefreshTimeOnSeconds { get; set; } = 300;
    }
}
