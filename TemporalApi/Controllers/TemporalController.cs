using Microsoft.AspNetCore.Mvc;
using TemporalApi.Workflows;
using TemporalApi.Activities;
using Temporalio.Client;

namespace TemporalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemporalController : ControllerBase
{
    private readonly ILogger<TemporalController> _logger;
    private readonly TemporalClient _client;

    public TemporalController(ILogger<TemporalController> logger)
    {
        _logger = logger;
        // Initialize Temporal client connection to local Temporal server
        _client = TemporalClient.ConnectAsync(new TemporalClientConnectOptions
        {
            TargetHost = "localhost:7233"
        }).Result;
    }

    /// <summary>
    /// Starts a new Order workflow
    /// Temporal creates a workflow execution and assigns it to the specified task queue
    /// Workers listening to this task queue will pick up and execute the workflow activities
    /// </summary>
    /// <param name="request">Order workflow start request</param>
    /// <returns>Workflow start result</returns>
    [HttpPost("start-order")]
    public async Task<IActionResult> StartOrderWorkflow([FromBody] StartWorkflowRequest request)
    {
        try
        {
            _logger.LogInformation("Starting Order workflow: {OrderId}", request.OrderId);

            // Temporal: Start workflow execution and assign to task queue
            // Workers listening to "my-task-queue" will execute OrderWorkflow activities
            await _client.StartWorkflowAsync(
                (OrderWorkflow wf) => wf.RunAsync(request.OrderId),
                new WorkflowOptions
                {
                    Id = request.OrderId,
                    TaskQueue = "my-task-queue" // Task queue for Order workers
                });

            return Ok(new WorkflowResponse
            {
                Success = true,
                Message = $"Order workflow started successfully: {request.OrderId}",
                WorkflowId = request.OrderId,
                TaskQueue = "my-task-queue",
                WorkflowType = "OrderWorkflow"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Order workflow: {OrderId}", request.OrderId);
            return BadRequest(new WorkflowResponse
            {
                Success = false,
                Message = $"Failed to start Order workflow: {ex.Message}",
                WorkflowId = request.OrderId,
                WorkflowType = "OrderWorkflow"
            });
        }
    }

    /// <summary>
    /// Starts a new Invoice workflow
    /// Temporal creates a separate workflow execution for invoice processing
    /// Workers listening to "invoice-task-queue" will handle invoice activities
    /// </summary>
    /// <param name="request">Invoice workflow start request</param>
    /// <returns>Workflow start result</returns>
    [HttpPost("start-invoice")]
    public async Task<IActionResult> StartInvoiceWorkflow([FromBody] StartWorkflowRequest request)
    {
        try
        {
            _logger.LogInformation("Starting Invoice workflow: {InvoiceId}", request.OrderId);

            // Temporal: Start invoice workflow with separate task queue
            // Different workers handle invoice processing for better separation of concerns
            await _client.StartWorkflowAsync(
                (InvoiceWorkflow wf) => wf.RunAsync(request.OrderId),
                new WorkflowOptions
                {
                    Id = request.OrderId,
                    TaskQueue = "invoice-task-queue" // Separate task queue for Invoice workers
                });

            return Ok(new WorkflowResponse
            {
                Success = true,
                Message = $"Invoice workflow started successfully: {request.OrderId}",
                WorkflowId = request.OrderId,
                TaskQueue = "invoice-task-queue",
                WorkflowType = "InvoiceWorkflow"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Invoice workflow: {InvoiceId}", request.OrderId);
            return BadRequest(new WorkflowResponse
            {
                Success = false,
                Message = $"Failed to start Invoice workflow: {ex.Message}",
                WorkflowId = request.OrderId,
                WorkflowType = "InvoiceWorkflow"
            });
        }
    }

    /// <summary>
    /// Starts both Order and Invoice workflows simultaneously
    /// Demonstrates Temporal's ability to handle multiple workflow types
    /// Each workflow runs independently with its own task queue and workers
    /// </summary>
    /// <param name="request">Both workflows start request</param>
    /// <returns>Workflow start result</returns>
    [HttpPost("start-both")]
    public async Task<IActionResult> StartBothWorkflows([FromBody] StartBothWorkflowsRequest request)
    {
        try
        {
            _logger.LogInformation("Starting both Order and Invoice workflows: {OrderId}, {InvoiceId}", 
                request.OrderId, request.InvoiceId);

            // Temporal: Start Order workflow - processed by Order workers
            await _client.StartWorkflowAsync(
                (OrderWorkflow wf) => wf.RunAsync(request.OrderId),
                new WorkflowOptions
                {
                    Id = request.OrderId,
                    TaskQueue = "my-task-queue"
                });

            // Temporal: Start Invoice workflow - processed by Invoice workers
            // Both workflows run independently and can be scaled separately
            await _client.StartWorkflowAsync(
                (InvoiceWorkflow wf) => wf.RunAsync(request.InvoiceId),
                new WorkflowOptions
                {
                    Id = request.InvoiceId,
                    TaskQueue = "invoice-task-queue"
                });

            return Ok(new WorkflowResponse
            {
                Success = true,
                Message = $"Both Order and Invoice workflows started successfully: {request.OrderId}, {request.InvoiceId}",
                WorkflowId = $"{request.OrderId},{request.InvoiceId}",
                TaskQueue = "my-task-queue,invoice-task-queue",
                WorkflowType = "OrderWorkflow,InvoiceWorkflow"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start workflows: {OrderId}, {InvoiceId}", 
                request.OrderId, request.InvoiceId);
            return BadRequest(new WorkflowResponse
            {
                Success = false,
                Message = $"Failed to start workflows: {ex.Message}",
                WorkflowId = $"{request.OrderId},{request.InvoiceId}",
                WorkflowType = "OrderWorkflow,InvoiceWorkflow"
            });
        }
    }

    /// <summary>
    /// Lists all existing workflows
    /// Temporal maintains workflow history and current state
    /// This endpoint queries Temporal server for workflow executions
    /// </summary>
    /// <returns>List of workflows</returns>
    [HttpGet("workflows")]
    public async Task<IActionResult> ListWorkflows()
    {
        try
        {
            // Temporal: Query workflow executions from Temporal server
            // This shows all workflows that have been started and their current status
            var workflows = _client.ListWorkflowsAsync("WorkflowType='OrderWorkflow' OR WorkflowType='InvoiceWorkflow'");
            var workflowList = new List<WorkflowInfo>();
            
            await foreach (var workflow in workflows)
            {
                workflowList.Add(new WorkflowInfo
                {
                    Id = workflow.Id,
                    Status = workflow.Status.ToString(),
                    Type = workflow.WorkflowType,
                    StartTime = workflow.StartTime
                });
            }

            return Ok(new WorkflowListResponse
            {
                Success = true,
                Workflows = workflowList,
                Count = workflowList.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve workflow list");
            return BadRequest(new WorkflowListResponse
            {
                Success = false,
                Message = $"Failed to retrieve workflow list: {ex.Message}",
                Workflows = new List<WorkflowInfo>()
            });
        }
    }

    /// <summary>
    /// Terminates a workflow execution
    /// Temporal provides workflow termination capabilities for cleanup
    /// This endpoint provides CLI command for manual termination
    /// </summary>
    /// <param name="workflowId">Workflow ID to terminate</param>
    /// <returns>Termination result</returns>
    [HttpDelete("workflows/{workflowId}")]
    public IActionResult TerminateWorkflow(string workflowId)
    {
        try
        {
            _logger.LogInformation("Terminating workflow: {WorkflowId}", workflowId);

            // Temporal: Provide CLI command for workflow termination
            // Temporal CLI can be used for workflow management and debugging
            return Ok(new WorkflowResponse
            {
                Success = true,
                Message = $"Workflow termination command: temporal workflow terminate --workflow-id {workflowId}",
                WorkflowId = workflowId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to terminate workflow: {WorkflowId}", workflowId);
            return BadRequest(new WorkflowResponse
            {
                Success = false,
                Message = $"Failed to terminate workflow: {ex.Message}",
                WorkflowId = workflowId
            });
        }
    }

    /// <summary>
    /// Health check endpoint for API status
    /// Shows available workflows and activities in the system
    /// </summary>
    /// <returns>API health status</returns>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            TemporalServer = "localhost:7233",
            AvailableWorkflows = new[] { "OrderWorkflow", "InvoiceWorkflow" },
            AvailableActivities = new[] { "OrderActivities", "InvoiceActivities" }
        });
    }
}

// Request/Response models
public class StartWorkflowRequest
{
    public string OrderId { get; set; } = string.Empty;
}

public class StartBothWorkflowsRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
}

public class WorkflowResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public string TaskQueue { get; set; } = string.Empty;
    public string WorkflowType { get; set; } = string.Empty;
}

public class WorkflowInfo
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}

public class WorkflowListResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<WorkflowInfo> Workflows { get; set; } = new();
    public int Count { get; set; }
} 