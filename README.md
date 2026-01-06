# Ateliers.Core

[![NuGet Version](https://img.shields.io/nuget/v/Ateliers.Core)](https://www.nuget.org/packages/Ateliers.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Ateliers エコシステムのコア機能を提供する .NET ライブラリです。

## 概要

Ateliers.Core は、ロギング、実行コンテキスト管理、ログリーダーなどの共通機能を提供します。
他の Ateliers プロジェクト（MCP, VoiceEngine 等）の基盤として設計されています。

## 主な機能

### ロギング

- 複数の出力先（コンソール、ファイル、メモリ）
- ログレベル管理（Trace, Debug, Information, Warning, Error, Critical）
- カテゴリによるログの分類
- 相関IDによるログの追跡
- ログ保持ポリシー（自動クリーンアップ）
- 依存性注入（DI）サポート

### 実行コンテキスト

- 相関IDの自動管理
- スコープベースの管理（using ステートメント対応）
- ネストされたスコープのサポート
- 非同期処理対応（async/await）
- 依存性注入（DI）サポート

### ログリーダー

- 相関IDによるフィルタリング
- カテゴリによるフィルタリング
- ログセッションの読み取り

## インストール

### NuGet パッケージ

```bash
dotnet add package Ateliers.Core
```

### パッケージマネージャーコンソール

```powershell
Install-Package Ateliers.Core
```

## クイックスタート

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

var serviceProvider = services.BuildServiceProvider();
```

## ドキュメント

- [ロギング使用方法](src/Ateliers.Core/Logging/USAGE.md)
- [実行コンテキスト使用方法](src/Ateliers.Core/Context/USAGE.md)

## 対応環境

- **.NET**: 10.0 以降
- **C#**: 14.0 以降

## Ateliers エコシステム

- **Ateliers.Core** (このリポジトリ) - コア機能
- **[Ateliers.Ai.Mcp.Core](https://github.com/yuu-git/ateliers-ai-mcp-core)** - MCP 機能の基礎
- **[Ateliers.Ai.Mcp.Tools](https://github.com/yuu-git/ateliers-ai-mcp-tools)** - MCP ツール集
- **[Ateliers.Ai.Mcp.Servers](https://github.com/yuu-git/ateliers-ai-mcp-servers)** - MCP サーバー集

## ライセンス

このプロジェクトは [MIT License](LICENSE) の下でライセンスされています。

## 貢献

貢献を歓迎します！Issue や Pull Request をお気軽にお送りください。

## サポート

- **Issues**: [GitHub Issues](https://github.com/yuu-git/ateliers-core/issues)
- **Discussions**: [GitHub Discussions](https://github.com/yuu-git/ateliers-core/discussions)

## 著者

- **yuu-git** - [GitHub](https://github.com/yuu-git)

## リリースノート

リリースノートは [CHANGELOG.md](CHANGELOG.md) を参照してください。
