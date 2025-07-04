using System;

namespace UnityCleanArchitectureTodo.Domain.Entities
{
    /// <summary>
    /// TodoTask Entity
    /// ID自動生成によるドメインオブジェクトの責務向上
    /// </summary>
    public class TodoTask
    {
        private readonly TimeProvider _timeProvider;

        public string Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime CreatedAt { get; }
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// TodoTaskのコンストラクタ（新規作成用）
        /// TDD Refactor Phase: ID自動生成による改善
        /// </summary>
        /// <param name="title">タスクタイトル</param>
        /// <param name="description">タスク説明</param>
        /// <param name="timeProvider">時刻プロバイダー（テスト可能性のため）</param>
        public TodoTask(string title, string description, TimeProvider timeProvider = null)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("タイトルは必須です", nameof(title));

            // TimeProvider設定（nullの場合はデフォルト）
            _timeProvider = timeProvider ?? TimeProvider.System;

            // プロパティ設定
            Id = GenerateNewId(); // ID自動生成
            Title = title;
            Description = description ?? "";
            IsCompleted = false;
            CreatedAt = _timeProvider.GetUtcNow().DateTime;
            CompletedAt = null;
        }

        /// <summary>
        /// TodoTaskのコンストラクタ（復元用）
        /// CSVファイルや外部データソースからの復元時に使用
        /// </summary>
        /// <param name="id">タスクID</param>
        /// <param name="title">タスクタイトル</param>
        /// <param name="description">タスク説明</param>
        /// <param name="isCompleted">完了状態</param>
        /// <param name="createdAt">作成日時（nullの場合は現在時刻）</param>
        /// <param name="completedAt">完了日時</param>
        /// <param name="timeProvider">時刻プロバイダー（テスト可能性のため）</param>
        public TodoTask(string id, string title, string description, bool isCompleted,
            DateTime? createdAt = null, DateTime? completedAt = null, TimeProvider timeProvider = null)
        {
            // バリデーション
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("タイトルは必須です", nameof(title));
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("IDは必須です", nameof(id));

            // TimeProvider設定（nullの場合はデフォルト）
            _timeProvider = timeProvider ?? TimeProvider.System;

            // プロパティ設定
            Id = id;
            Title = title;
            Description = description ?? "";
            IsCompleted = isCompleted;
            CreatedAt = createdAt ?? _timeProvider.GetUtcNow().DateTime;
            CompletedAt = completedAt;
        }

        /// <summary>
        /// タスクを完了状態にする
        /// TDD Green Phase: Complete_ShouldMarkAsCompletedテストを通すための実装
        /// </summary>
        public void Complete()
        {
            if (!IsCompleted)
            {
                IsCompleted = true;
                CompletedAt = _timeProvider.GetUtcNow().DateTime;
            }
        }

        /// <summary>
        /// タスクを未完了状態に戻す
        /// TDD Green Phase: Uncomplete_ShouldMarkAsIncompleteテストを通すための実装
        /// </summary>
        public void Uncomplete()
        {
            if (IsCompleted)
            {
                IsCompleted = false;
                CompletedAt = null;
            }
        }

        /// <summary>
        /// タスクのタイトルを更新する
        /// TDD Green Phase: UpdateTitle関連テストを通すための実装
        /// </summary>
        /// <param name="title">新しいタイトル</param>
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("タイトルは必須です", nameof(title));

            Title = title;
        }

        /// <summary>
        /// タスクの説明を更新する
        /// TDD Green Phase: UpdateDescription_ShouldChangeDescriptionテストを通すための実装
        /// </summary>
        /// <param name="description">新しい説明</param>
        public void UpdateDescription(string description)
        {
            Description = description ?? "";
        }

        /// <summary>
        /// 新しいIDを生成する
        /// TDD Green Phase: GenerateNewId_ShouldReturnNonEmptyStringテストを通すための実装
        /// </summary>
        /// <returns>生成されたID</returns>
        public static string GenerateNewId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}