# I-Synergy Framework AspNetCore Monitoring

Real-time monitoring and communication infrastructure using SignalR for ASP.NET Core applications. This package provides SignalR hub integration, monitor services for publishing events, connection management, and group-based messaging for building real-time dashboards, notifications, and collaboration features.

[![NuGet](https://img.shields.io/nuget/v/I-Synergy.Framework.AspNetCore.Monitoring.svg)](https://www.nuget.org/packages/I-Synergy.Framework.AspNetCore.Monitoring/)
[![License](https://img.shields.io/github/license/I-Synergy/I-Synergy.Framework)](https://github.com/I-Synergy/I-Synergy.Framework/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

## Features

- **SignalR hub integration** with OpenIddict authentication
- **Monitor service** for publishing real-time events to groups
- **Connection lifecycle management** with automatic group assignment
- **User and account-based groups** for tenant isolation
- **Generic event publishing** with typed entity support
- **Real-time notifications** for connected/disconnected users
- **Detailed error logging** for debugging SignalR connections
- **Scalable architecture** supporting multiple concurrent connections

## Installation

Install the package via NuGet:

```bash
dotnet add package I-Synergy.Framework.AspNetCore.Monitoring
```

## Quick Start

### 1. Configure Monitoring Services

In your `Program.cs`:

```csharp
using ISynergy.Framework.AspNetCore.Monitoring.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add monitoring with SignalR (specify your entity type)
builder.Services.AddMonitorSignalR<MonitorEvent>();

builder.Services.AddControllers();

var app = builder.Build();

// Map the monitor hub
app.MapHub<MonitorHub>("/hubs/monitor");

app.Run();
```

### 2. Using the Monitor Service

Publish events to connected clients:

```csharp
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMonitorService<OrderEvent> _monitorService;

    public OrdersController(IMonitorService<OrderEvent> monitorService)
    {
        _monitorService = monitorService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = await CreateOrderAsync(request);

        // Publish event to all users in the account group
        await _monitorService.PublishAsync(
            channel: request.AccountId.ToString(),
            eventname: "OrderCreated",
            data: new OrderEvent
            {
                OrderId = order.Id,
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            });

        return Ok(order);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderStatus(
        int id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await UpdateOrderAsync(id, request);

        // Notify users about status change
        await _monitorService.PublishAsync(
            channel: order.AccountId.ToString(),
            eventname: "OrderStatusChanged",
            data: new OrderEvent
            {
                OrderId = order.Id,
                Status = order.Status,
                UpdatedAt = DateTime.UtcNow
            });

        return Ok(order);
    }
}

public class OrderEvent
{
    public int OrderId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

### 3. Client-Side Connection (JavaScript/TypeScript)

Connect to the SignalR hub from your frontend:

```typescript
import * as signalR from "@microsoft/signalr";

// Configure connection with authentication
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/monitor", {
        accessTokenFactory: () => getAuthToken()
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Listen for order events
connection.on("OrderCreated", (event) => {
    console.log("New order created:", event);
    updateOrdersList(event);
});

connection.on("OrderStatusChanged", (event) => {
    console.log("Order status changed:", event);
    updateOrderStatus(event);
});

// Listen for user connection events
connection.on("Connected", (message) => {
    console.log(`User connected: ${message.Data}`);
    updateOnlineUsers(message);
});

connection.on("Disconnected", (message) => {
    console.log(`User disconnected: ${message.Data}`);
    updateOnlineUsers(message);
});

// Start connection
async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(startConnection, 5000);
    }
}

// Handle disconnections
connection.onclose(async () => {
    await startConnection();
});

// Get authentication token
function getAuthToken() {
    return localStorage.getItem("authToken");
}

// Start the connection
startConnection();
```

### 4. Client-Side Connection (.NET Client)

Connect from a .NET application:

```csharp
using Microsoft.AspNetCore.SignalR.Client;

public class MonitorClient : IAsyncDisposable
{
    private readonly HubConnection _connection;

    public MonitorClient(string hubUrl, string accessToken)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken);
            })
            .WithAutomaticReconnect()
            .Build();

        // Register event handlers
        _connection.On<OrderEvent>("OrderCreated", OnOrderCreated);
        _connection.On<OrderEvent>("OrderStatusChanged", OnOrderStatusChanged);
        _connection.On<HubMessage<string>>("Connected", OnUserConnected);
        _connection.On<HubMessage<string>>("Disconnected", OnUserDisconnected);
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
    }

    private void OnOrderCreated(OrderEvent orderEvent)
    {
        Console.WriteLine($"Order created: {orderEvent.OrderId}");
    }

    private void OnOrderStatusChanged(OrderEvent orderEvent)
    {
        Console.WriteLine($"Order {orderEvent.OrderId} status: {orderEvent.Status}");
    }

    private void OnUserConnected(HubMessage<string> message)
    {
        Console.WriteLine($"User connected: {message.Data}");
    }

    private void OnUserDisconnected(HubMessage<string> message)
    {
        Console.WriteLine($"User disconnected: {message.Data}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }
}

// Usage
var client = new MonitorClient("https://api.example.com/hubs/monitor", authToken);
await client.StartAsync();
```

## Core Components

### Hubs

```
ISynergy.Framework.AspNetCore.Monitoring.Hubs/
└── MonitorHub                    # SignalR hub with authentication
```

### Services

```
ISynergy.Framework.AspNetCore.Monitoring.Services/
└── MonitorService<TEntity>       # Publish events to hub groups
```

### Extensions

```
ISynergy.Framework.AspNetCore.Monitoring.Extensions/
├── ServiceCollectionExtensions   # DI configuration
├── HostApplicationBuilderExtensions  # Builder configuration
└── ApplicationBuilderExtensions  # Middleware configuration
```

## Advanced Features

### Publishing to Specific Users

The MonitorHub automatically creates groups based on user ID and account ID:

```csharp
using ISynergy.Framework.Monitoring.Abstractions.Services;

public class NotificationService
{
    private readonly IMonitorService<NotificationEvent> _monitorService;

    public NotificationService(IMonitorService<NotificationEvent> monitorService)
    {
        _monitorService = monitorService;
    }

    public async Task SendToUserAsync(string userId, NotificationEvent notification)
    {
        // Send to specific user's group
        await _monitorService.PublishAsync(
            channel: userId,
            eventname: "Notification",
            data: notification);
    }

    public async Task SendToAccountAsync(Guid accountId, NotificationEvent notification)
    {
        // Send to all users in an account
        await _monitorService.PublishAsync(
            channel: accountId.ToString(),
            eventname: "Notification",
            data: notification);
    }

    public async Task BroadcastAsync(NotificationEvent notification)
    {
        // Send to all connected users
        await _monitorService.PublishAsync(
            channel: "all",
            eventname: "Notification",
            data: notification);
    }
}
```

### Custom Event Types

Define strongly-typed events for different scenarios:

```csharp
// Base event
public abstract class MonitorEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Specific event types
public class OrderEvent : MonitorEvent
{
    public int OrderId { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}

public class InventoryEvent : MonitorEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Action { get; set; } // Added, Removed, Updated
}

public class UserActivityEvent : MonitorEvent
{
    public string UserId { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
}

// Register multiple monitor services
builder.Services.AddMonitorSignalR<OrderEvent>();
builder.Services.AddSingleton<IMonitorService<InventoryEvent>, MonitorService<InventoryEvent>>();
builder.Services.AddSingleton<IMonitorService<UserActivityEvent>, MonitorService<UserActivityEvent>>();
```

### Connection State Management

Track connection states in your hub:

```csharp
using ISynergy.Framework.AspNetCore.Monitoring.Hubs;
using Microsoft.AspNetCore.SignalR;

public class CustomMonitorHub : MonitorHub
{
    private readonly IConnectionTracker _connectionTracker;

    public CustomMonitorHub(
        ILogger<CustomMonitorHub> logger,
        IConnectionTracker connectionTracker)
        : base(logger)
    {
        _connectionTracker = connectionTracker;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var accountId = Context.User.GetAccountId();
        var userId = Context.User.GetUserId();

        // Track connection
        await _connectionTracker.AddConnectionAsync(
            Context.ConnectionId,
            userId,
            accountId);

        // Send current online users to the connected client
        var onlineUsers = await _connectionTracker.GetOnlineUsersAsync(accountId);
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var accountId = Context.User.GetAccountId();
        var userId = Context.User.GetUserId();

        // Remove connection
        await _connectionTracker.RemoveConnectionAsync(
            Context.ConnectionId,
            userId,
            accountId);

        await base.OnDisconnectedAsync(exception);
    }
}
```

### Real-Time Dashboard

Build a real-time monitoring dashboard:

```csharp
// Backend service
public class DashboardService
{
    private readonly IMonitorService<DashboardMetric> _monitorService;
    private readonly IMetricsCollector _metricsCollector;
    private Timer _timer;

    public DashboardService(
        IMonitorService<DashboardMetric> monitorService,
        IMetricsCollector metricsCollector)
    {
        _monitorService = monitorService;
        _metricsCollector = metricsCollector;
    }

    public void StartMonitoring(Guid accountId)
    {
        _timer = new Timer(async _ =>
        {
            var metrics = await _metricsCollector.GetCurrentMetricsAsync();

            await _monitorService.PublishAsync(
                channel: accountId.ToString(),
                eventname: "DashboardUpdate",
                data: new DashboardMetric
                {
                    ActiveUsers = metrics.ActiveUsers,
                    OrdersPerHour = metrics.OrdersPerHour,
                    Revenue = metrics.Revenue,
                    ServerLoad = metrics.ServerLoad
                });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }
}

public class DashboardMetric
{
    public int ActiveUsers { get; set; }
    public int OrdersPerHour { get; set; }
    public decimal Revenue { get; set; }
    public double ServerLoad { get; set; }
}
```

Frontend (React example):

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface DashboardMetric {
    activeUsers: number;
    ordersPerHour: number;
    revenue: number;
    serverLoad: number;
}

export function Dashboard() {
    const [metrics, setMetrics] = useState<DashboardMetric | null>(null);
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/monitor', {
                accessTokenFactory: () => getAuthToken()
            })
            .withAutomaticReconnect()
            .build();

        newConnection.on('DashboardUpdate', (metric: DashboardMetric) => {
            setMetrics(metric);
        });

        newConnection.start()
            .then(() => console.log('Connected to monitoring hub'))
            .catch(err => console.error('Connection error:', err));

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, []);

    return (
        <div className="dashboard">
            <MetricCard
                title="Active Users"
                value={metrics?.activeUsers ?? 0}
            />
            <MetricCard
                title="Orders/Hour"
                value={metrics?.ordersPerHour ?? 0}
            />
            <MetricCard
                title="Revenue"
                value={`$${metrics?.revenue?.toFixed(2) ?? '0.00'}`}
            />
            <MetricCard
                title="Server Load"
                value={`${metrics?.serverLoad?.toFixed(1) ?? '0.0'}%`}
            />
        </div>
    );
}
```

## Usage Examples

### Real-Time Collaboration

Implement collaborative editing features:

```csharp
public class DocumentService
{
    private readonly IMonitorService<DocumentChangeEvent> _monitorService;

    public DocumentService(IMonitorService<DocumentChangeEvent> monitorService)
    {
        _monitorService = monitorService;
    }

    public async Task UpdateDocumentAsync(
        Guid documentId,
        string userId,
        DocumentChange change)
    {
        // Save the change
        await SaveChangeAsync(documentId, change);

        // Notify all users viewing this document
        await _monitorService.PublishAsync(
            channel: documentId.ToString(),
            eventname: "DocumentChanged",
            data: new DocumentChangeEvent
            {
                DocumentId = documentId,
                UserId = userId,
                Change = change,
                Timestamp = DateTime.UtcNow
            });
    }
}

// Client-side (TypeScript)
connection.on("DocumentChanged", (event) => {
    if (event.userId !== currentUserId) {
        // Apply changes from other users
        applyDocumentChange(event.change);
    }
});
```

### Real-Time Notifications

Send instant notifications to users:

```csharp
public class NotificationHub
{
    private readonly IMonitorService<Notification> _monitorService;

    public NotificationHub(IMonitorService<Notification> monitorService)
    {
        _monitorService = monitorService;
    }

    public async Task SendNotificationAsync(
        string userId,
        string title,
        string message,
        NotificationType type)
    {
        await _monitorService.PublishAsync(
            channel: userId,
            eventname: "Notification",
            data: new Notification
            {
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            });
    }
}

// Client-side
connection.on("Notification", (notification) => {
    showToast(notification.title, notification.message, notification.type);
});
```

## Best Practices

> [!TIP]
> Use **groups** to efficiently broadcast messages to multiple connected clients without iterating through all connections.

> [!IMPORTANT]
> Always implement **automatic reconnection** on the client side to handle network interruptions gracefully.

> [!NOTE]
> Monitor SignalR **connection count** and **message throughput** to ensure your infrastructure can handle the load.

### Hub Design

- Keep hub methods focused and simple
- Use groups for efficient message routing
- Implement proper authentication and authorization
- Log connection events for monitoring
- Handle exceptions gracefully
- Limit message size to avoid performance issues

### Scalability

- Use Azure SignalR Service for production scaling
- Implement backplane (Redis) for multi-server deployments
- Monitor hub performance and connection metrics
- Set connection limits based on server capacity
- Use distributed caching for connection state
- Consider message batching for high-frequency updates

### Security

- Always require authentication for sensitive hubs
- Validate user permissions before sending messages
- Use groups to enforce tenant isolation
- Encrypt sensitive data in messages
- Implement rate limiting for hub methods
- Audit hub access and message patterns

### Performance

- Minimize message payload size
- Batch frequent updates when possible
- Use compression for large messages
- Implement client-side throttling
- Cache frequently accessed data
- Monitor and optimize hub method execution time

## Testing

Example unit tests for monitoring services:

```csharp
using ISynergy.Framework.Monitoring.Abstractions.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

public class MonitorServiceTests
{
    [Fact]
    public async Task PublishAsync_PublishesToCorrectGroup()
    {
        // Arrange
        var mockHubContext = new Mock<IHubContext<MonitorHub>>();
        var mockClients = new Mock<IHubClients>();
        var mockGroupClients = new Mock<IClientProxy>();

        mockHubContext.Setup(h => h.Clients).Returns(mockClients.Object);
        mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(mockGroupClients.Object);

        var service = new MonitorService<OrderEvent>(mockHubContext.Object);

        var orderEvent = new OrderEvent
        {
            OrderId = 123,
            Status = "Created"
        };

        // Act
        await service.PublishAsync("account-123", "OrderCreated", orderEvent);

        // Assert
        mockClients.Verify(c => c.Group("account-123"), Times.Once);
        mockGroupClients.Verify(
            c => c.SendCoreAsync(
                "OrderCreated",
                It.Is<object[]>(o => o[0] == orderEvent),
                default),
            Times.Once);
    }
}
```

## Dependencies

- **Microsoft.AspNetCore.SignalR** - SignalR infrastructure
- **Microsoft.AspNetCore.SignalR.Core** - Core SignalR abstractions
- **ISynergy.Framework.Core** - Core framework utilities
- **ISynergy.Framework.Monitoring.Abstractions** - Monitoring abstractions
- **ISynergy.Framework.MessageBus** - Message bus models

## Documentation

For more information about the I-Synergy Framework:

- [Framework Documentation](https://github.com/I-Synergy/I-Synergy.Framework)
- [API Reference](https://github.com/I-Synergy/I-Synergy.Framework/wiki)
- [Sample Applications](https://github.com/I-Synergy/I-Synergy.Framework/tree/main/samples)

## Related Packages

- **I-Synergy.Framework.Core** - Core framework components
- **I-Synergy.Framework.AspNetCore** - Base ASP.NET Core integration
- **I-Synergy.Framework.AspNetCore.Authentication** - Authentication utilities
- **I-Synergy.Framework.MessageBus** - Message bus infrastructure
- **I-Synergy.Framework.Monitoring.Abstractions** - Monitoring contracts

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/I-Synergy/I-Synergy.Framework).
