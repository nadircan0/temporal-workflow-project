# Temporal Workflow Project

A comprehensive .NET project demonstrating Temporal workflow orchestration with API, workers, and activities.

## 🏗️ Project Structure

```
Temporal_try/
├── TemporalApi/           # Web API with Swagger UI
│   ├── Controllers/       # REST API endpoints
│   ├── Workflows/         # Temporal workflows
│   ├── Activities/        # Business logic activities
│   └── Program.cs         # API configuration
├── WorkerApp/             # Order processing worker
├── InvoiceWorkerApp/      # Invoice processing worker
├── StarterApp/            # Workflow trigger application
└── Temporal_trying/       # Shared library (legacy)
```

## 🚀 Features

- **REST API**: Swagger UI for workflow management
- **Temporal Workflows**: Order and Invoice processing
- **Worker Applications**: Separate workers for different task queues
- **Fault Tolerance**: Automatic retry policies and error handling
- **Scalability**: Independent worker scaling
- **Observability**: Workflow execution monitoring

## 🛠️ Technologies

- **.NET 8.0**
- **Temporal SDK**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI**

## 📋 Prerequisites

- .NET 8.0 SDK
- Temporal Server (localhost:7233)
- Temporal CLI (optional)

## 🚀 Getting Started

### 1. Start Temporal Server

```bash
# Using Temporal CLI
temporal server start-dev

# Or using Docker
docker run -p 7233:7233 temporalio/auto-setup:1.22.3
```

### 2. Run the API

```bash
cd TemporalApi
dotnet run
```

Access Swagger UI: http://localhost:5000

### 3. Start Workers

```bash
# Terminal 1: Order Worker
cd WorkerApp
dotnet run

# Terminal 2: Invoice Worker
cd InvoiceWorkerApp
dotnet run
```

### 4. Start Workflows

```bash
# Using StarterApp
cd StarterApp
dotnet run

# Or using API endpoints
curl -X POST "http://localhost:5000/api/temporal/start-order" \
  -H "Content-Type: application/json" \
  -d '{"orderId": "order-123"}'
```

## 📚 API Endpoints

### Workflow Management

- `POST /api/temporal/start-order` - Start Order workflow
- `POST /api/temporal/start-invoice` - Start Invoice workflow
- `POST /api/temporal/start-both` - Start both workflows
- `GET /api/temporal/workflows` - List all workflows
- `DELETE /api/temporal/workflows/{id}` - Terminate workflow
- `GET /api/temporal/health` - API health check

### Example Requests

```bash
# Start Order workflow
curl -X POST "http://localhost:5000/api/temporal/start-order" \
  -H "Content-Type: application/json" \
  -d '{"orderId": "order-123"}'

# Start Invoice workflow
curl -X POST "http://localhost:5000/api/temporal/start-invoice" \
  -H "Content-Type: application/json" \
  -d '{"orderId": "invoice-456"}'

# Start both workflows
curl -X POST "http://localhost:5000/api/temporal/start-both" \
  -H "Content-Type: application/json" \
  -d '{"orderId": "order-123", "invoiceId": "invoice-456"}'
```

## 🔧 Workflow Types

### OrderWorkflow
- **Activities**: ChargeCustomer, ShipOrder, SendConfirmationEmail
- **Task Queue**: `my-task-queue`
- **Worker**: WorkerApp

### InvoiceWorkflow
- **Activities**: GenerateInvoice, SendInvoiceEmail
- **Task Queue**: `invoice-task-queue`
- **Worker**: InvoiceWorkerApp

## 📊 Monitoring

### Temporal CLI

```bash
# List workflows
temporal workflow list

# Describe specific workflow
temporal workflow describe <workflow-id>

# Terminate workflow
temporal workflow terminate --workflow-id <workflow-id>
```

### Temporal Web UI

Access Temporal Web UI: http://localhost:8233

## 🏗️ Architecture

### Task Queue System

```
API Endpoints → Temporal Server → Task Queues → Workers
     ↓              ↓                ↓           ↓
/start-order → localhost:7233 → my-task-queue → WorkerApp
/start-invoice → localhost:7233 → invoice-task-queue → InvoiceWorkerApp
```

### Workflow Execution

```
OrderWorkflow:
ChargeCustomer → ShipOrder → SendConfirmationEmail

InvoiceWorkflow:
GenerateInvoice → SendInvoiceEmail
```

## 🔄 Temporal Features

- **Workflow Durability**: Automatic state persistence
- **Activity Retry**: Configurable retry policies
- **Fault Tolerance**: Worker crash recovery
- **Scalability**: Independent worker scaling
- **Observability**: Complete execution history

## 📝 Development

### Adding New Workflows

1. Create workflow class in `TemporalApi/Workflows/`
2. Create activities in `TemporalApi/Activities/`
3. Register in worker applications
4. Add API endpoints in `TemporalController`

### Configuration

- **Temporal Server**: localhost:7233
- **API Port**: 5000
- **Swagger UI**: http://localhost:5000
- **Temporal Web UI**: http://localhost:8233

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Temporal team for the excellent workflow orchestration platform
- .NET community for the robust development ecosystem 