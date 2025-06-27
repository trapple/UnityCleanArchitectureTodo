# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## プロジェクト概要

このプロジェクトは「UnityCleanArchitectureTodo」という名前で、UnityでClean ArchitectureパターンとMVVMアーキテクチャを完全実装したTodoアプリケーションです。TDD（テスト駆動開発）手法を用いて開発され、現在95%以上の高い完成度を誇ります。

## プロジェクト構成

- **プロジェクト名**: UnityCleanArchitectureTodo
- **Unity バージョン**: 2022.3.61f1 LTS
- **Unity プロジェクトパス**: `UnityCleanArchitechtureTodo/`
- **プロジェクト完成度**: 95%以上完成
- **ビルド**: Unityエディタからビルド

## 主要な依存関係とライブラリ

### Unity Package Manager (.upm)
- **R3** (v1.3.0): リアクティブプログラミング用ライブラリ（MVVM ViewModelで全面活用）
- **UniTask** (v2.5.10): 非同期処理とタスク管理（全層で使用）
- **VContainer** (v1.16.9): 依存性注入コンテナ（Clean Architectureの実装に使用）
- **NuGet For Unity** (v4.4.0): .NET NuGetパッケージをUnityで使用するためのツール
- **TextMeshPro**: UI文字表示（日本語対応）

### NuGet パッケージ
- **Microsoft.Bcl.AsyncInterfaces** (v6.0.0): 非同期インターフェース
- **Microsoft.Bcl.TimeProvider** (v8.0.0): 時間プロバイダー（テスト可能な時刻実装）
- **R3** (v1.3.0): リアクティブ拡張ライブラリ
- **System.ComponentModel.Annotations** (v5.0.0): データ注釈
- **System.Runtime.CompilerServices.Unsafe** (v6.0.0): アンセーフコード関連
- **System.Threading.Channels** (v8.0.0): チャンネルベースの非同期処理

## アーキテクチャパターン

このプロジェクトはClean Architectureパターンを採用し、Presentation層にMVVMアーキテクチャを実装しています：

### Clean Architecture実装
- **Domain層**: TodoTask Entity + ITodoRepository Interface（完全実装済み）
- **Application層**: TodoUseCase（統合型CRUD操作、完全実装済み）
- **Infrastructure層**: CsvTodoRepository（ファイル永続化、完全実装済み）
- **Presentation層**: MVVM + R3 + VContainer（リアクティブUI、完全実装済み）

### MVVM実装詳細
- **ViewModel**: R3のReactivePropertyによる状態管理
- **View**: Unity UI + 双方向データバインディング
- **Presenter**: VContainer EntryPointパターンによるビジネスロジック

### 技術的特徴
- **VContainer**: 依存性注入による疎結合の実現
- **UniTask**: 非同期処理の効率的な管理
- **R3**: リアクティブプログラミングによるデータフローの管理
- **TDD**: 22のテストケースによる品質保証
- **SOLID原則**: 依存関係逆転の原則（DIP）完全実装

## 開発環境とツール

- **Unity Editor**: 2022.3.61f1 LTS
- **言語**: C# (.NET Standard 2.1対応)
- **テスト**: EditModeテスト（22ケース実装済み）
- **DI**: VContainer による依存性注入
- **UI構築**: EditorScript（TodoUIBuilder）による自動化
- **モバイル対応**: SafeAreaHandler実装済み

## 実装済み機能

### Domain層実装（完全実装済み）
- **TodoTask Entity**: ID自動生成、TimeProvider注入、完了状態管理
- **ITodoRepository Interface**: CRUD操作インターフェース定義

### Application層実装（完全実装済み）
- **TodoUseCase**: 統合型UseCaseパターン、全基本操作実装

### Infrastructure層実装（完全実装済み）
- **CsvTodoRepository**: CSV永続化実装、エラーハンドリング対応
- **RootLifetimeScope**: VContainer DI設定、適切なライフタイム管理

### Presentation層実装（完全実装済み）
- **TodoListViewModel**: R3 ReactiveProperty/Command活用
- **TodoListPresenter**: VContainer EntryPointパターン
- **TodoListView**: 完全なUIバインディング
- **TodoItemView**: 個別アイテム表示、視覚的フィードバック

### UI・EditorScript実装（完全実装済み）
- **TodoUIBuilder**: 846行の包括的EditorScript、冪等性保証
- **SafeAreaHandler**: モバイルデバイス対応、デバッグ機能充実

### テスト実装（完全実装済み）
- **TodoTaskTest**: Domain層テスト（7ケース）
- **TodoUseCaseTest**: Application層テスト（8ケース）
- **TodoListViewModelTest**: Presentation層テスト（7ケース）
- **MockTimeProvider**: テストユーティリティ

