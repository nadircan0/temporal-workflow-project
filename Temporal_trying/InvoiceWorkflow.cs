using System;
using System.Threading.Tasks;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Temporal_trying;

[Workflow]
public class InvoiceWorkflow
{
    [WorkflowRun]
    public async Task RunAsync(string invoiceId)
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
                (InvoiceActivities act) => act.GenerateInvoiceAsync(invoiceId), options);

            await Workflow.ExecuteActivityAsync(
                (InvoiceActivities act) => act.SendInvoiceEmailAsync(invoiceId), options);
        }
        catch (Exception ex)
        {
            // Log error and potentially trigger compensation logic
            Console.WriteLine($"Invoice workflow failed for invoice {invoiceId}: {ex.Message}");
            throw;
        }
    }
}