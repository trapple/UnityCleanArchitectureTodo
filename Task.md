# 実装タスク管理

Spec.md 7.2推奨実装順序に基づく進捗管理

## 1. Domain層の実装（TDD）

### 1.1 TodoTask Entity のテスト
- ✅ TodoTaskTest.cs作成（TDD Red Phase）
- ✅ Constructor_ShouldSetPropertiesCorrectly テスト
- ✅ Complete_ShouldMarkAsCompleted テスト
- ✅ Uncomplete_ShouldMarkAsIncomplete テスト
- ✅ UpdateTitle_ShouldChangeTitle テスト
- ✅ UpdateTitle_WithNullOrEmpty_ShouldThrowException テスト
- ✅ UpdateDescription_ShouldChangeDescription テスト
- ✅ GenerateNewId_ShouldReturnNonEmptyString テスト

### 1.2 TodoTask Entity の実装
- ✅ TodoTask.cs基本構造作成（空実装でTDD Red Phase）
- ✅ Constructor実装（TDD Green Phase）
- ✅ Complete()メソッド実装
- ✅ Uncomplete()メソッド実装
- ✅ UpdateTitle()メソッド実装
- ✅ UpdateDescription()メソッド実装
- ✅ GenerateNewId()静的メソッド実装
- ✅ Refactor: ID自動生成による設計改善

### 1.3 ITodoRepository Interface
- ✅ ITodoRepository.cs作成
- ✅ GetAllAsync()メソッド定義
- ✅ GetByIdAsync()メソッド定義
- ✅ SaveAsync()メソッド定義
- ✅ DeleteAsync()メソッド定義

## 2. App層の実装（TDD）

### 2.1 TodoUseCase の実装（アクター単位統合）
- ✅ TodoUseCaseTest.cs作成（全メソッドのテスト）
  - ✅ GetAllAsync_ShouldReturnAllTodos テスト
  - ✅ GetAllAsync_EmptyRepository_ShouldReturnEmptyList テスト
  - ✅ CreateAsync_ShouldCreateAndSaveTask テスト
  - ✅ ToggleCompleteAsync_ShouldToggleTaskCompletion テスト（メソッド名改善）
  - ✅ UpdateTitleAsync_ShouldUpdateTaskTitle テスト
  - ✅ UpdateDescriptionAsync_ShouldUpdateTaskDescription テスト
  - ✅ DeleteAsync_ShouldCallRepositoryDelete テスト
- ✅ TodoUseCase.cs実装（全メソッド統合）
  - ✅ GetAllAsync() - 全タスク取得
  - ✅ CreateAsync() - タスク作成
  - ✅ ToggleCompleteAsync() - 完了切り替え（メソッド名改善済み）
  - ✅ UpdateTitleAsync() - タイトル更新
  - ✅ UpdateDescriptionAsync() - 説明更新
  - ✅ DeleteAsync() - タスク削除

## 3. Infra層の実装（必要に応じてテスト）

### 3.1 CsvTodoRepository のテスト（TDD Red Phase）
- ✅ CsvTodoRepositoryTest.cs作成（10個のテストケース）
  - ✅ GetAllAsync_FileNotExists_ShouldReturnEmptyList テスト
  - ✅ GetAllAsync_EmptyFile_ShouldReturnEmptyList テスト
  - ✅ GetAllAsync_WithData_ShouldReturnTasks テスト
  - ✅ GetByIdAsync_ExistingTask_ShouldReturnTask テスト
  - ✅ GetByIdAsync_NonExistingTask_ShouldReturnNull テスト
  - ✅ SaveAsync_NewTask_ShouldAddToFile テスト
  - ✅ SaveAsync_ExistingTask_ShouldUpdateInFile テスト
  - ✅ DeleteAsync_ExistingTask_ShouldRemoveFromFile テスト
  - ✅ DeleteAsync_NonExistingTask_ShouldNotThrow テスト

