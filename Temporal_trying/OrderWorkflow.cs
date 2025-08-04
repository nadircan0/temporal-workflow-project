using System;
using System.Threading.Tasks;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Temporal_trying;

[Workflow]
public class OrderWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(string orderId)
    {
        var options = new ActivityOptions
        {
            StartToCloseTimeout = TimeSpan.FromSeconds(30),
            RetryPolicy = new RetryPolicy
            {
                InitialInterval = TimeSpan.FromSeconds(1),
                MaximumInterval = TimeSpan.FromSeconds(10),
                MaximumAttempts = 3,
                NonRetryableErrorTypes = new[] { "System.InvalidOperationException" }
            }
        };

        try
        {
            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.ChargeCustomerAsync(orderId), options);

            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.ShipOrderAsync(orderId), options);

            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.SendConfirmationEmailAsync(orderId), options);
        }
        catch (Exception ex)
        {
            // Log error and potentially trigger compensation logic
            Console.WriteLine($"Workflow failed for order {orderId}: {ex.Message}");
            throw;
        }
    }
}