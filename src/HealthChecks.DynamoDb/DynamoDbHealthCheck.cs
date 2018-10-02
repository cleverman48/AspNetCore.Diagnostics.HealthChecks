﻿using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealthChecks.DynamoDb
{
    public class DynamoDbHealthCheck
        : IHealthCheck
    {
        private readonly DynamoDBOptions _options;
        public DynamoDbHealthCheck(DynamoDBOptions options)
        {
            if (string.IsNullOrEmpty(options.AccessKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.AccessKey));
            if (string.IsNullOrEmpty(options.SecretKey)) throw new ArgumentNullException(nameof(DynamoDBOptions.SecretKey));
            if (options.RegionEndpoint == null) throw new ArgumentNullException(nameof(DynamoDBOptions.RegionEndpoint));

            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var credentials = new BasicAWSCredentials(_options.AccessKey, _options.SecretKey);
                var client = new AmazonDynamoDBClient(credentials, _options.RegionEndpoint);

                await client.ListTablesAsync();
                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
