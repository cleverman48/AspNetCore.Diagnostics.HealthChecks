﻿using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.AzureServiceBus
{
    public class AzureServiceBusQueueHealthCheck
        : IHealthCheck
    {
        private const string TEST_MESSAGE = "HealthCheckTest";
        private static readonly ConcurrentDictionary<string, QueueClient> _queueClientConnections = new ConcurrentDictionary<string, QueueClient>();

        private readonly string _connectionString;
        private readonly string _queueName;
        public AzureServiceBusQueueHealthCheck(string connectionString, string queueName)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }
            _connectionString = connectionString;
            _queueName = queueName;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_queueClientConnections.TryGetValue(_connectionString, out var queueClient))
                {
                    queueClient = new QueueClient(_connectionString, _queueName,ReceiveMode.PeekLock, RetryPolicy.NoRetry);

                    if (!_queueClientConnections.TryAdd(_connectionString, queueClient))
                    {
                        return new HealthCheckResult(context.Registration.FailureStatus, description: "New QueueClient connection can't be added into dictionary.");
                    }
                }

                var scheduledMessageId = await queueClient.ScheduleMessageAsync(
                    new Message(Encoding.UTF8.GetBytes(TEST_MESSAGE)),
                    new DateTimeOffset(DateTime.UtcNow).AddHours(2));

                await queueClient.CancelScheduledMessageAsync(scheduledMessageId);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
