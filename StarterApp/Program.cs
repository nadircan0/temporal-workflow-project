using TemporalApi.Workflows;
using Temporalio.Client;

/// <summary>
/// Temporal Workflow Starter Application
/// This application demonstrates how to start workflows using Temporal client
/// Temporal provides workflow orchestration and state management
/// </summary>

Console.WriteLine("🚀 Temporal Workflow Starter Application");
Console.WriteLine("==========================================");

// Temporal: Connect to Temporal server for workflow management
// Client provides APIs to start, query, and manage workflows
var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions
{
    TargetHost = "localhost:7233"
});

Console.WriteLine("✅ Connected to Temporal server successfully");

// Get user input for workflow parameters
Console.Write("Enter Order ID (e.g., order-001): ");
var orderId = Console.ReadLine() ?? "order-001";

Console.Write("Enter Invoice ID (e.g., invoice-001): ");
var invoiceId = Console.ReadLine() ?? "invoice-001";

Console.WriteLine("\n📋 Starting workflows...");

// Temporal: Start Order workflow
// Workflow is assigned to "my-task-queue" for Order workers to process
Console.WriteLine($"🛒 Starting Order workflow: {orderId}");
await client.StartWorkflowAsync(
    (OrderWorkflow wf) => wf.RunAsync(orderId),
    new WorkflowOptions
    {
        Id = orderId,
        TaskQueue = "my-task-queue"  // Task queue for Order workers
    });

// Temporal: Start Invoice workflow
// Workflow is assigned to "invoice-task-queue" for Invoice workers to process
Console.WriteLine($"📄 Starting Invoice workflow: {invoiceId}");
await client.StartWorkflowAsync(
    (InvoiceWorkflow wf) => wf.RunAsync(invoiceId),
    new WorkflowOptions
    {
        Id = invoiceId,
        TaskQueue = "invoice-task-queue"  // Task queue for Invoice workers
    });

Console.WriteLine("\n✅ Workflows started successfully!");
Console.WriteLine("📝 Remember to start the workers:");
Console.WriteLine("   Terminal 1: dotnet run --project WorkerApp");
Console.WriteLine("   Terminal 2: dotnet run --project InvoiceWorkerApp");