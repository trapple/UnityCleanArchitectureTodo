using System;
using System.Collections.Generic;
using R3;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.App.UseCases;

namespace UnityCleanArchitectureTodo.Presentation.ViewModels
{
    /// <summary>
    /// TodoリストのViewModel
    /// R3のReactivePropertyとReactiveCommandを使用してMVVMパターンを実装
    /// </summary>
    public class TodoListViewModel : IDisposable
    {
        private readonly TodoUseCase _todoUseCase;
        private readonly CompositeDisposable _disposables = new();

        // Observable Properties
        private readonly ReactiveProperty<IReadOnlyList<TodoTask>> _todos = new(new List<TodoTask>());
        private readonly ReactiveProperty<bool> _isLoading = new(false);
        private readonly ReactiveProperty<string> _newTodoTitle = new("");
        private readonly ReactiveProperty<string> _newTodoDescription = new("");

        public ReadOnlyReactiveProperty<IReadOnlyList<TodoTask>> Todos => _todos.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> IsLoading => _isLoading.ToReadOnlyReactiveProperty();

        public ReactiveProperty<string> NewTodoTitle => _newTodoTitle;
        public ReactiveProperty<string> NewTodoDescription => _newTodoDescription;

        // Commands
        public ReactiveCommand CreateTodoCommand { get; }
        public ReactiveCommand<string> ToggleCompleteCommand { get; }
        public ReactiveCommand<string> DeleteTodoCommand { get; }
        public ReactiveCommand LoadTasksCommand { get; }

        public TodoListViewModel(TodoUseCase todoUseCase)
        {
            _todoUseCase = todoUseCase;

            // Commands初期化（空実装）
            CreateTodoCommand = new ReactiveCommand();
            ToggleCompleteCommand = new ReactiveCommand<string>();
            DeleteTodoCommand = new ReactiveCommand<string>();
            LoadTasksCommand = new ReactiveCommand();

            // Disposablesに追加
            _todos.AddTo(_disposables);
            _isLoading.AddTo(_disposables);
            _newTodoTitle.AddTo(_disposables);
            _newTodoDescription.AddTo(_disposables);
            Todos.AddTo(_disposables);
            IsLoading.AddTo(_disposables);
            CreateTodoCommand.AddTo(_disposables);
            ToggleCompleteCommand.AddTo(_disposables);
            DeleteTodoCommand.AddTo(_disposables);
            LoadTasksCommand.AddTo(_disposables);
        }

        /// <summary>
        /// 全タスクを読み込む
        /// </summary>
        public async UniTask LoadTasksAsync()
        {
            var tasks = await _todoUseCase.GetAllAsync();
            _todos.Value = tasks;
        }

        /// <summary>
        /// 新しいタスクを作成する
        /// </summary>
        public async UniTask CreateTodoAsync()
        {
            // タイトルが空の場合は作成しない
            if (string.IsNullOrWhiteSpace(_newTodoTitle.Value))
            {
                return;
            }

            await _todoUseCase.CreateAsync(_newTodoTitle.Value, _newTodoDescription.Value);
            ClearInputs();
        }

        /// <summary>
        /// タスクの完了状態を切り替える
        /// </summary>
        /// <param name="taskId">対象タスクID</param>
        public async UniTask ToggleCompleteAsync(string taskId)
        {
            await _todoUseCase.ToggleCompleteAsync(taskId);
        }

        /// <summary>
        /// タスクを削除する
        /// </summary>
        /// <param name="taskId">削除対象のタスクID</param>
        public async UniTask DeleteTodoAsync(string taskId)
        {
            // 空実装 - TDD Red Phase
        }

        /// <summary>
        /// 入力フィールドをクリアする
        /// </summary>
        public void ClearInputs()
        {
            _newTodoTitle.Value = "";
            _newTodoDescription.Value = "";
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
