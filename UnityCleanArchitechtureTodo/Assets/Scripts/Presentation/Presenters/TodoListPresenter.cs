using System;
using UnityEngine;
using VContainer.Unity;
using R3;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Presentation.ViewModels;
using UnityCleanArchitectureTodo.App.UseCases;

namespace UnityCleanArchitectureTodo.Presentation.Presenters
{
    /// <summary>
    /// TodoListのPresenter
    /// ViewModelとUseCaseの橋渡しを行い、ビジネスロジックを実行する
    /// VContainer EntryPointパターンで自動初期化される
    /// </summary>
    public class TodoListPresenter : IStartable, IDisposable
    {
        private readonly TodoListViewModel _viewModel;
        private readonly TodoUseCase _todoUseCase;
        private readonly CompositeDisposable _disposables;

        public TodoListPresenter(
            TodoListViewModel viewModel,
            TodoUseCase todoUseCase)
        {
            _viewModel = viewModel;
            _todoUseCase = todoUseCase;
            _disposables = new CompositeDisposable();
        }

        /// <summary>
        /// VContainer EntryPointとして自動実行される初期化処理
        /// </summary>
        public void Start()
        {
            // ViewModelのCommandをPresenterのメソッドにバインド
            BindCommands();

            // 初期データロード
            LoadTodosAsync().Forget();
        }

        /// <summary>
        /// ViewModelのCommandをPresenterのビジネスロジックにバインド
        /// </summary>
        private void BindCommands()
        {
            // タスク作成コマンド
            _viewModel.CreateTodoCommand
                .Subscribe(_ => OnCreateTodoAsync().Forget())
                .AddTo(_disposables);

            // 完了切り替えコマンド
            _viewModel.ToggleCompleteCommand
                .Subscribe(OnToggleCompleteAsync)
                .AddTo(_disposables);

            // タスク削除コマンド
            _viewModel.DeleteTodoCommand
                .Subscribe(OnDeleteTodoAsync)
                .AddTo(_disposables);

            // タスク読み込みコマンド
            _viewModel.LoadTasksCommand
                .Subscribe(_ => LoadTodosAsync().Forget())
                .AddTo(_disposables);
        }

        /// <summary>
        /// 新しいタスク作成処理
        /// </summary>
        private async UniTask OnCreateTodoAsync()
        {
            // タイトルが空の場合は作成しない
            if (string.IsNullOrWhiteSpace(_viewModel.NewTodoTitle.Value))
            {
                return;
            }

            try
            {
                // UseCaseを直接呼び出してタスク作成
                await _todoUseCase.CreateAsync(_viewModel.NewTodoTitle.Value, _viewModel.NewTodoDescription.Value);

                // 入力フィールドをクリア
                _viewModel.ClearInputs();

                // 作成後にリストを再読み込み
                await LoadTodosAsync();
            }
            catch (Exception ex)
            {
                // エラーハンドリング（今後、エラー表示機能を追加予定）
                Debug.LogError($"タスク作成エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// タスク完了状態切り替え処理
        /// </summary>
        /// <param name="taskId">対象タスクのID</param>
        private async void OnToggleCompleteAsync(string taskId)
        {
            try
            {
                // UseCaseを直接呼び出して完了状態切り替え
                await _todoUseCase.ToggleCompleteAsync(taskId);

                // 切り替え後にリストを再読み込み
                await LoadTodosAsync();
            }
            catch (Exception ex)
            {
                // エラーハンドリング
                Debug.LogError($"タスク完了切り替えエラー: {ex.Message}");
            }
        }

        /// <summary>
        /// タスク削除処理
        /// </summary>
        /// <param name="taskId">削除対象のタスクID</param>
        private async void OnDeleteTodoAsync(string taskId)
        {
            try
            {
                // UseCaseを直接呼び出してタスク削除
                await _todoUseCase.DeleteAsync(taskId);

                // 削除後にリストを再読み込み
                await LoadTodosAsync();
            }
            catch (Exception ex)
            {
                // エラーハンドリング
                Debug.LogError($"タスク削除エラー: {ex.Message}");
            }
        }

        /// <summary>
        /// 全タスクの読み込み処理
        /// </summary>
        private async UniTask LoadTodosAsync()
        {
            try
            {
                // Loading状態管理はViewModelに委譲
                await _viewModel.LoadTasksAsync();
            }
            catch (Exception ex)
            {
                // エラーハンドリング
                Debug.LogError($"タスク読み込みエラー: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
