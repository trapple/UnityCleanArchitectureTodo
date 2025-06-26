using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Domain.Repositories;

namespace UnityCleanArchitectureTodo.App.UseCases
{
    /// <summary>
    /// Todoタスクに関するすべてのユースケースを提供するクラス
    /// アクター: 一般ユーザー
    /// </summary>
    public class TodoUseCase
    {
        private readonly ITodoRepository _repository;

        public TodoUseCase(ITodoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 全てのTodoタスクを取得する
        /// </summary>
        public UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        /// <summary>
        /// 新しいTodoタスクを作成する
        /// </summary>
        /// <param name="title">タスクのタイトル</param>
        /// <param name="description">タスクの説明</param>
        public async UniTask<TodoTask> CreateAsync(string title, string description = "")
        {
            var task = new TodoTask(title, description);
            await _repository.SaveAsync(task);
            return task;
        }

        /// <summary>
        /// Todoタスクの完了状態を切り替える
        /// </summary>
        /// <param name="taskId">対象タスクのID</param>
        public async UniTask ToggleCompleteAsync(string taskId)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null) return;

            if (task.IsCompleted)
                task.Uncomplete();
            else
                task.Complete();

            await _repository.SaveAsync(task);
        }

        /// <summary>
        /// Todoタスクのタイトルを更新する
        /// </summary>
        /// <param name="taskId">対象タスクのID</param>
        /// <param name="newTitle">新しいタイトル</param>
        public async UniTask UpdateTitleAsync(string taskId, string newTitle)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null) return;

            task.UpdateTitle(newTitle);
            await _repository.SaveAsync(task);
        }

        /// <summary>
        /// Todoタスクの説明を更新する
        /// </summary>
        /// <param name="taskId">対象タスクのID</param>
        /// <param name="newDescription">新しい説明</param>
        public async UniTask UpdateDescriptionAsync(string taskId, string newDescription)
        {
            var task = await _repository.GetByIdAsync(taskId);
            if (task == null) return;

            task.UpdateDescription(newDescription);
            await _repository.SaveAsync(task);
        }

        /// <summary>
        /// Todoタスクを削除する
        /// </summary>
        /// <param name="taskId">削除対象タスクのID</param>
        public UniTask DeleteAsync(string taskId)
        {
            return _repository.DeleteAsync(taskId);
        }
    }
}