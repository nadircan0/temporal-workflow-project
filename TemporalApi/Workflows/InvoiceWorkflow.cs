using System;
using System.Threading.Tasks;
using TemporalApi.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace TemporalApi.Workflows;

/// <summary>
/// InvoiceWorkflow handles invoice generation and delivery
/// Temporal provides workflow isolation and independent scaling
/// This workflow demonstrates parallel processing capabilities
/// </summary>
[Workflow]
public class InvoiceWorkflow
{
    /// <summary>
    /// Main workflow execution method for invoice processing
    /// Temporal ensures workflow durability and fault tolerance
    /// Activities are executed with retry policies and timeouts
    /// </summary>
    /// <param name="invoiceId">Unique invoice identifier</param>
    /// <returns>Task representing workflow completion</returns>
    [WorkflowRun]
    public async Task RunAsync(string invoiceId)
    {
        // Temporal: Configure activity execution options
        // RetryPolicy provides resilience against transient failures
        // StartToCloseTimeout ensures activities don't run indefinitely
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
            // Temporal: Execute invoice processing activities
            // Each activity execution is recorded in Temporal's event history
            // Workflow state is automatically persisted for recovery
            
            // Step 1: Generate invoice document
            await Workflow.ExecuteActivityAsync(
                (InvoiceActivities act) => act.GenerateInvoiceAsync(invoiceId), options);

            // Step 2: Send invoice via email
            await Workflow.ExecuteActivityAsync(
                (InvoiceActivities act) => act.SendInvoiceEmailAsync(invoiceId), options);
        }
        catch (Exception ex)
        {
            // Temporal: Handle workflow failures
            // Temporal maintains complete execution history for debugging
            // Failed workflows can be retried or compensated
            Console.WriteLine($"Invoice workflow failed for invoice {invoiceId}: {ex.Message}");
            throw;
        }
    }
} 