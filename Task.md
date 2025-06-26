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
- ⏳ **Next**: UpdateTitle()メソッド実装
- ⬜ UpdateDescription()メソッド実装
- ⬜ GenerateNewId()静的メソッド実装
- ⬜ Refactor: コードの改善

### 1.3 ITodoRepository Interface
- ⬜ ITodoRepository.cs作成
- ⬜ GetAllAsync()メソッド定義
- ⬜ GetByIdAsync()メソッド定義
- ⬜ SaveAsync()メソッド定義
- ⬜ DeleteAsync()メソッド定義

## 2. App層の実装（TDD）

### 2.1 GetAllTodosUseCase のテスト
- ⬜ GetAllTodosUseCaseTest.cs作成
- ⬜ ExecuteAsync_ShouldReturnAllTodos テスト

### 2.2 各UseCase のテストと実装
- ⬜ CreateTodoUseCaseTest.cs作成・実装
- ⬜ CompleteTodoUseCaseTest.cs作成・実装
- ⬜ DeleteTodoUseCaseTest.cs作成・実装
- ⬜ UpdateTodoUseCaseTest.cs作成・実装

## 3. Infra層の実装（必要に応じてテスト）

### 3.1 CsvTodoRepository のテスト
- ⬜ CsvTodoRepositoryTest.cs作成
- ⬜ SaveAsync_NewTask_ShouldAddToFile テスト
- ⬜ GetAllAsync_EmptyFile_ShouldReturnEmptyList テスト
- ⬜ その他のRepository操作テスト

### 3.2 CsvTodoRepository の実装
- ⬜ CsvTodoRepository.cs作成
- ⬜ CSVファイル操作ロジック実装
- ⬜ エラーハンドリング実装

## 4. Presentation層の実装（UnitTestなし）

- ⬜ TodoListViewModel実装
- ⬜ TodoListPresenter実装
- ⬜ TodoListView実装
- ⬜ TodoItemView実装

## 5. DI設定の実装

- ⬜ TodoAppLifetimeScope実装
- ⬜ 依存関係の配線

## 6. UI構築

- ⬜ Scene設定
- ⬜ Prefab作成
- ⬜ UI配置

---

## 現在の状況
- **現在地**: 1.2 TodoTask Entity の実装（TDD Green Phase）
- **次のタスク**: UpdateTitle()メソッドの実装でUpdateTitle_ShouldChangeTitleテストを通す
- **TDDフェーズ**: Green Phase継続中（段階的実装）

## 凡例
- ✅ 完了
- ⏳ 進行中/次のタスク
- ⬜ 未実装