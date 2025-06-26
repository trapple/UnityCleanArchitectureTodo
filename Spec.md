# Unity Clean Architecture Todo アプリ設計仕様書

## 1. プロジェクト概要

### 1.1 プロジェクト名
UnityCleanArchitectureTodo

### 1.2 目的
UnityでClean Architectureパターンを適用したTodoアプリケーションの実装

### 1.3 使用技術
- **Unity**: ゲームエンジン・UI框架
- **VContainer**: 依存性注入コンテナ
- **UniTask**: 非同期処理ライブラリ
- **R3**: リアクティブプログラミングライブラリ
- **TextMeshPro**: テキスト表示

## 2. 要件定義

### 2.1 機能要件

#### 基本機能
- **タスク作成**: 新しいTodoタスクを追加
- **タスク表示**: Todoリストの一覧表示
- **タスク完了**: タスクの完了/未完了の切り替え
- **タスク削除**: 不要なタスクの削除
- **タスク編集**: 既存タスクの内容編集

#### 詳細機能
- タスクにはタイトルと説明を設定可能
- 作成日時と完了日時の記録
- タスクの並び順は作成日時順
- 完了済みタスクの視覚的区別

### 2.2 非機能要件

#### 技術要件
- **データ永続化**: CSVファイルでの保存（1行1レコード形式）
- **リアクティブUI**: R3によるデータバインディング
- **非同期処理**: UniTaskによる非同期操作
- **依存性注入**: VContainerによる疎結合設計

#### 品質要件
- テスタブルな設計
- 保守性の高いコード構造
- パフォーマンスを考慮した実装

## 3. アーキテクチャ設計

### 3.1 Clean Architecture レイヤー構成

```
Assets/Scripts/
├── Domain/                  # ドメインレイヤー
│   ├── Entities/           # エンティティ
│   └── Repositories/       # リポジトリインターフェース
├── App/                   # アプリケーションレイヤー
│   ├── UseCases/          # ユースケース
│   └── Services/         # アプリケーションサービス
├── Infra/                 # インフラレイヤー
│   └── Repositories/      # リポジトリ実装
└── Presentation/          # プレゼンテーションレイヤー
    ├── ViewModels/       # ビューモデル
    ├── Presenters/       # プレゼンター
    └── Views/            # ビュー
```

### 3.2 依存関係の流れ

#### 3.2.1 大きく4層の依存関係

```
Presentation → App → Domain ← Infra
```

- **Presentation層**: App層に依存（UseCaseを呼び出し）
- **App層**: Domain層に依存（Entity、Repositoryインターフェースを使用）
- **Infra層**: Domain層に依存（依存関係逆転の原則でRepositoryインターフェースを実装）
- **Domain層**: 他のレイヤーに依存しない（純粋なビジネスロジック）

#### 3.2.2 Presentation層内部の依存関係

```
View → ViewModel ← Presenter
```

- **View**: ViewModelに依存（データバインディング）
- **Presenter**: ViewModelに依存（状態更新、コマンド処理）
- **ViewModel**: 他に依存しない（状態とコマンドの定義のみ）

**MVVMパターンの特徴**:
- ViewとPresenterは直接依存しない
- ViewModelが状態の中心となる
- Presenterがビジネスロジックを担当
- Viewが純粋な表示を担当

### 3.3 ライブラリ依存関係ルール

- **Domainレイヤー**: UniTask、R3、VContainerに依存しない（純粋なC#のみ）
- **App、Infra、Presentationレイヤー**: UniTask、R3、VContainerに依存可能
  - UniTask: 非同期処理の実装
  - R3: リアクティブプログラミングの実装
  - VContainer: 依存性注入の実装

## 4. 詳細設計

### 4.1 Domainレイヤー

#### 4.1.1 Entity設計

**TodoTask Entity**
```csharp
public class TodoTask
{
    public string Id { get; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? CompletedAt { get; private set; }
    
    // ビジネスロジックメソッド
    public void Complete()
    public void Uncomplete()
    public void UpdateTitle(string title)
    public void UpdateDescription(string description)
    
    // ID生成メソッド
    public static string GenerateNewId() => Guid.NewGuid().ToString()
}
```

