
using TemporalApi.Workflows;
using TemporalApi.Activities;
using Temporalio.Client;
using Temporalio.Worker;

/// <summary>
/// Invoice Worker Application
/// This worker processes InvoiceWorkflow activities from the "invoice-task-queue"
/// Temporal workers provide fault tolerance and automatic scaling capabilities
/// </summary>

// Temporal: Connect to Temporal server for workflow execution
// Workers poll the server for new workflow tasks to execute
var client = await TemporalClient.ConnectAsync(new("localhost:7233")
{
   
});

// Temporal: Setup cancellation token for graceful shutdown
// Workers can be stopped gracefully without losing in-progress work
using var tokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    tokenSource.Cancel();
    eventArgs.Cancel = true;
};

// Temporal: Create activity instances for workflow execution
// Activities contain the business logic that workers execute
var activities = new InvoiceActivities();

// Temporal: Create and configure worker
// Workers listen to specific task queues and execute registered activities
Console.WriteLine("Starting Invoice Worker...");
using var worker = new TemporalWorker(
    client,
    new TemporalWorkerOptions(taskQueue: "invoice-task-queue").  // Task queue for Invoice workflows
        AddAllActivities(activities).                             // Register InvoiceActivities
        AddWorkflow<InvoiceWorkflow>());                         // Register InvoiceWorkflow

try
{
    // Temporal: Start worker execution
    // Worker continuously polls for new workflow tasks
    // Temporal handles task distribution and load balancing
    await worker.ExecuteAsync(tokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Invoice Worker stopped gracefully");
}