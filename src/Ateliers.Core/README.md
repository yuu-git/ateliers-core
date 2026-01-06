# Ateliers.Core

Ateliers エコシステムのコア機能を提供するライブラリです。

## 機能

### ロギング

- **複数の出力先**: コンソール、ファイル、メモリ
- **ログレベル**: Trace, Debug, Information, Warning, Error, Critical
- **カテゴリ管理**: アプリケーションの異なる部分を分類
- **相関ID**: ログの追跡とトレーサビリティ
- **保持ポリシー**: ログファイルの自動クリーンアップ
- **依存性注入サポート**: ASP.NET Core との統合

### 実行コンテキスト

- **相関ID管理**: 処理の追跡
- **スコープ管理**: ネストされた処理の階層管理
- **非同期対応**: async/await をサポート
- **依存性注入サポート**: DI コンテナとの統合

### ログリーダー

- **相関IDでフィルタリング**: 特定の処理のログを抽出
- **カテゴリでフィルタリング**: 特定のカテゴリのログを抽出
- **セッション管理**: ログセッションの読み取り

## インストール

```bash
dotnet add package Ateliers.Core
```

## 使用方法

詳細な使用方法は、以下のドキュメントを参照してください：

- [ロギング使用方法](https://github.com/yuu-git/ateliers-core/blob/main/src/Ateliers.Core/Logging/USAGE.md)
- [実行コンテキスト使用方法](https://github.com/yuu-git/ateliers-core/blob/main/src/Ateliers.Core/Context/USAGE.md)

## ライセンス

MIT License

## リンク

- [GitHub リポジトリ](https://github.com/yuu-git/ateliers-core)
- [ドキュメント](https://github.com/yuu-git/ateliers-core)
- [Issues](https://github.com/yuu-git/ateliers-core/issues)
