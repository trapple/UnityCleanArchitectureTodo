# Unity Clean Architecture Todo

UnityでClean ArchitectureパターンとMVVMアーキテクチャを適用したTodoアプリケーション

## 🏗️ アーキテクチャ概要

### Clean Architecture + MVVM
本プロジェクトは**Clean Architecture**をベースに、**Presentation層にMVVMアーキテクチャ**を採用した設計です。

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │    View     │  │ ViewModel   │  │  Presenter  │    │
│  │  (Unity UI) │→ │ (R3 State)  │← │ (Business)  │    │
│  └─────────────┘  └─────────────┘  └─────────────┘    │
└─────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────┐
│                   Application                          │
│         TodoUseCase (統合型CRUD操作)                    │
└─────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────┐
│                     Domain                             │
│  TodoTask Entity + ITodoRepository Interface           │
└─────────────────────────────────────────────────────────┘
                              ↑
┌─────────────────────────────────────────────────────────┐
│                 Infrastructure                         │
│      CsvTodoRepository (ファイル永続化)                 │
└─────────────────────────────────────────────────────────┘
```

### Clean Architectureの詳細実装

#### 🏗️ Domain層（ビジネスルール）
- **役割**: ビジネスロジックとエンティティの管理
- **技術**: C# Pure Classes + Interfaces
- **特徴**: 外部依存ゼロ、最も内側の層

```csharp
// エンティティ：ビジネスの核となるオブジェクト
public class TodoTask
{
    public string Id { get; }
    public string Title { get; private set; }
    public bool IsCompleted { get; private set; }
    
    // ビジネスルールをメソッドとして表現
    public void Complete() => IsCompleted = true;
    public void UpdateTitle(string newTitle) { /* バリデーション + 更新 */ }
}

// リポジトリインターフェース：データアクセスの抽象化
public interface ITodoRepository
{
    UniTask<IReadOnlyList<TodoTask>> GetAllAsync();
    UniTask SaveAsync(TodoTask task);
}
```

#### 🎯 Application層（ユースケース）
- **役割**: アプリケーション固有のビジネスロジック
- **技術**: Domain Entities + Repository Interfaces
- **特徴**: 外部世界との橋渡し、オーケストレーション

```csharp
public class TodoUseCase
{
    private readonly ITodoRepository _repository;
    
    // アプリケーション固有のビジネスフロー
    public async UniTask CreateAsync(string title, string description)
    {
        var task = new TodoTask(title, description); // Domain Entity使用
        await _repository.SaveAsync(task); // Repository Interface使用
    }
    
    // 複数のDomainオブジェクトを組み合わせた処理
    public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        => await _repository.GetAllAsync();
}
```

#### 🔧 Infrastructure層（技術詳細）
- **役割**: 外部システムとの実際の通信
- **技術**: File I/O, Database, Web API等
- **特徴**: Domain Interfaceの具象実装、最も外側の層

```csharp
public class CsvTodoRepository : ITodoRepository
{
    // 具体的な永続化技術（CSV）
    public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
    {
        var csvContent = await File.ReadAllTextAsync(_filePath);
        return ParseCsvToTodoTasks(csvContent); // CSV固有のロジック
    }
}
```

### MVVMアーキテクチャの詳細実装（Presentation層）

#### 🎯 View (Unity UI)
- **役割**: UI表示とユーザー入力の受付
- **技術**: Unity UI Components
- **特徴**: ViewModelとの双方向データバインディング

```csharp
// ViewはViewModelの状態を監視し、UIに反映
_viewModel.Todos
    .Subscribe(OnTodosChanged)
    .AddTo(_disposables);

// UIイベントをViewModelのCommandにバインド
_addButton.onClick.AsObservable()
    .Subscribe(_ => _viewModel.CreateTodoCommand.Execute(Unit.Default))
    .AddTo(_disposables);
```

#### 🧠 ViewModel (状態管理)
- **役割**: UI状態の管理とCommandの定義
- **技術**: R3 ReactiveProperty + ReactiveCommand
- **特徴**: 純粋な状態とコマンドのみ、ビジネスロジックは含まない

```csharp
public class TodoListViewModel : IDisposable
{
    // 状態管理
    public ReadOnlyReactiveProperty<IReadOnlyList<TodoTask>> Todos { get; }
    public ReadOnlyReactiveProperty<bool> IsLoading { get; }
    public ReactiveProperty<string> NewTodoTitle { get; }
    
    // コマンド定義
    public ReactiveCommand CreateTodoCommand { get; }
    public ReactiveCommand<string> ToggleCompleteCommand { get; }
    public ReactiveCommand<string> DeleteTodoCommand { get; }
}
```

#### 🎬 Presenter (ビジネスロジック)
- **役割**: ViewModelとUseCaseの橋渡し、ビジネスロジックの実行
- **技術**: VContainer EntryPoint + UniTask
- **特徴**: CommandとUseCaseのバインディング、自動初期化

```csharp
public class TodoListPresenter : IStartable, IDisposable
{
    // ViewModelのCommandをUseCaseにバインド
    private void BindCommands()
    {
        _viewModel.CreateTodoCommand
            .Subscribe(_ => OnCreateTodoAsync().Forget())
            .AddTo(_disposables);
    }
    