#### 4.1.2 Repository Interface

**ITodoRepository**
```csharp
public interface ITodoRepository
{
    UniTask<IReadOnlyList<TodoTask>> GetAllAsync();
    UniTask<TodoTask> GetByIdAsync(string id);
    UniTask SaveAsync(TodoTask task);
    UniTask DeleteAsync(string id);
}
```

### 4.2 Appレイヤー（Applicationレイヤー）

#### 4.2.1 UseCase設計

**UseCase一覧**
- `GetAllTodosUseCase`: 全タスク取得
- `CreateTodoUseCase`: タスク作成
- `UpdateTodoUseCase`: タスク更新
- `CompleteTodoUseCase`: タスク完了切り替え
- `DeleteTodoUseCase`: タスク削除

**UseCase実装例**
```csharp
public class GetAllTodosUseCase
{
    private readonly ITodoRepository _repository;
    
    public GetAllTodosUseCase(ITodoRepository repository)
    {
        _repository = repository;
    }
    
    public async UniTask<IReadOnlyList<TodoTask>> ExecuteAsync()
    {
        return await _repository.GetAllAsync();
    }
}
```

### 4.3 Infraレイヤー（Infrastructureレイヤー）

#### 4.3.1 Repository実装

**CsvTodoRepository**
```csharp
public class CsvTodoRepository : ITodoRepository
{
    private readonly string _filePath;
    private readonly List<TodoTask> _cachedTodos;
    
    public CsvTodoRepository(string filePath)
    {
        _filePath = filePath;
        _cachedTodos = new List<TodoTask>();
    }
    
    public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
    {
        await LoadFromFileAsync();
        return _cachedTodos.AsReadOnly();
    }
    
    public async UniTask<TodoTask> GetByIdAsync(string id)
    {
        await LoadFromFileAsync();
        return _cachedTodos.FirstOrDefault(t => t.Id == id);
    }
    
    public async UniTask SaveAsync(TodoTask task)
    {
        await LoadFromFileAsync();
        var existingIndex = _cachedTodos.FindIndex(t => t.Id == task.Id);
        if (existingIndex >= 0)
            _cachedTodos[existingIndex] = task;
        else
            _cachedTodos.Add(task);
        await SaveToFileAsync();
    }
    
    public async UniTask DeleteAsync(string id)
    {
        await LoadFromFileAsync();
        _cachedTodos.RemoveAll(t => t.Id == id);
        await SaveToFileAsync();
    }
    
    private async UniTask LoadFromFileAsync()
    {
        if (!File.Exists(_filePath))
        {
            _cachedTodos.Clear();
            return;
        }
        
        var csvContent = await File.ReadAllTextAsync(_filePath);
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        _cachedTodos.Clear();
        // Skip header line
        foreach (var line in lines.Skip(1))
        {
            var task = CsvHelper.FromCsvLine(line);
            if (task != null)
                _cachedTodos.Add(task);
        }
    }
    
    private async UniTask SaveToFileAsync()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        var csvLines = new List<string>
        {
            "Id,Title,Description,IsCompleted,CreatedAtTicks,CompletedAtTicks"
        };
        
        csvLines.AddRange(_cachedTodos.Select(CsvHelper.ToCsvLine));
        
        await File.WriteAllTextAsync(_filePath, string.Join("\n", csvLines));
    }
}
```

### 4.4 Presentationレイヤー

#### 4.4.1 ViewModel設計

