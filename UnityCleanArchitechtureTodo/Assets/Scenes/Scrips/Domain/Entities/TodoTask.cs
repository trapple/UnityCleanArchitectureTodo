using System;

namespace UnityCleanArchitectureTodo.Domain.Entities
{
    /// <summary>
    /// TodoTask Entity
    /// TDD Green Phase: コンストラクタのテストを通すための最小実装
    /// </summary>
    public class TodoTask
    {
        public string Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? CompletedAt { get; private set; }
        
        /// <summary>
        /// TodoTaskのコンストラクタ
        /// TDD Green Phase: Constructor_ShouldSetPropertiesCorrectlyテストを通すための実装
        /// </summary>
        /// <param name="id">タスクID</param>
        /// <param name="title">タスクタイトル</param>
        /// <param name="description">タスク説明</param>
        public TodoTask(string id, string title, string description)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("IDは必須です", nameof(id));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("タイトルは必須です", nameof(title));
            
            // プロパティ設定
            Id = id;
            Title = title;
            Description = description ?? "";
            IsCompleted = false;
            CreatedAt = DateTime.Now;
            CompletedAt = null;
        }
        
        /// <summary>
        /// タスクを完了状態にする
        /// TDD: 空の実装（テストが失敗することを確認するため）
        /// </summary>
        public void Complete()
        {
            // まだ実装しない
        }
        
        /// <summary>
        /// タスクを未完了状態に戻す
        /// TDD: 空の実装（テストが失敗することを確認するため）
        /// </summary>
        public void Uncomplete()
        {
            // まだ実装しない
        }
        
        /// <summary>
        /// タスクのタイトルを更新する
        /// TDD: 空の実装（テストが失敗することを確認するため）
        /// </summary>
        /// <param name="title">新しいタイトル</param>
        public void UpdateTitle(string title)
        {
            // まだ実装しない
        }
        
        /// <summary>
        /// タスクの説明を更新する
        /// TDD: 空の実装（テストが失敗することを確認するため）
        /// </summary>
        /// <param name="description">新しい説明</param>
        public void UpdateDescription(string description)
        {
            // まだ実装しない
        }
        
        /// <summary>
        /// 新しいIDを生成する
        /// TDD: 空の実装（テストが失敗することを確認するため）
        /// </summary>
        /// <returns>生成されたID</returns>
        public static string GenerateNewId()
        {
            // まだ実装しない
            return null;
        }
    }
}