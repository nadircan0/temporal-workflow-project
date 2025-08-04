using System;
using System.Threading.Tasks;
using Temporalio.Activities;

namespace TemporalApi.Activities;

/// <summary>
/// OrderActivities contains business logic for order processing
/// Temporal activities are the building blocks of workflows
/// Each activity represents a unit of work that can be retried independently
/// </summary>
public class OrderActivities 
{
    /// <summary>
    /// Processes customer payment for an order
    /// Temporal automatically handles activity retries and timeouts
    /// This activity simulates payment processing with configurable delay
    /// </summary>
    /// <param name="orderId">Unique order identifier</param>
    /// <returns>Task representing payment completion</returns>
    [Activity]
    public async Task ChargeCustomerAsync(string orderId)
    {
        Console.WriteLine($" Processing payment for order {orderId}...");
        // Temporal: Activity execution with automatic retry capability
        // If this activity fails, Temporal will retry based on retry policy
        await Task.Delay(500); // Simulate payment processing time
        Console.WriteLine($" Payment processed successfully for order {orderId}");
    }

    /// <summary>
    /// Handles order shipping and logistics
    /// Temporal ensures this activity is executed reliably
    /// Activity state is preserved even if worker restarts
    /// </summary>
    /// <param name="orderId">Unique order identifier</param>
    /// <returns>Task representing shipping completion</returns>
    [Activity]
    public async Task ShipOrderAsync(string orderId)
    {
        Console.WriteLine($" Processing shipping for order {orderId}...");
        // Temporal: Activity execution with durability guarantees
        // This activity can be retried without side effects
        await Task.Delay(500); // Simulate shipping processing time
        Console.WriteLine($" Order shipped successfully: {orderId}");
    }

    /// <summary>
    /// Sends confirmation email to customer
    /// Temporal provides idempotency for email sending
    /// This activity can be safely retried without duplicate emails
    /// </summary>
    /// <param name="orderId">Unique order identifier</param>
    /// <returns>Task representing email sending completion</returns>
    [Activity]
    public async Task SendConfirmationEmailAsync(string orderId)
    {
        Console.WriteLine($" Sending confirmation email for order {orderId}...");
        // Temporal: Activity execution with idempotency
        // Email sending is idempotent - safe to retry
        await Task.Delay(500); // Simulate email sending time
        Console.WriteLine($" Confirmation email sent successfully for order {orderId}");
    }
} 