using System;
using System.Threading.Tasks;
using TemporalApi.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace TemporalApi.Workflows;

/// <summary>
/// OrderWorkflow handles the complete order processing lifecycle
/// Temporal ensures workflow durability, fault tolerance, and state management
/// This workflow demonstrates sequential activity execution with error handling
/// </summary>
[Workflow]
public class OrderWorkflow
{
    /// <summary>
    /// Main workflow execution method
    /// Temporal automatically handles workflow state persistence and recovery
    /// Activities are executed sequentially with retry policies and timeouts
    /// </summary>
    /// <param name="orderId">Unique order identifier</param>
    /// <returns>Task representing workflow completion</returns>
    [WorkflowRun]
    public async Task RunAsync(string orderId)
    {
        // Temporal: Configure activity execution options
        // RetryPolicy ensures resilience against transient failures
        // StartToCloseTimeout prevents activities from running indefinitely
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
            // Temporal: Execute activities with automatic state management
            // Each activity execution is recorded in Temporal's event history
            // If workflow fails, Temporal can replay from the last successful point
            
            // Step 1: Process customer payment
            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.ChargeCustomerAsync(orderId), options);

            // Step 2: Ship the order
            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.ShipOrderAsync(orderId), options);

            // Step 3: Send confirmation email
            await Workflow.ExecuteActivityAsync(
                (OrderActivities act) => act.SendConfirmationEmailAsync(orderId), options);
        }
        catch (Exception ex)
        {
            // Temporal: Handle workflow failures
            // Temporal maintains the complete execution history for debugging
            // Failed workflows can be retried or compensated
            Console.WriteLine($"Workflow failed for order {orderId}: {ex.Message}");
            throw;
        }
    }
} 