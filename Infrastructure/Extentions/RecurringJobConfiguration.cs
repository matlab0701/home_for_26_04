using Hangfire;
using Infrastructure.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class RecurringJobConfiguration
{
    public static void RegisterRecurringJobs(this IServiceProvider services)
    {
        var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<EmailService>(
        "send-payment-email-to-students",
        service => service.ExecuteAsync(CancellationToken.None),
        "0 7 2-5 * *"
    );
    }
}
