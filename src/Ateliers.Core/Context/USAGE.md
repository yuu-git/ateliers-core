# Ateliers.Core 実行コンテキスト使用方法

## 概要

実行コンテキストは、アプリケーションの処理を追跡するための仕組みです。
各処理には一意の相関ID（CorrelationId）が割り当てられ、処理のスコープと共に管理されます。

## 基本的な使い方

### 1. DI コンテナへの登録

```csharp
using Ateliers.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 実行コンテキストを登録
services.AddAteliersExecutionContext();

var serviceProvider = services.BuildServiceProvider();
```

### 2. コンストラクタインジェクション

```csharp
using Ateliers;
using Ateliers.Context;

public class MyService
{
    private readonly IExecutionContext _context;

    public MyService(IExecutionContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync()
    {
        // 処理スコープを開始
        using var scope = _context.BeginScope("MyOperation");
        
        // 相関IDが自動設定される
        Console.WriteLine($"CorrelationId: {_context.CorrelationId}");
        Console.WriteLine($"Properties: {_context.Properties}");
        
        // 処理実行
        await ProcessAsync();
    }
    
    private async Task ProcessAsync()
    {
        // このメソッド内でも同じコンテキストが利用可能
        Console.WriteLine($"Still in context: {_context.CorrelationId}");
        await Task.Delay(100);
    }
}
```

## スコープの管理

### 基本スコープ

```csharp
public async Task ExecuteProcessAsync()
{
    // using ステートメントでスコープを管理
    using var scope = _context.BeginScope("DataProcessing");
    
    // スコープ内の処理
    await ProcessDataAsync();
    
    // スコープ終了時に自動的にクリーンアップ
}
```

### ネストしたスコープ

```csharp
public async Task ParentProcessAsync()
{
    using var parentScope = _context.BeginScope("ParentOperation");
    Console.WriteLine($"Parent CorrelationId: {_context.CorrelationId}");
    Console.WriteLine($"Parent Properties: {_context.Properties}");
    
    // 子処理を呼び出し
    await ChildProcessAsync();
    
    // 親スコープに戻る
    Console.WriteLine($"Back to parent: {_context.CorrelationId}");
}

private async Task ChildProcessAsync()
{
    using var childScope = _context.BeginScope("ChildOperation");
    Console.WriteLine($"Child CorrelationId: {_context.CorrelationId}");
    Console.WriteLine($"Child Properties: {_context.Properties}");
    
    await Task.Delay(100);
}
```

## 相関IDの活用

### ログとの統合

```csharp
using Ateliers;
using Ateliers.Logging;

public class DataService
{
    private readonly IExecutionContext _context;
    private readonly ILogger _logger;

    public DataService(IExecutionContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessDataAsync()
    {
        using var scope = _context.BeginScope("DataProcessing");
        
        // ログに自動的に相関IDが付与される
        _logger.Info("Processing started");  // [CID:abc-123] Processing started
        
        try
        {
            await FetchDataAsync();
            await TransformDataAsync();
            await SaveDataAsync();
            
            _logger.Info("Processing completed");
        }
        catch (Exception ex)
        {
            _logger.Error("Processing failed", ex);
            throw;
        }
    }

    private async Task FetchDataAsync()
    {
        _logger.Debug("Fetching data...");
        await Task.Delay(100);
    }

    private async Task TransformDataAsync()
    {
        _logger.Debug("Transforming data...");
        await Task.Delay(100);
    }

    private async Task SaveDataAsync()
    {
        _logger.Debug("Saving data...");
        await Task.Delay(100);
    }
}
```

### HTTPリクエストヘッダーへの追加

```csharp
public class ApiClient
{
    private readonly IExecutionContext _context;
    private readonly HttpClient _httpClient;

    public ApiClient(IExecutionContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<string> GetDataAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        
        // 相関IDをヘッダーに追加（分散トレーシング）
        if (!string.IsNullOrEmpty(_context.CorrelationId))
        {
            request.Headers.Add("X-Correlation-Id", _context.CorrelationId);
        }
        
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}
```

## 複数処理の実行

### 順次実行

```csharp
public async Task ExecuteMultipleProcessesAsync()
{
    // 処理1
    using (var scope1 = _context.BeginScope("Process1"))
    {
        _logger.Info("Executing process1");
        await Task.Delay(100);
    }
    
    // 処理2
    using (var scope2 = _context.BeginScope("Process2"))
    {
        _logger.Info("Executing process2");
        await Task.Delay(100);
    }
    
    // 各処理は独立した相関IDを持つ
}
```