**TodoListViewModel**
```csharp
public class TodoListViewModel : IDisposable
{
    private readonly ReactiveProperty<IReadOnlyList<TodoTask>> _todos;
    private readonly ReactiveProperty<bool> _isLoading;
    private readonly ReactiveProperty<string> _newTodoTitle;
    private readonly ReactiveProperty<string> _newTodoDescription;
    private readonly CompositeDisposable _disposables;
    
    public TodoListViewModel()
    {
        _todos = new ReactiveProperty<IReadOnlyList<TodoTask>>(new List<TodoTask>());
        _isLoading = new ReactiveProperty<bool>(false);
        _newTodoTitle = new ReactiveProperty<string>("");
        _newTodoDescription = new ReactiveProperty<string>("");
        _disposables = new CompositeDisposable();
        
        // Commands
        CreateTodoCommand = _newTodoTitle
            .Select(title => !string.IsNullOrWhiteSpace(title))
            .ToReactiveCommand()
            .AddTo(_disposables);
            
        DeleteTodoCommand = new ReactiveCommand<string>()
            .AddTo(_disposables);
            
        ToggleCompleteCommand = new ReactiveCommand<string>()
            .AddTo(_disposables);
    }
    
    // Properties
    public IReadOnlyReactiveProperty<IReadOnlyList<TodoTask>> Todos => _todos;
    public IReadOnlyReactiveProperty<bool> IsLoading => _isLoading;
    public ReactiveProperty<string> NewTodoTitle => _newTodoTitle;
    public ReactiveProperty<string> NewTodoDescription => _newTodoDescription;
    
    // Commands
    public ReactiveCommand CreateTodoCommand { get; }
    public ReactiveCommand<string> DeleteTodoCommand { get; }
    public ReactiveCommand<string> ToggleCompleteCommand { get; }
    
    // Internal methods for Presenter
    public void SetTodos(IReadOnlyList<TodoTask> todos) => _todos.Value = todos;
    public void SetLoading(bool isLoading) => _isLoading.Value = isLoading;
    public void ClearNewTodoInputs()
    {
        _newTodoTitle.Value = "";
        _newTodoDescription.Value = "";
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
        _todos?.Dispose();
        _isLoading?.Dispose();
        _newTodoTitle?.Dispose();
        _newTodoDescription?.Dispose();
    }
}
```

#### 4.4.2 Presenter設計

**TodoListPresenter**
```csharp
public class TodoListPresenter : IStartable, IDisposable
{
    private readonly TodoListViewModel _viewModel;
    private readonly CompositeDisposable _disposables;
    
    private readonly GetAllTodosUseCase _getAllTodosUseCase;
    private readonly CreateTodoUseCase _createTodoUseCase;
    private readonly CompleteTodoUseCase _completeTodoUseCase;
    private readonly DeleteTodoUseCase _deleteTodoUseCase;
    
    // Constructor Injection
    public TodoListPresenter(
        TodoListViewModel viewModel,
        GetAllTodosUseCase getAllTodosUseCase,
        CreateTodoUseCase createTodoUseCase,
        CompleteTodoUseCase completeTodoUseCase,
        DeleteTodoUseCase deleteTodoUseCase)
    {
        _viewModel = viewModel;
        _getAllTodosUseCase = getAllTodosUseCase;
        _createTodoUseCase = createTodoUseCase;
        _completeTodoUseCase = completeTodoUseCase;
        _deleteTodoUseCase = deleteTodoUseCase;
        
        _disposables = new CompositeDisposable();
    }
    
    // EntryPoint Start
    public void Start()
    {
        // Commandをバインド
        _viewModel.CreateTodoCommand
            .Subscribe(_ => OnCreateTodo())
            .AddTo(_disposables);
            
        _viewModel.DeleteTodoCommand
            .Subscribe(OnDeleteTodo)
            .AddTo(_disposables);
            
        _viewModel.ToggleCompleteCommand
            .Subscribe(OnToggleComplete)
            .AddTo(_disposables);
        
        // 初期ロード
        LoadTodosAsync().Forget();
    }
    
    private async void OnCreateTodo()
    {
        var title = _viewModel.NewTodoTitle.Value;
        var description = _viewModel.NewTodoDescription.Value;
        
        await _createTodoUseCase.ExecuteAsync(title, description);
        _viewModel.ClearNewTodoInputs();
        await LoadTodosAsync();
    }
    
    private async void OnDeleteTodo(string id)
    {
        await _deleteTodoUseCase.ExecuteAsync(id);
        await LoadTodosAsync();
    }
    
    private async void OnToggleComplete(string id)
    {
        await _completeTodoUseCase.ExecuteAsync(id);
        await LoadTodosAsync();
    }
    
    private async UniTask LoadTodosAsync()
    {
        _viewModel.SetLoading(true);
        try
        {
            var todos = await _getAllTodosUseCase.ExecuteAsync();
            _viewModel.SetTodos(todos);
        }
        finally
        {
            _viewModel.SetLoading(false);
        }
    }
    
    public void Dispose()
    {
        _disposables?.Dispose();
    }
}
```

