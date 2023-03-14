using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.System;

public class DiskStorageHealthCheck : IHealthCheck
{
    private readonly DiskStorageOptions _options;

    public DiskStorageHealthCheck(DiskStorageOptions options)
    {
        _options = Guard.ThrowIfNull(options);
    }

    /// <inheritdoc/>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var configuredDrives = _options.ConfiguredDrives;

            List<string>? errorList = null;
            if (configuredDrives.Count > 0)
            {
                var drives = DriveInfo.GetDrives();

                foreach (var (DriveName, MinimumFreeMegabytes) in configuredDrives.Values)
                {
                    var driveInfo = drives.FirstOrDefault(drive => string.Equals(drive.Name, DriveName, StringComparison.InvariantCultureIgnoreCase));

                    if (driveInfo != null)
                    {
                        long actualFreeMegabytes = driveInfo.AvailableFreeSpace / 1024 / 1024;
                        if (actualFreeMegabytes < MinimumFreeMegabytes)
                        {
                            (errorList ??= new()).Add(_options.FailedDescription(DriveName, MinimumFreeMegabytes, actualFreeMegabytes));
                            if (!_options.CheckAllDrives)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        (errorList ??= new()).Add(_options.FailedDescription(DriveName, MinimumFreeMegabytes, null));
                        if (!_options.CheckAllDrives)
                        {
                            break;
                        }
                    }
                }
            }

            return Task.FromResult(errorList.GetHealthState(context));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
        }
    }
}
