# Ateliers.Core ロギング使用方法

## 基本的な使い方

### 1. DI コンテナへの登録

```csharp
using Ateliers.DependencyInjection;
using Ateliers.Logging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 実行コンテキストを登録
services.AddAteliersExecutionContext();

// ロギングを登録
services.AddAteliersLogging(logging =>
{
    logging
        .SetCategory("App")                          // カテゴリ設定（例: "App", "VoiceEngine"）
        .SetMinimumLevel(LogLevel.Information)       // 最小ログレベル
        .AddConsole()                                // コンソール出力
        .AddFile(logDirectory: "./logs");           // ファイル出力
});

var serviceProvider = services.BuildServiceProvider();
```

### 2. コンストラクタインジェクション

```csharp
using Ateliers;
using Ateliers.Logging;

public class MyService
{
    private readonly IExecutionContext _context;
    private readonly ILogger _logger;

    public MyService(IExecutionContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public void DoWork()
    {
        // 新しいスコープを開始（相関IDが自動管理）
        using var scope = _context.BeginScope("MyWork");
        
        _logger.Info($"Starting work with correlation ID: {_context.CorrelationId}");
        _logger.Debug("Detailed information");
        _logger.Warn("Warning message");
        
        try
        {
            // 処理
        }
        catch (Exception ex)
        {
            _logger.Error("Error occurred", ex);
        }
    }
}
```

## ログレベル

```csharp
_logger.Trace("トレース情報");              // LogLevel.Trace
_logger.Debug("デバッグ情報");              // LogLevel.Debug
_logger.Info("情報メッセージ");             // LogLevel.Information
_logger.Warn("警告メッセージ");             // LogLevel.Warning
_logger.Error("エラーメッセージ", ex);      // LogLevel.Error
_logger.Critical("重大なエラー", ex);       // LogLevel.Critical
```

## ログ出力フォーマット

```
[2025-01-23T10:00:00.0000000Z] [Information] [App] [CID:abc-123] Starting work
[2025-01-23T10:00:01.0000000Z] [Debug] [App] [CID:abc-123] Detailed information
[2025-01-23T10:00:02.0000000Z] [Warning] [App] [CID:abc-123] Warning message
[2025-01-23T10:00:03.0000000Z] [Error] [App] [CID:abc-123] Error occurred
System.InvalidOperationException: エラー詳細
```

## カテゴリの使い方

カテゴリを使用すると、アプリケーションの異なる部分からのログを区別できます：

```csharp
// アプリケーションログ
services.AddAteliersLogging(logging => logging.SetCategory("App"));

// VoiceEngine ログ
services.AddAteliersLogging(logging => logging.SetCategory("VoiceEngine"));

// データベースログ
services.AddAteliersLogging(logging => logging.SetCategory("Database"));
```

## 実行コンテキストの使い方

### スコープの作成

```csharp
public async Task ProcessRequestAsync()
{
    // スコープを開始（新しい相関IDが自動生成）
    using var scope = _context.BeginScope("RequestProcessing");
    
    _logger.Info("Request started");
    
    // このスコープ内のすべてのログに同じ相関IDが付与される
    await DoSomethingAsync();
    
    _logger.Info("Request completed");
}
```

### 相関IDの取得

```csharp
public void LogCorrelationId()
{
    var correlationId = _context.CorrelationId;
    _logger.Info($"Current correlation ID: {correlationId}");
}
```

## 複数ロガーの組み合わせ

```csharp
services.AddAteliersLogging(logging =>
{
    logging
        .SetCategory("App")
        .SetMinimumLevel(LogLevel.Debug)
        .AddConsole()                              // コンソールに出力
        .AddFile("./logs", "app")                 // ファイルに出力（app-2025-01-23.log）
        .AddInMemory(out var memoryLogger);       // メモリに保持（テスト用）
});
```

## カスタムロガーの追加

```csharp
public class CustomLogger : LoggerBase
{
    public CustomLogger(LoggerOptions options) : base(options.Category) { }
    
    public override void Log(LogEntry entry)
    {
        // カスタム処理
        Console.WriteLine($"Custom: {entry.LogText}");
    }
}

services.AddAteliersLogging(logging =>
{
    logging.AddLogger(new CustomLogger(new LoggerOptions { Category = "Custom" }));
});
```

## ログの保持ポリシー

```csharp
using Ateliers.Logging.Retention;

var policy = new LogRetentionPolicy
{
    TraceRetention = TimeSpan.FromDays(3),        // トレースログを3日間保持
    DebugRetention = TimeSpan.FromDays(7),        // デバッグログを7日間保持
    InformationRetention = TimeSpan.FromDays(14), // 情報ログを14日間保持
    WarningRetention = TimeSpan.FromDays(30),     // 警告ログを30日間保持
    ErrorRetention = TimeSpan.FromDays(90),       // エラーログを90日間保持
    CriticalRetention = TimeSpan.FromDays(90)     // 重大ログを90日間保持
};

var cleaner = new LogRetentionCleaner("./logs", policy);
cleaner.Clean(); // 古いログファイルを削除
```

## テストでの使用例

```csharp
using Ateliers.Logging;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class MyServiceTests
{
    [Fact]
    public void Test_LoggingWorks()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAteliersExecutionContext();
        
        InMemoryLogger memoryLogger = null!;
        services.AddAteliersLogging(logging =>
        {
            logging
                .SetCategory("Test")
                .SetMinimumLevel(LogLevel.Debug)
                .AddInMemory(out memoryLogger);
        });
        
        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger>();
        
        // Act
        logger.Info("Test message");
        
        // Assert
        Assert.Single(memoryLogger.Entries);
        Assert.Equal("Test message", memoryLogger.Entries[0].Message);
        Assert.Equal(LogLevel.Information, memoryLogger.Entries[0].Level);
        Assert.Equal("Test", memoryLogger.Entries[0].Category);
    }
}
```

## ベストプラクティス

1. **常に ExecutionContext を使用する**: 相関IDによるログ追跡が可能になります
2. **適切なログレベルを使用する**: Debug は開発時のみ、Production では Information 以上
3. **カテゴリを設定する**: アプリケーションの異なる部分を区別しやすくなります
4. **例外は必ず Error または Critical でログに記録する**: スタックトレースを保存します
5. **スコープを活用する**: 処理の開始と終了を明確にします

## トラブルシューティング

### ログが出力されない場合

```csharp
// 最小ログレベルを確認
services.AddAteliersLogging(logging =>
{
    logging.SetMinimumLevel(LogLevel.Trace); // すべてのログを出力
});
```

### ログファイルのパスを確認

```csharp
var logDir = Path.Combine(AppContext.BaseDirectory, "logs", "app");
Console.WriteLine($"Log directory: {logDir}");
```

### 相関IDが設定されない場合

```csharp
// ExecutionContext が登録されているか確認
services.AddAteliersExecutionContext();

// スコープを作成しているか確認
using var scope = context.BeginScope("MyOperation");
```