### 並列実行

```csharp
public async Task ExecuteProcessesInParallelAsync()
{
    var tasks = new[]
    {
        ExecuteProcessAsync("Process1"),
        ExecuteProcessAsync("Process2"),
        ExecuteProcessAsync("Process3")
    };
    
    await Task.WhenAll(tasks);
}

private async Task ExecuteProcessAsync(string processName)
{
    // 各タスクで独立したコンテキストを持つ
    using var scope = _context.BeginScope(processName);
    _logger.Info($"Executing {processName}");
    await Task.Delay(100);
}
```

## 実行コンテキストの取得

### 静的アクセス

```csharp
using Ateliers.Context;

public class MyService
{
    public void DoSomething()
    {
        // 静的プロパティから現在のコンテキストを取得
        var current = ExecutionContext.Current;
        if (current != null)
        {
            Console.WriteLine($"CorrelationId: {current.CorrelationId}");
            Console.WriteLine($"Properties: {current.Properties}");
        }
    }
}
```

### DI 経由でのアクセス（推奨）

```csharp
public class MyService
{
    private readonly IExecutionContext _context;

    // コンストラクタインジェクションを使用（推奨）
    public MyService(IExecutionContext context)
    {
        _context = context;
    }

    public void DoSomething()
    {
        Console.WriteLine($"CorrelationId: {_context.CorrelationId}");
        Console.WriteLine($"Properties: {_context.Properties}");
    }
}
```

## DI とロギングの統合

```csharp
using Ateliers.DependencyInjection;
using Ateliers.Logging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 実行コンテキストとロギングを登録
services.AddAteliersExecutionContext();
services.AddAteliersLogging(logging =>
{
    logging
        .SetCategory("App")
        .SetMinimumLevel(LogLevel.Information)
        .AddConsole()
        .AddFile();
});

// サービスを登録
services.AddScoped<MyService>();

var serviceProvider = services.BuildServiceProvider();
var myService = serviceProvider.GetRequiredService<MyService>();

await myService.ExecuteAsync();
```

## テストでの使用例

### 基本的なテスト

```csharp
using Ateliers;
using Ateliers.Context;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ExecutionContextTests
{
    [Fact]
    public void Test_ContextScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<IExecutionContext>();
        
        // Act
        string? correlationId = null;
        string? properties = null;
        
        using (var scope = context.BeginScope("TestOperation"))
        {
            correlationId = context.CorrelationId;
            properties = context.Properties;
        }
        
        // Assert
        Assert.NotNull(correlationId);
        Assert.Equal("TestOperation", properties);
    }

    [Fact]
    public void Test_NestedScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<IExecutionContext>();
        
        // Act & Assert
        using (var parentScope = context.BeginScope("ParentOperation"))
        {
            var parentCorrelationId = context.CorrelationId;
            var parentProperties = context.Properties;
            
            Assert.Equal("ParentOperation", parentProperties);
            
            using (var childScope = context.BeginScope("ChildOperation"))
            {
                var childCorrelationId = context.CorrelationId;
                var childProperties = context.Properties;
                
                Assert.Equal("ChildOperation", childProperties);
                Assert.NotEqual(parentCorrelationId, childCorrelationId);
            }
            
            // 親スコープに戻る
            Assert.Equal(parentCorrelationId, context.CorrelationId);
            Assert.Equal("ParentOperation", context.Properties);
        }
    }

    [Fact]
    public void Test_StaticAccess()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<IExecutionContext>();
        
        // Act
        using var scope = context.BeginScope("TestOperation");
        
        // 静的プロパティからアクセス
        var current = ExecutionContext.Current;
        
        // Assert
        Assert.NotNull(current);
        Assert.Equal(context.CorrelationId, current.CorrelationId);
        Assert.Equal("TestOperation", current.Properties);
    }
}
```

### 統合テスト

