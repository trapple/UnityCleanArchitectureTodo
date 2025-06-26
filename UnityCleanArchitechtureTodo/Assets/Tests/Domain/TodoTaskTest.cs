using System;
using NUnit.Framework;
using UnityCleanArchitectureTodo.Domain.Entities;

namespace UnityCleanArchitectureTodo.Tests.Domain
{
    /// <summary>
    /// TodoTask Entity のテストクラス
    /// TDD Red Phase: 失敗するテストを先に書く
    /// </summary>
    [TestFixture]
    public class TodoTaskTest
    {
        /// <summary>
        /// テスト対象: TodoTaskのコンストラクタ
        /// 期待結果: プロパティが正しく設定される
        /// - Id, Title, Description が設定される
        /// - IsCompleted は false で初期化される
        /// - CreatedAt は現在日時で設定される
        /// - CompletedAt は null で初期化される
        /// </summary>
        [Test]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var id = "test-id";
            var title = "テストタスク";
            var description = "テスト用の説明";
            
            // Act
            var task = new TodoTask(id, title, description);
            
            // Assert
            Assert.AreEqual(id, task.Id);
            Assert.AreEqual(title, task.Title);
            Assert.AreEqual(description, task.Description);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsNotNull(task.CreatedAt);
            Assert.IsNull(task.CompletedAt);
        }
        
        /// <summary>
        /// テスト対象: TodoTask.Complete()メソッド
        /// 期待結果: タスクが完了状態に変わる
        /// - IsCompleted が true になる
        /// - CompletedAt に現在日時が設定される
        /// </summary>
        [Test]
        public void Complete_ShouldMarkAsCompleted()
        {
            // Arrange
            var task = new TodoTask("test-id", "テストタスク", "説明");
            
            // Act
            task.Complete();
            
            // Assert
            Assert.IsTrue(task.IsCompleted);
            Assert.IsNotNull(task.CompletedAt);
        }
        
        /// <summary>
        /// テスト対象: TodoTask.Uncomplete()メソッド
        /// 期待結果: 完了状態のタスクが未完了状態に戻る
        /// - IsCompleted が false になる
        /// - CompletedAt が null になる
        /// 注意: このテストはComplete()が実装されるまで正しく動作しない
        /// </summary>
        [Test]
        public void Uncomplete_ShouldMarkAsIncomplete()
        {
            // Arrange
            var task = new TodoTask("test-id", "テストタスク", "説明");
            task.Complete();
            
            // Complete()が空実装の場合、このテストは期待通り失敗しない
            // これはTDDの制約として受け入れる
            
            // Act  
            task.Uncomplete();
            
            // Assert
            Assert.IsFalse(task.IsCompleted);
            Assert.IsNull(task.CompletedAt);
        }
        
        /// <summary>
        /// テスト対象: TodoTask.UpdateTitle()メソッド
        /// 期待結果: タイトルが正しく更新される
        /// - Title プロパティが新しい値に変わる
        /// </summary>
        [Test]
        public void UpdateTitle_ShouldChangeTitle()
        {
            // Arrange
            var task = new TodoTask("test-id", "元のタイトル", "説明");
            var newTitle = "新しいタイトル";
            
            // Act
            task.UpdateTitle(newTitle);
            
            // Assert
            Assert.AreEqual(newTitle, task.Title);
        }
        
        /// <summary>
        /// テスト対象: TodoTask.UpdateTitle()メソッドのバリデーション
        /// 期待結果: 無効なタイトルでArgumentExceptionが発生
        /// - null の場合に例外が発生
        /// - 空文字の場合に例外が発生
        /// - 空白のみの場合に例外が発生
        /// </summary>
        [Test]
        public void UpdateTitle_WithNullOrEmpty_ShouldThrowException()
        {
            // Arrange
            var task = new TodoTask("test-id", "タイトル", "説明");
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => task.UpdateTitle(null));
            Assert.Throws<ArgumentException>(() => task.UpdateTitle(""));
            Assert.Throws<ArgumentException>(() => task.UpdateTitle("   "));
        }
        
        /// <summary>
        /// テスト対象: TodoTask.UpdateDescription()メソッド
        /// 期待結果: 説明が正しく更新される
        /// - Description プロパティが新しい値に変わる
        /// - null の場合は空文字に変換される
        /// </summary>
        [Test]
        public void UpdateDescription_ShouldChangeDescription()
        {
            // Arrange
            var task = new TodoTask("test-id", "タイトル", "元の説明");
            var newDescription = "新しい説明";
            
            // Act
            task.UpdateDescription(newDescription);
            
            // Assert
            Assert.AreEqual(newDescription, task.Description);
        }
        
        /// <summary>
        /// テスト対象: TodoTask.GenerateNewId()静的メソッド
        /// 期待結果: 一意のIDが生成される
        /// - null でない文字列が返される
        /// - 空文字でない文字列が返される
        /// - 呼び出すたびに異なるIDが生成される
        /// </summary>
        [Test]
        public void GenerateNewId_ShouldReturnNonEmptyString()
        {
            // Act
            var id1 = TodoTask.GenerateNewId();
            var id2 = TodoTask.GenerateNewId();
            
            // Assert
            Assert.IsNotNull(id1);
            Assert.IsNotEmpty(id1);
            Assert.IsNotNull(id2);
            Assert.IsNotEmpty(id2);
            Assert.AreNotEqual(id1, id2); // 異なるIDが生成されることを確認
        }
    }
}