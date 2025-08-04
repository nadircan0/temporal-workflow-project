using System;
using System.Threading.Tasks;
using Temporalio.Activities;

namespace Temporal_trying;

public class InvoiceActivities
{
    [Activity]
    public async Task GenerateInvoiceAsync(string invoiceId)
    {
        Console.WriteLine($"📄 Generating invoice: {invoiceId}");
        await Task.Delay(1000);
    }

    [Activity]
    public async Task SendInvoiceEmailAsync(string invoiceId)
    {
        Console.WriteLine($"📧 Sending invoice email for: {invoiceId}");
        await Task.Delay(1000);
    }
}