## Assembly Definition Files構成

レイヤー間の依存関係を強制的に分離：

- **UnityCleanArchitectureTodo.Domain.asmdef**: Domain層
- **UnityCleanArchitectureTodo.App.asmdef**: Application層
- **UnityCleanArchitectureTodo.Infra.asmdef**: Infrastructure層
- **UnityCleanArchitectureTodo.Presentation.asmdef**: Presentation層
- **テスト用asmdefファイル**: 各層のテスト分離

## ファイル構成詳細

```
UnityCleanArchitechtureTodo/
├── Assets/
│   ├── Scripts/
│   │   ├── Domain/           # ドメイン層（完全実装済み）
│   │   │   ├── Entities/     # TodoTask Entity
│   │   │   └── Repositories/ # ITodoRepository Interface
│   │   ├── App/              # アプリケーション層（完全実装済み）
│   │   │   └── UseCases/     # TodoUseCase
│   │   ├── Infra/            # インフラ層（完全実装済み）
│   │   │   ├── Repositories/ # CsvTodoRepository
│   │   │   └── LifetimeScope/# RootLifetimeScope（DI設定）
│   │   └── Presentation/     # プレゼンテーション層（MVVM完全実装済み）
│   │       ├── ViewModels/   # TodoListViewModel
│   │       ├── Presenters/   # TodoListPresenter
│   │       ├── Views/        # TodoListView, TodoItemView
│   │       └── UI/           # SafeAreaHandler
│   ├── Tests/                # テストコード（22ケース実装済み）
│   │   ├── EditMode/         # 単体テスト
│   │   └── PlayMode/         # 統合テスト（設定済み）
│   ├── Editor/               # 開発ツール（完全実装済み）
│   │   └── TodoUIBuilder.cs  # UI自動構築EditorScript
│   ├── Prefabs/              # UIプレファブ
│   ├── Scenes/               # Main.unity シーン
│   └── Packages/             # NuGetパッケージ
├── Packages/                 # Unity Package Manager設定
├── ProjectSettings/          # プロジェクト設定
├── README.md                 # プロジェクト説明（包括的）
├── Spec.md                   # 詳細仕様書
└── Task.md                   # 実装タスク管理（完了済み）
```

## 開発手法とパターン

### TDD（テスト駆動開発）実装
- **Red-Green-Refactor**: サイクル完全実施
- **モックオブジェクト**: 適切なテスト分離
- **非同期テスト**: UniTask対応
- **TimeProvider**: テスト可能な時刻実装

### Clean Architecture実装
- **依存関係の流れ**: Presentation → Application → Domain ← Infrastructure
- **依存関係逆転の原則**: SOLID-D完全実装
- **レイヤー間疎結合**: Assembly Definition Files強制分離

### MVVM + リアクティブプログラミング
- **双方向データバインディング**: R3による実装
- **状態管理**: ReactiveProperty活用
- **コマンドパターン**: ReactiveCommand実装
- **メモリリーク防止**: CompositeDisposable活用

## 重要な実装パターン

### VContainer EntryPointパターン
```csharp
public class TodoListPresenter : IStartable, IDisposable
{
    // 自動初期化、ライフサイクル管理
}
```

### R3 ReactiveProperty活用
```csharp
public ReactiveProperty<string> NewTodoTitle { get; }
public ReadOnlyReactiveProperty<IReadOnlyList<TodoTask>> Todos { get; }
```

### 依存関係逆転の原則実装
```csharp
// Domain層でインターフェース定義、Infrastructure層で実装
public interface ITodoRepository { }
public class CsvTodoRepository : ITodoRepository { }
```

## プロジェクトの品質レベル

このプロジェクトは以下の品質基準を満たしています：

- ✅ **プロダクション品質**: 実用レベルの実装
- ✅ **教育的価値**: Clean Architecture学習の完全な参考実装
- ✅ **保守性**: 適切な設計パターンによる高い保守性
- ✅ **テスト品質**: 包括的なテストカバレッジ
- ✅ **拡張性**: 新機能追加の容易性
- ✅ **モバイル対応**: SafeArea対応済み

## 開発・保守時の注意事項

1. **Clean Architectureの原則**: レイヤー間の依存関係を遵守
2. **TDDサイクル**: 新機能追加時はテストファーストで実装
3. **MVVM分離**: ViewとViewModelの責務を明確に分離
4. **R3使用法**: ReactivePropertyのDisposeを適切に管理
5. **VContainer設定**: ライフタイムの適切な設定
6. **Assembly Definition**: 依存関係違反の防止
7. **EditorScript**: UI構築の冪等性保持