```csharp
using Ateliers;
using Ateliers.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class DataServiceIntegrationTests
{
    [Fact]
    public async Task Test_ProcessingWithContext()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        
        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .SetCategory("Test")
                .AddInMemory(out memoryLogger);
        });
        
        services.AddScoped<DataService>();
        
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<DataService>();
        
        // Act
        await service.ProcessDataAsync();
        
        // Assert
        Assert.All(memoryLogger.Entries, entry =>
        {
            Assert.NotNull(entry.CorrelationId);
            Assert.Equal("Test", entry.Category);
        });
        
        // すべてのログが同じ相関IDを持つことを確認
        var correlationIds = memoryLogger.Entries
            .Select(e => e.CorrelationId)
            .Distinct()
            .ToList();
        Assert.Single(correlationIds);
    }
}
```

## ベストプラクティス

1. **必ず using ステートメントを使用する**: スコープの適切な管理
2. **DI でコンテキストを注入する**: 静的アクセスよりも推奨
3. **スコープは短く保つ**: 処理の単位でスコープを作成
4. **相関IDをログに活用する**: トレーサビリティの向上
5. **ネストしたスコープを活用する**: 複雑な処理の階層管理
6. **HTTPヘッダーに相関IDを含める**: 分散トレーシングの実現

## 高度な使用例

### カスタムプロパティの追加

```csharp
public class ExtendedExecutionContext : ExecutionContext
{
    public string? UserId { get; init; }
    public string? SessionId { get; init; }

    public ExtendedExecutionContext(
        string correlationId,
        string? properties,
        string? userId = null,
        string? sessionId = null)
        : base(correlationId, properties)
    {
        UserId = userId;
        SessionId = sessionId;
    }
}

// DI 登録
services.AddScoped<IExecutionContext>(provider =>
    new ExtendedExecutionContext(
        Guid.NewGuid().ToString(),
        null,
        userId: "user123",
        sessionId: "session456"));
```

### ミドルウェアとの統合（ASP.NET Core）

```csharp
public class ExecutionContextMiddleware
{
    private readonly RequestDelegate _next;

    public ExecutionContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var executionContext = context.RequestServices
            .GetRequiredService<IExecutionContext>();

        // HTTPヘッダーから相関IDを取得（存在する場合）
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        using var scope = executionContext.BeginScope(context.Request.Path);
        
        // レスポンスヘッダーに相関IDを追加
        context.Response.Headers.Append("X-Correlation-Id", correlationId);

        await _next(context);
    }
}

// Startup.cs または Program.cs
app.UseMiddleware<ExecutionContextMiddleware>();
```

### バックグラウンドジョブとの統合

```csharp
public class JobScheduler
{
    private readonly IServiceProvider _serviceProvider;

    public JobScheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteJobAsync()
    {
        // 新しいスコープを作成してコンテキストを取得
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        
        using var jobScope = context.BeginScope("BackgroundJob");
        
        logger.Info("Job started");
        
        try
        {
            // ジョブ実行
            await Task.Delay(1000);
            
            logger.Info("Job completed");
        }
        catch (Exception ex)
        {
            logger.Error("Job failed", ex);
            throw;
        }
    }
}
```

## トラブルシューティング

### コンテキストが null の場合

```csharp
// AddAteliersExecutionContext が登録されているか確認
services.AddAteliersExecutionContext();

// スコープが作成されているか確認
using var scope = context.BeginScope("OperationName");
```

### 相関IDが一致しない場合

```csharp
// 非同期処理で ExecutionContext が引き継がれない場合
// ConfigureAwait(false) を使用していないか確認
await Task.Delay(100); // OK
await Task.Delay(100).ConfigureAwait(false); // NG: コンテキストが失われる
```

### ネストしたスコープがうまく動作しない場合

```csharp
// using ステートメントを正しく使用しているか確認
using (var scope1 = context.BeginScope("Operation1"))
{
    using (var scope2 = context.BeginScope("Operation2"))
    {
        // OK
    }
}

// 以下は NG: scope が適切に閉じられない
var scope1 = context.BeginScope("Operation1");
var scope2 = context.BeginScope("Operation2");
```

### DI でコンテキストが取得できない場合

```csharp
// Scoped ライフタイムで登録されているか確認
services.AddScoped<IExecutionContext>(...); // OK
services.AddSingleton<IExecutionContext>(...); // NG: 状態が共有される

// スコープ内で解決しているか確認
using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IExecutionContext>();
    // OK
}
```

## 参考リンク

- [Ateliers.Core Logging USAGE](../Logging/USAGE.md)
- [Microsoft.Extensions.DependencyInjection ドキュメント](https://docs.microsoft.com/ja-jp/dotnet/core/extensions/dependency-injection)
