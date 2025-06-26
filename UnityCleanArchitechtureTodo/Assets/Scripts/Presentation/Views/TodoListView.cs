using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using R3;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Presentation.ViewModels;

namespace UnityCleanArchitectureTodo.Presentation.Views
{
    /// <summary>
    /// TodoListのView
    /// Unity UIコンポーネントとViewModelをバインドし、ユーザーインターフェースを提供する
    /// </summary>
    public class TodoListView : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private Transform _todoListParent;

        [SerializeField] private TodoItemView _todoItemPrefab;
        [SerializeField] private Button _addButton;
        [SerializeField] private TMP_InputField _newTodoTitleInput;
        [SerializeField] private TMP_InputField _newTodoDescriptionInput;
        [SerializeField] private GameObject _loadingIndicator;

        [Header("Optional UI")] [SerializeField]
        private TextMeshProUGUI _taskCountText;

        [SerializeField] private Button _loadTasksButton;

        private TodoListViewModel _viewModel;
        private readonly List<TodoItemView> _todoItemViews = new();
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        /// VContainer依存性注入
        /// </summary>
        [Inject]
        public void Construct(TodoListViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            BindToViewModel();
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// ViewModelとUIコンポーネントをバインド
        /// </summary>
        private void BindToViewModel()
        {
            if (_viewModel == null)
            {
                Debug.LogError("ViewModel is null! VContainer injection might have failed.");
                return;
            }

            // ViewModelの状態をUIにバインド
            BindViewModelStates();

            // InputフィールドをViewModelにバインド
            BindInputFields();

            // CommandをUIイベントにバインド
            BindCommands();
        }

        /// <summary>
        /// ViewModelの状態プロパティをUIにバインド
        /// </summary>
        private void BindViewModelStates()
        {
            // Todosリストの変更を監視
            _viewModel.Todos
                .Subscribe(OnTodosChanged)
                .AddTo(_disposables);

            // Loading状態をUIに反映
            _viewModel.IsLoading
                .Subscribe(isLoading =>
                {
                    if (_loadingIndicator != null)
                        _loadingIndicator.SetActive(isLoading);
                })
                .AddTo(_disposables);

            // タスク数表示（オプション）
            if (_taskCountText != null)
            {
                _viewModel.Todos
                    .Subscribe(todos =>
                    {
                        var completed = 0;
                        foreach (var todo in todos)
                        {
                            if (todo.IsCompleted) completed++;
                        }

                        _taskCountText.text = $"Tasks: {todos.Count} (Completed: {completed})";
                    })
                    .AddTo(_disposables);
            }
        }

        /// <summary>
        /// 入力フィールドをViewModelにバインド
        /// </summary>
        private void BindInputFields()
        {
            // タイトル入力フィールド
            if (_newTodoTitleInput != null)
            {
                // ViewModel → UI への一方向バインディング
                _viewModel.NewTodoTitle
                    .Subscribe(text => _newTodoTitleInput.text = text)
                    .AddTo(_disposables);

                // UI → ViewModel への一方向バインディング
                _newTodoTitleInput.onValueChanged.AsObservable()
                    .Subscribe(text => _viewModel.NewTodoTitle.Value = text)
                    .AddTo(_disposables);
            }

            // 説明入力フィールド
            if (_newTodoDescriptionInput != null)
            {
                // ViewModel → UI への一方向バインディング
                _viewModel.NewTodoDescription
                    .Subscribe(text => _newTodoDescriptionInput.text = text)
                    .AddTo(_disposables);

                // UI → ViewModel への一方向バインディング
                _newTodoDescriptionInput.onValueChanged.AsObservable()
                    .Subscribe(text => _viewModel.NewTodoDescription.Value = text)
                    .AddTo(_disposables);
            }
        }

        /// <summary>
        /// CommandをUIイベントにバインド
        /// </summary>
        private void BindCommands()
        {
            // 追加ボタン
            if (_addButton != null)
            {
                _addButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.CreateTodoCommand.Execute(Unit.Default))
                    .AddTo(_disposables);
            }

            // タスク読み込みボタン（オプション）
            if (_loadTasksButton != null)
            {
                _loadTasksButton.onClick.AsObservable()
                    .Subscribe(_ => _viewModel.LoadTasksCommand.Execute(Unit.Default))
                    .AddTo(_disposables);
            }
        }

        /// <summary>
        /// Todosリストの変更時に呼ばれるコールバック
        /// </summary>
        /// <param name="todos">新しいTodosリスト</param>
        private void OnTodosChanged(IReadOnlyList<TodoTask> todos)
        {
            // 既存のアイテムをクリア
            ClearExistingItems();

            // 新しいアイテムを作成
            CreateTodoItems(todos);
        }

        /// <summary>
        /// 既存のTodoアイテムをクリア
        /// </summary>
        private void ClearExistingItems()
        {
            foreach (var item in _todoItemViews)
            {
                if (item != null)
                {
                    item.Cleanup();
                    Destroy(item.gameObject);
                }
            }

            _todoItemViews.Clear();
        }

        /// <summary>
        /// 新しいTodoアイテムを作成
        /// </summary>
        /// <param name="todos">作成するTodosリスト</param>
        private void CreateTodoItems(IReadOnlyList<TodoTask> todos)
        {
            if (_todoItemPrefab == null || _todoListParent == null)
            {
                Debug.LogWarning("TodoItemPrefab or TodoListParent is not assigned!");
                return;
            }

            foreach (var todo in todos)
            {
                var itemView = Instantiate(_todoItemPrefab, _todoListParent);
                itemView.Setup(
                    todo,
                    id => _viewModel.ToggleCompleteCommand.Execute(id),
                    id => _viewModel.DeleteTodoCommand.Execute(id)
                );
                _todoItemViews.Add(itemView);
            }
        }

        /// <summary>
        /// デバッグ用：ViewModelの状態をログ出力
        /// </summary>
        [ContextMenu("Debug ViewModel State")]
        private void DebugViewModelState()
        {
            if (_viewModel == null)
            {
                Debug.Log("ViewModel is null");
                return;
            }

            Debug.Log($"Todos Count: {_viewModel.Todos.CurrentValue.Count}");
            Debug.Log($"IsLoading: {_viewModel.IsLoading.CurrentValue}");
            Debug.Log($"NewTodoTitle: '{_viewModel.NewTodoTitle.Value}'");
            Debug.Log($"NewTodoDescription: '{_viewModel.NewTodoDescription.Value}'");
        }
    }
}
