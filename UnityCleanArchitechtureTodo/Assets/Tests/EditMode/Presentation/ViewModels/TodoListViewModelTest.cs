using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Domain.Repositories;
using UnityCleanArchitectureTodo.App.UseCases;
using UnityCleanArchitectureTodo.Presentation.ViewModels;
using UnityEngine.TestTools;

namespace UnityCleanArchitectureTodo.Tests.Presentation.ViewModels
{
    [TestFixture]
    public class TodoListViewModelTest
    {
        private MockTodoRepository _mockRepository;
        private TodoUseCase _todoUseCase;
        private TodoListViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockTodoRepository();
            _todoUseCase = new TodoUseCase(_mockRepository);
            _viewModel = new TodoListViewModel(_todoUseCase);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        /// <summary>
        /// 初期化：デフォルト値が正しく設定されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator Initialize_ShouldSetDefaultValues() => UniTask.ToCoroutine(async () =>
        {
            // Assert - ReactivePropertyの初期値確認
            Assert.AreEqual(0, _viewModel.Todos.CurrentValue.Count, "初期状態でTodosは空のリストであるべき");
            Assert.IsFalse(_viewModel.IsLoading.CurrentValue, "初期状態でIsLoadingはfalseであるべき");
            Assert.AreEqual("", _viewModel.NewTodoTitle.Value, "初期状態でNewTodoTitleは空文字であるべき");
            Assert.AreEqual("", _viewModel.NewTodoDescription.Value, "初期状態でNewTodoDescriptionは空文字であるべき");
            
            // Commands の初期化確認
            Assert.IsNotNull(_viewModel.CreateTodoCommand, "CreateTodoCommandが初期化されているべき");
            Assert.IsNotNull(_viewModel.ToggleCompleteCommand, "ToggleCompleteCommandが初期化されているべき");
            Assert.IsNotNull(_viewModel.DeleteTodoCommand, "DeleteTodoCommandが初期化されているべき");
            Assert.IsNotNull(_viewModel.LoadTasksCommand, "LoadTasksCommandが初期化されているべき");
        });

        /// <summary>
        /// タスク読み込み：Todosプロパティが正しく更新されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator LoadTasks_ShouldUpdateTodosProperty() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - モックリポジトリに2つのタスクを設定
            var task1 = new TodoTask("タスク1", "説明1");
            var task2 = new TodoTask("タスク2", "説明2");
            var expectedTasks = new List<TodoTask> { task1, task2 };
            _mockRepository.SetTasks(expectedTasks);

            // Act - タスクを読み込み
            await _viewModel.LoadTasksAsync();

