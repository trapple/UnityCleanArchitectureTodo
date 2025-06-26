using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Infra.Repositories;
using UnityCleanArchitectureTodo.Tests.TestUtils;
using UnityEngine.TestTools;

namespace UnityCleanArchitectureTodo.Tests.Infra.Repositories
{
    [TestFixture]
    public class CsvTodoRepositoryTest
    {
        private CsvTodoRepository _repository;
        private string _testFilePath;
        private MockTimeProvider _mockTimeProvider;

        [SetUp]
        public void SetUp()
        {
            // テスト用の一意なファイルパスを生成
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_todos_{Guid.NewGuid()}.csv");
            _mockTimeProvider = new MockTimeProvider(new DateTime(2024, 1, 1, 12, 0, 0));
            _repository = new CsvTodoRepository(_testFilePath);
        }

        [TearDown]
        public void TearDown()
        {
            // テスト後にファイルを削除
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        /// <summary>
        /// 全タスク取得（空ファイル）：ファイルが存在しない場合、空のリストが返されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetAllAsync_FileNotExists_ShouldReturnEmptyList() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - ファイルが存在しないことを確認
            Assert.IsFalse(File.Exists(_testFilePath));

            // Act - 全タスクを取得
            var result = await _repository.GetAllAsync();

            // Assert - 空のリストが返されることを確認
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        });

        /// <summary>
        /// 全タスク取得（空ファイル）：空のCSVファイルが存在する場合、空のリストが返されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetAllAsync_EmptyFile_ShouldReturnEmptyList() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のCSVファイルを作成
            await File.WriteAllTextAsync(_testFilePath, "");

            // Act - 全タスクを取得
            var result = await _repository.GetAllAsync();

            // Assert - 空のリストが返されることを確認
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        });

        /// <summary>
        /// 全タスク取得（データあり）：CSVファイルに保存されたタスクデータが正しく読み込まれることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetAllAsync_WithData_ShouldReturnTasks() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - CSVファイルにテストデータを書き込み
            var csvContent = @"Id,Title,Description,IsCompleted,CreatedAt,CompletedAt
test-id-1,タスク1,説明1,False,2024-01-01T12:00:00.0000000Z,
test-id-2,タスク2,説明2,True,2024-01-01T13:00:00.0000000Z,2024-01-01T14:00:00.0000000Z";
            await File.WriteAllTextAsync(_testFilePath, csvContent);

            // Act - 全タスクを取得
            var result = await _repository.GetAllAsync();

            // Assert - 正しくタスクが読み込まれることを確認
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("test-id-1", result[0].Id);
            Assert.AreEqual("タスク1", result[0].Title);
            Assert.AreEqual("説明1", result[0].Description);
            Assert.IsFalse(result[0].IsCompleted);
            Assert.AreEqual("test-id-2", result[1].Id);
            Assert.AreEqual("タスク2", result[1].Title);
            Assert.IsTrue(result[1].IsCompleted);
        });

        /// <summary>
        /// ID別タスク取得（存在する）：指定したIDのタスクが正しく取得されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetByIdAsync_ExistingTask_ShouldReturnTask() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - CSVファイルにテストデータを書き込み
            var csvContent = @"Id,Title,Description,IsCompleted,CreatedAt,CompletedAt
test-id-1,タスク1,説明1,False,2024-01-01T12:00:00.0000000Z,";
            await File.WriteAllTextAsync(_testFilePath, csvContent);

            // Act - 特定のIDでタスクを取得
            var result = await _repository.GetByIdAsync("test-id-1");

            // Assert - 正しいタスクが返されることを確認
            Assert.IsNotNull(result);
            Assert.AreEqual("test-id-1", result.Id);
            Assert.AreEqual("タスク1", result.Title);
        });

        /// <summary>
        /// ID別タスク取得（存在しない）：存在しないIDの場合、nullが返されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator GetByIdAsync_NonExistingTask_ShouldReturnNull() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のCSVファイルを作成
            await File.WriteAllTextAsync(_testFilePath, "");

            // Act - 存在しないIDでタスクを取得
            var result = await _repository.GetByIdAsync("non-existing-id");

            // Assert - nullが返されることを確認
            Assert.IsNull(result);
        });

        /// <summary>
        /// タスク保存（新規）：新しいタスクがCSVファイルに正しく保存されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator SaveAsync_NewTask_ShouldAddToFile() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 新しいタスクを作成
            var task = new TodoTask("新規タスク", "新規説明", _mockTimeProvider);

            // Act - タスクを保存
            await _repository.SaveAsync(task);

            // Assert - ファイルが作成され、タスクが保存されることを確認
            Assert.IsTrue(File.Exists(_testFilePath));
            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.IsTrue(fileContent.Contains("新規タスク"));
            Assert.IsTrue(fileContent.Contains("新規説明"));
        });

        /// <summary>
        /// タスク保存（更新）：既存タスクが正しく更新されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator SaveAsync_ExistingTask_ShouldUpdateInFile() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 既存タスクをファイルに保存
            var originalTask = new TodoTask("元のタスク", "元の説明", _mockTimeProvider);
            await _repository.SaveAsync(originalTask);

            // タスクを更新
            originalTask.UpdateTitle("更新されたタスク");
            originalTask.Complete();

            // Act - 更新されたタスクを保存
            await _repository.SaveAsync(originalTask);

            // Assert - ファイル内のタスクが更新されていることを確認
            var result = await _repository.GetByIdAsync(originalTask.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("更新されたタスク", result.Title);
            Assert.IsTrue(result.IsCompleted);
        });

        /// <summary>
        /// タスク削除：指定したIDのタスクがCSVファイルから正しく削除されることを確認
        /// </summary>
        [UnityTest]
        public IEnumerator DeleteAsync_ExistingTask_ShouldRemoveFromFile() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 複数のタスクをファイルに保存
            var task1 = new TodoTask("タスク1", "説明1", _mockTimeProvider);
            var task2 = new TodoTask("タスク2", "説明2", _mockTimeProvider);
            await _repository.SaveAsync(task1);
            await _repository.SaveAsync(task2);

            // Act - 1つのタスクを削除
            await _repository.DeleteAsync(task1.Id);

            // Assert - 削除されたタスクが存在せず、他のタスクは残っていることを確認
            var deletedTask = await _repository.GetByIdAsync(task1.Id);
            var remainingTask = await _repository.GetByIdAsync(task2.Id);
            Assert.IsNull(deletedTask);
            Assert.IsNotNull(remainingTask);
        });

        /// <summary>
        /// タスク削除（存在しない）：存在しないIDの削除が例外を発生させず、正常終了することを確認
        /// </summary>
        [UnityTest]
        public IEnumerator DeleteAsync_NonExistingTask_ShouldNotThrow() => UniTask.ToCoroutine(async () =>
        {
            // Arrange - 空のファイル状態

            // Act & Assert - 存在しないIDの削除で例外が発生しないことを確認
            try
            {
                await _repository.DeleteAsync("non-existing-id");
                // 例外が発生しなければテスト成功
            }
            catch (Exception ex)
            {
                Assert.Fail($"例外が発生しました: {ex.Message}");
            }
        });
    }
}
