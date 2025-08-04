using System;
using System.Threading.Tasks;
using Temporalio.Activities;

namespace TemporalApi.Activities;

/// <summary>
/// InvoiceActivities contains business logic for invoice processing
/// Temporal activities provide fault tolerance and retry capabilities
/// Each activity represents a discrete unit of work in the invoice workflow
/// </summary>
public class InvoiceActivities
{
    /// <summary>
    /// Generates invoice document for the specified invoice
    /// Temporal ensures this activity is executed reliably with retry capability
    /// Activity state is preserved across worker restarts
    /// </summary>
    /// <param name="invoiceId">Unique invoice identifier</param>
    /// <returns>Task representing invoice generation completion</returns>
    [Activity]
    public async Task GenerateInvoiceAsync(string invoiceId)
    {
        Console.WriteLine($" Generating invoice document: {invoiceId}");
        // Temporal: Activity execution with automatic retry capability
        // If invoice generation fails, Temporal will retry based on retry policy
        await Task.Delay(500); // Simulate invoice generation time
        Console.WriteLine($" Invoice document generated successfully: {invoiceId}");
    }

    /// <summary>
    /// Sends invoice email to the customer
    /// Temporal provides idempotency for email sending operations
    /// This activity can be safely retried without sending duplicate emails
    /// </summary>
    /// <param name="invoiceId">Unique invoice identifier</param>
    /// <returns>Task representing email sending completion</returns>
    [Activity]
    public async Task SendInvoiceEmailAsync(string invoiceId)
    {
        Console.WriteLine($" Sending invoice email for: {invoiceId}");
        // Temporal: Activity execution with idempotency guarantees
        // Email sending is idempotent - safe to retry without duplicates
        await Task.Delay(500); // Simulate email sending time
        Console.WriteLine($" Invoice email sent successfully: {invoiceId}");
    }
} 