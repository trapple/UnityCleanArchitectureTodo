using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Domain.Repositories;
using UnityCleanArchitectureTodo.App.UseCases;
using UnityEngine.TestTools;

namespace UnityCleanArchitectureTodo.Tests.App.UseCases
{
    [TestFixture]
    public class TodoUseCaseTest
    {
        private MockTodoRepository _mockRepository;
        private TodoUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockTodoRepository();
            _useCase = new TodoUseCase(_mockRepository);
        }

        /// <summary>
        /// 全タスク取得：リポジトリに複数のタスクが存在する場合、すべてのタスクを正しく取得できることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetAllAsync_ShouldReturnAllTodos() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 2つのタスクをモックリポジトリに設定
            var task1 = new TodoTask("タスク1", "説明1");
            var task2 = new TodoTask("タスク2", "説明2");
            var expectedTasks = new List<TodoTask> { task1, task2 };
            _mockRepository.SetTasks(expectedTasks);

            // Act - 全タスクを取得
            var result = await _useCase.GetAllAsync();

            // Assert - 期待される数とタイトルが正しく取得されることを確認
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("タスク1", result[0].Title);
            Assert.AreEqual("タスク2", result[1].Title);
        });

        /// <summary>
        /// 全タスク取得（空リスト）：リポジトリが空の場合、空のリストが正しく返されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetAllAsync_EmptyRepository_ShouldReturnEmptyList() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のタスクリストをモックリポジトリに設定
            _mockRepository.SetTasks(new List<TodoTask>());

            // Act - 全タスクを取得
            var result = await _useCase.GetAllAsync();

            // Assert - 空のリストが返されることを確認
            Assert.AreEqual(0, result.Count);
        });

        /// <summary>
        /// タスク作成：指定されたタイトルと説明で新しいタスクが正しく作成され、リポジトリに保存されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator CreateAsync_ShouldCreateAndSaveTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 作成するタスクの情報を準備
            var title = "新しいタスク";
            var description = "テスト用の説明";

            // Act - 新しいタスクを作成
            var result = await _useCase.CreateAsync(title, description);

            // Assert - タスクが正しく作成され、初期状態が未完了で、リポジトリに保存されることを確認
            Assert.IsNotNull(result);
            Assert.AreEqual(title, result.Title);
            Assert.AreEqual(description, result.Description);
            Assert.IsFalse(result.IsCompleted);
            Assert.IsTrue(_mockRepository.SaveAsyncCalled);
        });

        /// <summary>
        /// タスク完了切り替え：タスクの完了状態を未完了→完了→未完了と正しく切り替えができることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator ToggleCompleteAsync_ShouldToggleTaskCompletion() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 未完了状態のタスクを準備
            var task = new TodoTask("テストタスク", "説明");
            _mockRepository.SetTasks(new List<TodoTask> { task });

            // Act & Assert - タスクを完了状態にする
            await _useCase.ToggleCompleteAsync(task.Id);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(_mockRepository.SaveAsyncCalled);

            // Act & Assert - タスクを未完了状態に戻す
            _mockRepository.SaveAsyncCalled = false; // フラグをリセット
            await _useCase.ToggleCompleteAsync(task.Id);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsTrue(_mockRepository.SaveAsyncCalled);
        });

        /// <summary>
        /// タスク完了切り替え（存在しないタスク）：存在しないタスクIDに対して例外が発生せず、処理が正常終了することを確認
        /// </summary>
        [UnityTest]
        public IEnumerator ToggleCompleteAsync_TaskNotFound_ShouldNotThrow() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のタスクリストを設定（存在しないタスクIDをテストするため）
            _mockRepository.SetTasks(new List<TodoTask>());

            // Act & Assert - 存在しないIDに対して例外が発生しないことを確認
            try
            {
                await _useCase.ToggleCompleteAsync("存在しないID");
                // 例外が発生しなければテスト成功
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"例外が発生しました: {ex.Message}");
            }
        });

        /// <summary>
        /// タスクタイトル更新：既存タスクのタイトルが正しく更新され、リポジトリに保存されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateTitleAsync_ShouldUpdateTaskTitle() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 元のタイトルを持つタスクを準備
            var task = new TodoTask("元のタイトル", "説明");
            _mockRepository.SetTasks(new List<TodoTask> { task });
            var newTitle = "更新されたタイトル";

            // Act - タスクのタイトルを更新
            await _useCase.UpdateTitleAsync(task.Id, newTitle);

            // Assert - タイトルが正しく更新され、リポジトリに保存されることを確認
            Assert.AreEqual(newTitle, task.Title);
            Assert.IsTrue(_mockRepository.SaveAsyncCalled);
        });

        /// <summary>
        /// タスク説明更新：既存タスクの説明が正しく更新され、リポジトリに保存されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateDescriptionAsync_ShouldUpdateTaskDescription() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 元の説明を持つタスクを準備
            var task = new TodoTask("タイトル", "元の説明");
            _mockRepository.SetTasks(new List<TodoTask> { task });
            var newDescription = "更新された説明";

            // Act - タスクの説明を更新
            await _useCase.UpdateDescriptionAsync(task.Id, newDescription);

            // Assert - 説明が正しく更新され、リポジトリに保存されることを確認
            Assert.AreEqual(newDescription, task.Description);
            Assert.IsTrue(_mockRepository.SaveAsyncCalled);
        });

        /// <summary>
        /// タスク削除：指定されたIDのタスクがリポジトリから正しく削除されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator DeleteAsync_ShouldCallRepositoryDelete() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 削除対象のタスクIDを準備
            var taskId = "test-id";

            // Act - タスクを削除
            await _useCase.DeleteAsync(taskId);

            // Assert - リポジトリの削除メソッドが正しいIDで呼び出されることを確認
            Assert.IsTrue(_mockRepository.DeleteAsyncCalled);
            Assert.AreEqual(taskId, _mockRepository.LastDeletedId);
        });
    }

    // テスト用のモックリポジトリ
    public class MockTodoRepository : ITodoRepository
    {
        private List<TodoTask> _tasks = new List<TodoTask>();
        public bool SaveAsyncCalled { get; set; }
        public bool DeleteAsyncCalled { get; set; }
        public string LastDeletedId { get; set; }

        public void SetTasks(List<TodoTask> tasks)
        {
            _tasks = tasks;
        }

        public UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        {
            return UniTask.FromResult((IReadOnlyList<TodoTask>)_tasks.AsReadOnly());
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