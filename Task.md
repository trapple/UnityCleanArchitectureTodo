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

### 3.2 CsvTodoRepository の実装（TDD Green Phase完了）
- ✅ CsvTodoRepository.cs作成（空実装 - TDD Red Phase）
- ✅ GetAllAsync() - CSV読み込み・パース機能実装
- ✅ GetByIdAsync() - ID別タスク取得機能実装
- ✅ SaveAsync() - 新規作成・更新機能実装
- ✅ DeleteAsync() - タスク削除機能実装
- ✅ WriteCsvFileAsync() - CSV書き込み機能実装
- ✅ ParseCsvContent() & ParseCsvLine() - CSVパース機能実装
- ✅ エラーハンドリング実装（例外安全設計）

## 4. Presentation層の実装（E2Eテスト）

- ⏳ **Next**: TodoListViewModel実装
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
- **現在地**: 3.2 CsvTodoRepository の完全実装（TDD Green Phase完了）🎉
- **次のタスク**: 4.1 TodoListViewModel の実装（Presentation層開始）
- **TDDフェーズ**: Infra層完了 → Presentation層MVVM実装開始
- **実装済み**: Domain層、App層、Infra層の完全実装（全テスト成功）

## 凡例
- ✅ 完了
- ⏳ 進行中/次のタスク
- ⬜ 未実装