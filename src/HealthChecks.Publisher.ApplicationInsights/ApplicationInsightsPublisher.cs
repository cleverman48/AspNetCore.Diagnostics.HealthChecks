﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.Publisher.ApplicationInsights
{
    class ApplicationInsightsPublisher
        : IHealthCheckPublisher
    {
        const string EVENT_NAME = "AspNetCoreHealthCheck";
        const string METRIC_STATUS_NAME = "AspNetCoreHealthCheckStatus";
        const string METRIC_DURATION_NAME = "AspNetCoreHealthCheckDuration";
        const string HEALTHCHECK_NAME = "AspNetCoreHealthCheckName";

        private readonly string _instrumentationKey;
        private static TelemetryClient _client;
        private static readonly object sync_root = new object();
        private readonly bool _saveDetailedReport;
        private readonly bool _excludeHealthyReports;

        public ApplicationInsightsPublisher(string instrumentationKey = default, bool saveDetailedReport = false, bool excludeHealthyReports = false)
        {
            _instrumentationKey = instrumentationKey;
            _saveDetailedReport = saveDetailedReport;
            _excludeHealthyReports = excludeHealthyReports;
        }
        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            if (report.Status == HealthStatus.Healthy && _excludeHealthyReports)
            {
                return Task.CompletedTask;
            }

            var client = GetOrCreateTelemetryClient();

            if (_saveDetailedReport)
            {
                SaveDetailedReport(report, client);
            }
            else
            {
                SaveGeneralizedReport(report, client);
            }

            return Task.CompletedTask;
        }
        private void SaveDetailedReport(HealthReport report, TelemetryClient client)
        {
            foreach (var reportEntry in report.Entries.Where(entry => !_excludeHealthyReports || entry.Value.Status != HealthStatus.Healthy))
            {
                client.TrackEvent($"{EVENT_NAME}:{reportEntry.Key}",
                    properties: new Dictionary<string, string>()
                    {
                        { nameof(Environment.MachineName), Environment.MachineName },
                        { nameof(Assembly), Assembly.GetEntryAssembly().GetName().Name },
                        { HEALTHCHECK_NAME, reportEntry.Key }
                    },
                    metrics: new Dictionary<string, double>()
                    {
                        { METRIC_STATUS_NAME, reportEntry.Value.Status == HealthStatus.Healthy ? 1 : 0 },
                        { METRIC_DURATION_NAME, reportEntry.Value.Duration.TotalMilliseconds }
                    });
            }
        }
        private static void SaveGeneralizedReport(HealthReport report, TelemetryClient client)
        {
            client.TrackEvent(EVENT_NAME,
                properties: new Dictionary<string, string>
                {
                    { nameof(Environment.MachineName), Environment.MachineName },
                    { nameof(Assembly), Assembly.GetEntryAssembly().GetName().Name }
                },
                metrics: new Dictionary<string, double>
                {
                    { METRIC_STATUS_NAME, report.Status == HealthStatus.Healthy ? 1 : 0 },
                    { METRIC_DURATION_NAME, report.TotalDuration.TotalMilliseconds }
                });
        }
        private TelemetryClient GetOrCreateTelemetryClient()
        {
            if (_client == null)
            {
                lock (sync_root)
                {
                    if (_client == null)
                    {
                        //override instrumentation key or use default instrumentation 
                        //key active on the project.

                        var configuration = string.IsNullOrWhiteSpace(_instrumentationKey)
                            ? TelemetryConfiguration.Active
                            : new TelemetryConfiguration(_instrumentationKey);

                        _client = new TelemetryClient(configuration);
                    }
                }
            }
            return _client;
        }
    }
}
