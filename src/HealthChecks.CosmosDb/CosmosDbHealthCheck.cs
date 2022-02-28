using System.Collections.Concurrent;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.CosmosDb
{
    public class CosmosDbHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, CosmosClient> _connections = new();

        private readonly string _connectionString;
        private readonly string? _database;
        private readonly IEnumerable<string>? _containers;

        public CosmosDbHealthCheck(string connectionString)
            : this(connectionString, default, default)
        {
        }

        public CosmosDbHealthCheck(string connectionString, string database)
            : this(connectionString, database, default)
        {
            _database = database;
        }

        public CosmosDbHealthCheck(string connectionString, string? database, IEnumerable<string>? containers)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _database = database;
            _containers = containers;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_connections.TryGetValue(_connectionString, out var cosmosDbClient))
                {
                    cosmosDbClient = new CosmosClient(_connectionString);

                    if (!_connections.TryAdd(_connectionString, cosmosDbClient))
                    {
                        cosmosDbClient.Dispose();
                        cosmosDbClient = _connections[_connectionString];
                    }
                }

                await cosmosDbClient.ReadAccountAsync();

                if (_database != null)
                {
                    var database = cosmosDbClient.GetDatabase(_database);
                    await database.ReadAsync();

                    if (_containers != null && _containers.Any())
                    {
                        foreach (var container in _containers)
                        {
                            await database.GetContainer(container)
                                .ReadContainerAsync();
                        }
                    }
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
