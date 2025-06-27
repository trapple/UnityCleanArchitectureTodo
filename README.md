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

### MVVMアーキテクチャの詳細実装

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

## 🛠️ 技術スタック

### Core Framework
- **Unity** - ゲームエンジン・UI框架
- **Clean Architecture** - レイヤー分離アーキテクチャ
- **MVVM Pattern** - Presentation層のアーキテクチャパターン

### 依存性注入・非同期処理
- **VContainer** - 依存性注入コンテナ
- **UniTask** - 高性能非同期処理ライブラリ
- **R3** - リアクティブプログラミングライブラリ

### UI・表示
- **NotoSansJP** - 日本語フォント対応
- **SafeAreaHandler** - モバイル端末SafeArea自動対応

### 開発・品質保証
- **TDD (Test-Driven Development)** - テスト駆動開発
- **EditorScript** - UI構築自動化ツール
- **Assembly Definition Files** - レイヤー分離の強制

## 🎯 主要機能

### Todoアプリ機能
- ✅ **タスク作成** - タイトル・説明付きタスク作成
- ✅ **完了切り替え** - リアルタイム状態更新
- ✅ **タスク削除** - 即座削除・リスト更新
- ✅ **データ永続化** - CSV形式ローカル保存
- ✅ **統計表示** - タスク数・完了数表示
- ✅ **ローディング状態** - 非同期操作の視覚的フィードバック

### 技術的特徴
- 🏗️ **完全な層分離** - Domain, Application, Infrastructure, Presentation
- 🧪 **TDD品質保証** - Red-Green-Refactorサイクル
- ⚛️ **リアクティブUI** - R3による双方向データバインディング
- 🔧 **開発効率化** - EditorScriptによる自動UI構築
- 📱 **モバイル対応** - iPhone/AndroidのSafeArea自動対応
- 🌏 **国際化対応** - 日本語フォント完全対応

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

## 🚀 セットアップ

### 必要環境
- Unity 2022.3 LTS以上
- .NET Standard 2.1対応

### 依存パッケージ
```json
{
  "dependencies": {
    "com.cysharp.unitask": "2.5.10",
    "jp.cysharp.vcontainer": "1.16.9", 
    "com.cysharp.r3": "1.3.0"
  }
}
```

### 実行方法
1. Unityプロジェクトを開く
2. `Assets/Scenes/Main.unity`を開く
3. Playボタンでアプリ実行

### UI再構築（開発者向け）
- Unity上部メニュー: `Todo App > Build UI > Build All UI (Complete Setup)`
- 自動でCanvas、SafeArea、全UI要素が構築されます

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