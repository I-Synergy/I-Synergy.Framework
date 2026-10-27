# Load Testing & Performance Monitoring

The k6 load-test script and thresholds, custom counters, and EF Core query logging. Load this when defining performance targets or instrumenting critical operations.

## Load Testing

### Using k6 for Load Testing
```javascript
// load-test.js
import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 20 },  // Ramp up
        { duration: '1m', target: 20 },   // Stay at 20 users
        { duration: '30s', target: 0 },   // Ramp down
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests < 500ms
    },
};

export default function () {
    let response = http.get('https://localhost:7001/budgets');

    check(response, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}
```

```bash
# Run load test
k6 run load-test.js
```

## Performance Monitoring

### Custom Metrics
```csharp
public class PerformanceMetrics(ILogger<PerformanceMetrics> logger)
{
    private readonly ConcurrentDictionary<string, long> _counters = new();

    public void IncrementCounter(string name)
    {
        _counters.AddOrUpdate(name, 1, (_, count) => count + 1);
    }

    public void RecordDuration(string operation, long milliseconds)
    {
        logger.LogInformation(
            "Operation {Operation} completed in {Duration}ms",
            operation, milliseconds);
    }

    public Dictionary<string, long> GetCounters() => _counters.ToDictionary(k => k.Key, v => v.Value);
}
```

### Database Query Logging
```csharp
// Program.cs
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});
```