### 3.2 CsvTodoRepository の実装（TDD Green Phase完了）
- ✅ CsvTodoRepository.cs作成（空実装 - TDD Red Phase）
- ✅ GetAllAsync() - CSV読み込み・パース機能実装
- ✅ GetByIdAsync() - ID別タスク取得機能実装
- ✅ SaveAsync() - 新規作成・更新機能実装
- ✅ DeleteAsync() - タスク削除機能実装
- ✅ WriteCsvFileAsync() - CSV書き込み機能実装
- ✅ ParseCsvContent() & ParseCsvLine() - CSVパース機能実装
- ✅ エラーハンドリング実装（例外安全設計）

## 4. Presentation層の実装（TDD + E2E）

### 4.1 TodoListViewModel の実装（TDD）
- ✅ **Complete**: TodoListViewModel.cs実装（TDD Green Phase完了）🎉
  - ✅ Initialize_ShouldSetDefaultValues テスト - 基本的な初期化完了
  - ✅ LoadTasks_ShouldUpdateTodosProperty テスト - タスク読み込み機能完了
  - ✅ CreateCommand_WithValidInput_ShouldCreateTask テスト - タスク作成機能完了
  - ✅ CreateCommand_WithEmptyTitle_ShouldNotExecute テスト - バリデーション完了
  - ✅ ToggleCompleteCommand_ShouldToggleTaskState テスト - 完了切り替え機能完了
  - ✅ DeleteCommand_ShouldRemoveTask テスト - タスク削除機能完了
  - ✅ Operations_ShouldManageLoadingState テスト - Loading状態管理完了
- ✅ TodoListViewModel.cs実装（TDD Green Phase 7/7完了）🎉
  - ✅ ReactiveProperty による状態管理
  - ✅ ReactiveCommand による操作実装
  - ✅ TodoUseCaseとの連携
  - ✅ CreateTodoAsync() - タスク作成機能（バリデーション付き）
  - ✅ ToggleCompleteAsync() - 完了切り替え機能（メソッド名改善済み）
  - ✅ DeleteTodoAsync() - タスク削除機能
  - ✅ LoadingState管理機能（遅延テスト付き）

### 4.2 その他のPresentation層コンポーネント
- ✅ TodoListPresenter実装完了🎉
  - ✅ VContainer EntryPointパターン（IStartable, IDisposable）
  - ✅ ViewModelとUseCaseの橋渡し機能
  - ✅ ReactiveCommandバインディング
  - ✅ CRUD操作の完全実装（Create, Toggle, Delete, Load）
  - ✅ エラーハンドリングとリソース管理
  - ✅ 自動データリロード機能
- ✅ TodoListView実装完了🎉
  - ✅ Unity UIコンポーネント統合（TMP_InputField, Button, Transform等）
  - ✅ VContainer依存性注入によるViewModel取得
  - ✅ R3双方向バインディング実装（ReactiveProperty ↔ UI）
  - ✅ 動的TodoItemView管理（生成・削除・クリーンアップ）
  - ✅ Loading状態表示、タスク数統計、デバッグ機能
- ✅ TodoItemView実装完了🎉
  - ✅ 個別Todoタスク表示・操作機能
  - ✅ 視覚的フィードバック（完了状態の色変更・打ち消し線）
  - ✅ リアクティブUIイベント処理（Toggle, Button）
  - ✅ 完了オーバーレイとカスタマイズ可能な視覚設定

## 5. DI設定の実装

- ⬜ TodoAppLifetimeScope実装
- ⬜ 依存関係の配線

## 6. UI構築

- ⬜ Scene設定
- ⬜ Prefab作成
- ⬜ UI配置

---

## 現在の状況
- **現在地**: 5. DI設定の実装準備（VContainer LifetimeScope）
- **次のタスク**: TodoAppLifetimeScope実装（依存関係配線）
- **TDDフェーズ**: Presentation層完全完了🎉 → DI設定 + Scene構築
- **実装済み**: Clean Architecture全層完成（Domain, App, Infra, Presentation: MVVM + UI）

## 凡例
- ✅ 完了
- ⏳ 進行中/次のタスク
- ⬜ 未実装