#### 4.4.3 View設計

**TodoListView**
```csharp
public class TodoListView : MonoBehaviour
{
    [SerializeField] private Transform _todoListParent;
    [SerializeField] private TodoItemView _todoItemPrefab;
    [SerializeField] private Button _addButton;
    [SerializeField] private TMP_InputField _newTodoTitleInput;
    [SerializeField] private TMP_InputField _newTodoDescriptionInput;
    [SerializeField] private GameObject _loadingIndicator;
    
    private TodoListViewModel _viewModel;
    private readonly List<TodoItemView> _todoItemViews = new();
    private readonly CompositeDisposable _disposables = new();
    
    [Inject]
    public void Construct(TodoListViewModel viewModel)
    {
        _viewModel = viewModel;
    }
    
    private void Start()
    {
        BindToViewModel();
    }
    
    private void OnDestroy()
    {
        _disposables?.Dispose();
    }
    
    private void BindToViewModel()
    {
        // ViewModelの状態をUIにバインド
        _viewModel.Todos
            .Subscribe(OnTodosChanged)
            .AddTo(_disposables);
            
        _viewModel.IsLoading
            .Subscribe(isLoading => _loadingIndicator.SetActive(isLoading))
            .AddTo(_disposables);
        
        // InputフィールドをViewModelにバインド
        _viewModel.NewTodoTitle
            .BindTo(_newTodoTitleInput)
            .AddTo(_disposables);
            
        _viewModel.NewTodoDescription
            .BindTo(_newTodoDescriptionInput)
            .AddTo(_disposables);
        
        // CommandをUIイベントにバインド
        _addButton.onClick.AsObservable()
            .Subscribe(_ => _viewModel.CreateTodoCommand.Execute())
            .AddTo(_disposables);
    }
    
    private void OnTodosChanged(IReadOnlyList<TodoTask> todos)
    {
        // 既存のアイテムをクリア
        foreach (var item in _todoItemViews)
            item.Cleanup();
        _todoItemViews.Clear();
        
        // 新しいアイテムを作成
        foreach (var todo in todos)
        {
            var itemView = Instantiate(_todoItemPrefab, _todoListParent);
            itemView.Setup(todo, 
                id => _viewModel.ToggleCompleteCommand.Execute(id),
                id => _viewModel.DeleteTodoCommand.Execute(id));
            _todoItemViews.Add(itemView);
        }
    }
}
```

**TodoItemView**
```csharp
public class TodoItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _createdAtText;
    [SerializeField] private Toggle _completedToggle;
    [SerializeField] private Button _deleteButton;
    
    private TodoTask _currentTask;
    private readonly CompositeDisposable _disposables = new();
    
    public void Setup(TodoTask task, System.Action<string> onToggleComplete, System.Action<string> onDelete)
    public void Cleanup()
    private void UpdateUI()
    private void OnToggleChanged(bool isOn)
    private void OnDeleteClicked()
}
```

### 4.5 DI設定

#### 4.5.1 VContainer LifetimeScope

