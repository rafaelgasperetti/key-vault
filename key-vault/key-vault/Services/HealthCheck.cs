using key_vault.Database;
using key_vault.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace key_vault.Services
{
    public class HealthCheck : IHealthCheck
    {
        private readonly APIEnvironment Environment;

        public HealthCheck(APIEnvironment env)
        {
            Environment = env;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var db = new MySqlDb(Environment);

            if (db.IsHealhty())
            {
                return await Task.FromResult(HealthCheckResult.Healthy(nameof(HealthCheck)));
            }
            else
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(nameof(HealthCheck)));
            }
        }
    }
}