            // Assert - Todosプロパティが正しく更新されることを確認
            Assert.AreEqual(2, _viewModel.Todos.CurrentValue.Count, "読み込み後、Todosに2つのタスクが含まれているべき");
            Assert.AreEqual("タスク1", _viewModel.Todos.CurrentValue[0].Title, "1つ目のタスクのタイトルが正しく設定されているべき");
            Assert.AreEqual("タスク2", _viewModel.Todos.CurrentValue[1].Title, "2つ目のタスクのタイトルが正しく設定されているべき");
        });

        /// <summary>
        /// タスク作成（有効な入力）：新しいタスクが正しく作成され、入力がクリアされることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator CreateCommand_WithValidInput_ShouldCreateTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 有効な入力値を設定
            _viewModel.NewTodoTitle.Value = "新しいタスク";
            _viewModel.NewTodoDescription.Value = "新しい説明";

            // Act - タスク作成を実行
            await _viewModel.CreateTodoAsync();

            // Assert - UseCaseのCreateAsyncが呼ばれ、入力がクリアされることを確認
            Assert.IsTrue(_mockRepository.SaveAsyncCalled, "SaveAsyncが呼ばれているべき");
            Assert.AreEqual("", _viewModel.NewTodoTitle.Value, "作成後、NewTodoTitleがクリアされているべき");
            Assert.AreEqual("", _viewModel.NewTodoDescription.Value, "作成後、NewTodoDescriptionがクリアされているべき");
        });

        /// <summary>
        /// 完了切り替え：タスクの完了状態が正しく切り替わることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator ToggleCompleteCommand_ShouldToggleTaskState() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 未完了のタスクを準備
            var task = new TodoTask("テストタスク", "説明");
            _mockRepository.SetTasks(new List<TodoTask> { task });

            // Act - 完了状態を切り替え
            await _viewModel.ToggleCompleteAsync(task.Id);

            // Assert - タスクが完了状態になり、SaveAsyncが呼ばれることを確認
            Assert.IsTrue(task.IsCompleted, "タスクが完了状態になっているべき");
            Assert.IsTrue(_mockRepository.SaveAsyncCalled, "SaveAsyncが呼ばれているべき");
        });

        /// <summary>
        /// タスク削除：指定されたタスクが正しく削除されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator DeleteCommand_ShouldRemoveTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 削除対象のタスクを準備
            var task = new TodoTask("削除対象タスク", "説明");
            _mockRepository.SetTasks(new List<TodoTask> { task });

            // Act - タスクを削除
            await _viewModel.DeleteTodoAsync(task.Id);

            // Assert - DeleteAsyncが正しいIDで呼ばれることを確認
            Assert.IsTrue(_mockRepository.DeleteAsyncCalled, "DeleteAsyncが呼ばれているべき");
            Assert.AreEqual(task.Id, _mockRepository.LastDeletedId, "正しいIDでDeleteAsyncが呼ばれているべき");
        });

        /// <summary>
        /// タスク作成（無効な入力）：空のタイトルでは作成が実行されないことを確認
        /// </summary>
        [UnityTest]
        public IEnumerator CreateCommand_WithEmptyTitle_ShouldNotExecute() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のタイトルを設定
            _viewModel.NewTodoTitle.Value = "";
            _viewModel.NewTodoDescription.Value = "説明のみ";

            // Act - タスク作成を試行
            await _viewModel.CreateTodoAsync();

            // Assert - SaveAsyncが呼ばれないことを確認
            Assert.IsFalse(_mockRepository.SaveAsyncCalled, "空のタイトルの場合、SaveAsyncが呼ばれないべき");
        });

        /// <summary>
        /// ローディング状態管理：操作中にIsLoadingが適切に管理されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator Operations_ShouldManageLoadingState() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 初期状態の確認
            Assert.IsFalse(_viewModel.IsLoading.CurrentValue, "初期状態でIsLoadingはfalseであるべき");

            // Arrange - 遅延を設定してLoading状態を確認しやすくする
            _mockRepository.DelayMilliseconds = 100; // 100ms遅延

            // Act - 非同期操作を開始（待機しない）
            var loadTask = _viewModel.LoadTasksAsync();
            
            // Assert - 操作開始直後はIsLoadingがtrueになることを確認
            Assert.IsTrue(_viewModel.IsLoading.CurrentValue, "操作中はIsLoadingがtrueになるべき");

            // Act - 操作完了まで待機
            await loadTask;

            // Assert - 操作完了後はIsLoadingがfalseに戻ることを確認
            Assert.IsFalse(_viewModel.IsLoading.CurrentValue, "操作完了後はIsLoadingがfalseになるべき");
            
            // 遅延をリセットして複数回の操作をテスト
            _mockRepository.DelayMilliseconds = 0;
            await _viewModel.LoadTasksAsync();
            Assert.IsFalse(_viewModel.IsLoading.CurrentValue, "複数回実行後もIsLoadingがfalseになるべき");
        });
    }

    /// <summary>
    /// テスト用のモックリポジトリ（既存のものを流用）
    /// </summary>
    public class MockTodoRepository : ITodoRepository
    {
        private List<TodoTask> _tasks = new List<TodoTask>();
        public bool SaveAsyncCalled { get; set; }
        public bool DeleteAsyncCalled { get; set; }
        public string LastDeletedId { get; set; }
        
        // 遅延設定用プロパティ
        public int DelayMilliseconds { get; set; } = 0;

        public void SetTasks(List<TodoTask> tasks)
        {
            _tasks = tasks;
        }

        public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        {
            if (DelayMilliseconds > 0)
            {
                await UniTask.Delay(DelayMilliseconds);
            }
            return _tasks.AsReadOnly();
        }

        public UniTask<TodoTask> GetByIdAsync(string id)
        {
            var task = _tasks.Find(t => t.Id == id);
            return UniTask.FromResult(task);
        }

        public UniTask SaveAsync(TodoTask task)
        {
            SaveAsyncCalled = true;
            var existing = _tasks.Find(t => t.Id == task.Id);
            if (existing != null)
            {
                _tasks.Remove(existing);
            }
            _tasks.Add(task);
            return UniTask.CompletedTask;
        }

        public UniTask DeleteAsync(string id)
        {
            DeleteAsyncCalled = true;
            LastDeletedId = id;
            _tasks.RemoveAll(t => t.Id == id);
            return UniTask.CompletedTask;
        }
    }
}