**TodoAppLifetimeScope**
```csharp
public class TodoAppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Repository
        builder.Register<ITodoRepository, CsvTodoRepository>(Lifetime.Singleton)
            .WithParameter("filePath", Path.Combine(Application.persistentDataPath, "todos.csv"));
        
        // UseCases
        builder.Register<GetAllTodosUseCase>(Lifetime.Transient);
        builder.Register<CreateTodoUseCase>(Lifetime.Transient);
        builder.Register<UpdateTodoUseCase>(Lifetime.Transient);
        builder.Register<CompleteTodoUseCase>(Lifetime.Transient);
        builder.Register<DeleteTodoUseCase>(Lifetime.Transient);
        
        // ViewModel
        builder.Register<TodoListViewModel>(Lifetime.Singleton);
        
        // Presenters (EntryPoint)
        builder.RegisterEntryPoint<TodoListPresenter>();
        
        // Views
        builder.RegisterComponentInHierarchy<TodoListView>();
    }
}
```

## 5. データモデル

### 5.1 永続化データ構造

**CSV形式データ**
```
# CSVファイル形式（todos.csv）
# ヘッダー行
Id,Title,Description,IsCompleted,CreatedAtTicks,CompletedAtTicks

# データ行例
"12345-abcde","買い物","牛乳と卵を買う",false,638123456789012345,
"67890-fghij","会議資料作成","明日のプレゼン資料を準備する",true,638123456789012346,638123456789012400
```

**CSV操作用ユーティリティ**
```csharp
public static class CsvHelper
{
    public static string ToCsvLine(TodoTask task)
    public static TodoTask FromCsvLine(string csvLine)
    public static string EscapeCsvField(string field)
    public static string[] ParseCsvLine(string csvLine)
}
```

## 6. UI設計

### 6.1 画面構成

**メイン画面**
- ヘッダー: アプリタイトル
- 新規作成エリア: タイトル入力、説明入力、追加ボタン
- タスクリスト: スクロール可能なタスク一覧
- フッター: 統計情報（総数、完了数など）

**タスクアイテム**
- タイトル表示
- 説明表示（折りたたみ可能）
- 完了チェックボックス
- 作成日時表示
- 削除ボタン

### 6.2 インタラクション

- タスク追加: 入力後「追加」ボタンクリック
- 完了切り替え: チェックボックスをタップ
- タスク削除: 削除ボタンをタップ（確認ダイアログ表示）

## 7. 実装順序

### 7.1 推奨実装順序

1. **Domain層の実装**
   - TodoTask (Entity)
   - ITodoRepository (Interface)

2. **App層の実装**
   - 各UseCase実装

3. **Infra層の実装**
   - CsvTodoRepository実装
   - CSVファイル操作ロジック

4. **Presentation層の実装**
   - TodoListViewModel実装
   - TodoListPresenter実装
   - TodoListView実装
   - TodoItemView実装

5. **DI設定の実装**
   - TodoAppLifetimeScope実装

6. **UI構築**
   - Scene設定
   - Prefab作成
   - UI配置

## 8. テスト方針

### 8.1 テスト対象

- **Unit Test**: Domain層、UseCase層
- **Integration Test**: Repository実装
- **UI Test**: Presenter層

### 8.2 テスト戦略

- 依存性注入により外部依存をモック化
- UniTaskを使用した非同期テスト
- R3のReactivePropertyテスト

## 9. 技術的考慮事項

### 9.1 パフォーマンス

- タスクリストの効率的な更新（差分更新）
- メモリ使用量の最適化
- UI更新の最適化

### 9.2 拡張性

- 新機能追加時のレイヤー構造維持
- データソース変更の容易性
- UI変更の容易性

### 9.3 保守性

- 各レイヤーの責任分離
- インターフェースベースの設計
- 設定の外部化

## 10. 既知の制約事項

### 10.1 技術的制約

- ファイルシステムへの書き込み権限
- モバイル環境でのファイルアクセス制限
- Unityのマルチスレッド制限

### 10.2 機能的制約

- 単一デバイスでの使用
- オフライン専用
- 基本的なCRUD操作のみ

---

この仕様書に基づいて、Clean Architectureパターンを適用したUnity Todoアプリケーションを実装することができます。