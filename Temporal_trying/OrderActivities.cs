using System;
using System.Threading.Tasks;
using Temporalio.Activities;

namespace Temporal_trying;

public class OrderActivities 
{
    [Activity]
    public async Task ChargeCustomerAsync(string orderId)
    {
        Console.WriteLine($"Charging customer for order {orderId}...");
        await Task.Delay(1000);
    }


    [Activity]
    public async Task ShipOrderAsync(string orderId)
    {
        Console.WriteLine($"Shipping order {orderId}...");
        await Task.Delay(1000);
    }


    [Activity]
    public async Task SendConfirmationEmailAsync(string orderId)
    {
        Console.WriteLine($"Sending confirmation email for order {orderId}...");
        await Task.Delay(1000); // 2 saniye olarak düzeltildi
    }
}

