﻿using System.Collections.Generic;

namespace HealthChecks.Network
{
    public class PingHealthCheckOptions
    {
        internal Dictionary<string, (string Host, int TimeOut)> ConfiguredHosts { get; } = new Dictionary<string, (string, int)>();

        public PingHealthCheckOptions AddHost(string host, int timeout)
        {
            ConfiguredHosts.Add(host, (host, timeout));
            return this;
        }
    }
}
