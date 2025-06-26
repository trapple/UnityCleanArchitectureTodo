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
  - ✅ CompleteAsync_ShouldToggleTaskCompletion テスト
  - ✅ UpdateTitleAsync_ShouldUpdateTaskTitle テスト
  - ✅ UpdateDescriptionAsync_ShouldUpdateTaskDescription テスト
  - ✅ DeleteAsync_ShouldCallRepositoryDelete テスト
- ✅ TodoUseCase.cs実装（全メソッド統合）
  - ✅ GetAllAsync() - 全タスク取得
  - ✅ CreateAsync() - タスク作成
  - ✅ CompleteAsync() - 完了切り替え
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

### 3.2 CsvTodoRepository の実装
- ✅ CsvTodoRepository.cs作成（空実装 - TDD Red Phase）
- ⏳ **Next**: CSVファイル操作ロジック実装（TDD Green Phase）
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
- **現在地**: 3.1 CsvTodoRepository のテスト作成・空実装（TDD Red Phase完了）
- **次のタスク**: 3.2 CsvTodoRepository のCSVファイル操作ロジック実装（TDD Green Phase）
- **TDDフェーズ**: Infra層TDD継続中（Red → Green移行）

## 凡例
- ✅ 完了
- ⏳ 進行中/次のタスク
- ⬜ 未実装