    // ビジネスロジックの実行
    private async UniTask OnCreateTodoAsync()
    {
        await _todoUseCase.CreateAsync(_viewModel.NewTodoTitle.Value, 
                                     _viewModel.NewTodoDescription.Value);
        await LoadTodosAsync();
    }
}
```

#### 🏛️ 依存関係逆転の原則（SOLID-D）実装
- **役割**: Domain層とInfra層の疎結合を実現
- **技術**: インターフェースという抽象化に対する依存
- **特徴**: Domain層はRepositoryの抽象に依存し、実装詳細を知らない設計

```csharp
// Domain層: インターフェース定義（抽象化）
namespace UnityCleanArchitectureTodo.Domain.Repositories
{
    public interface ITodoRepository
    {
        UniTask<IReadOnlyList<TodoTask>> GetAllAsync();
        UniTask SaveAsync(TodoTask task);
        UniTask DeleteAsync(string id);
    }
}

// Infrastructure層: 具象実装（詳細）
namespace UnityCleanArchitectureTodo.Infra.Repositories  
{
    public class CsvTodoRepository : ITodoRepository
    {
        // CSVファイル永続化の具体的実装
        public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync() { ... }
    }
}
```

#### 🔄 依存関係の流れ
```
Application層 → Domain層（ITodoRepository）← Infrastructure層（CsvTodoRepository）
     ↓              ↓                              ↓
  ビジネス        抽象化                      技術実装詳細
  ロジック     インターフェース               （CSV, DB等）
```

**メリット**:
- **テスタビリティ**: Mockオブジェクトでの単体テスト容易
- **拡張性（SOLID-O）**: CSV → Database切り替え時、Domain/App層に影響なし  
- **保守性**: ビジネスロジックと技術詳細の完全分離

## 🛠️ 技術スタック

### Core Framework
- **Unity** - ゲームエンジン・UI框架
- **Clean Architecture** - レイヤー分離アーキテクチャ
- **MVVM Pattern** - Presentation層のアーキテクチャパターン

### 依存性注入・非同期処理
- **VContainer** - 依存性注入コンテナ
- **UniTask** - 高性能非同期処理ライブラリ
- **R3** - リアクティブプログラミングライブラリ

### 開発・品質保証
- **TDD (Test-Driven Development)** - テスト駆動開発
- **Assembly Definition Files** - レイヤー分離の強制

## 🎯 主要機能

### Todoアプリ機能
- ✅ **タスク作成** - タイトル・説明付きタスク作成
- ✅ **完了切り替え** - リアルタイム状態更新
- ✅ **タスク削除** - 即座削除・リスト更新
- ✅ **データ永続化** - CSV形式ローカル保存
- ✅ **統計表示** - タスク数・完了数表示
- ✅ **ローディング状態** - 非同期操作の視覚的フィードバック

## 📁 プロジェクト構造

```
UnityCleanArchitechtureTodo/
├── Assets/
│   ├── Scripts/
│   │   ├── Domain/           # ドメイン層
│   │   │   ├── Entities/     # エンティティ（TodoTask）
│   │   │   └── Repositories/ # リポジトリインターフェース
│   │   ├── App/              # アプリケーション層  
│   │   │   └── UseCases/     # ユースケース（TodoUseCase）
│   │   ├── Infra/            # インフラ層
│   │   │   ├── Repositories/ # リポジトリ実装（CsvTodoRepository）
│   │   │   └── LifetimeScope/# DI設定（RootLifetimeScope）
│   │   └── Presentation/     # プレゼンテーション層（MVVM）
│   │       ├── ViewModels/   # ViewModel（状態管理）
│   │       ├── Presenters/   # Presenter（ビジネスロジック）
│   │       ├── Views/        # View（Unity UI）
│   │       └── UI/           # UI共通コンポーネント
│   ├── Tests/                # テストコード（TDD）
│   ├── Editor/               # 開発ツール（TodoUIBuilder）
│   ├── Prefabs/              # UIプレファブ
│   └── Scenes/               # シーンファイル
├── Spec.md                   # 詳細仕様書
└── Task.md                   # 実装タスク管理
```

## 🎓 学習ポイント

### Clean Architecture実装
- **依存関係の流れ**: Presentation → Application → Domain ← Infrastructure
- **依存関係逆転の原則**: InfrastructureがDomainのインターフェースに依存
- **レイヤー間の疎結合**: Assembly Definition Filesによる強制分離

### MVVM + リアクティブプログラミング
- **View**: UIコンポーネントとイベント処理
- **ViewModel**: R3による状態管理とコマンド定義
- **Presenter**: ビジネスロジックとUseCase連携
- **双方向データバインディング**: リアルタイムUI更新

### TDD実践
- **Red Phase**: 失敗するテストを先に書く
- **Green Phase**: テストが通る最小実装
- **Refactor Phase**: コードの改善・最適化

## 📄 ライセンス

MIT License

---

**🏆 UnityでClean Architecture + MVVMを学ぶための完全なサンプルプロジェクト**