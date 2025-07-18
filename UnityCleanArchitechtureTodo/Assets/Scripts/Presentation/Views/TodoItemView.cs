using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using UnityCleanArchitectureTodo.Domain.Entities;

namespace UnityCleanArchitectureTodo.Presentation.Views
{
    /// <summary>
    /// 個別のTodoアイテムを表示するView
    /// TodoTask一つ分のUIコンポーネントを管理する
    /// </summary>
    public class TodoItemView : MonoBehaviour
    {
        [Header("Text Components")] [SerializeField]
        private TextMeshProUGUI _titleText = null!;

        [SerializeField] private TextMeshProUGUI _descriptionText = null!;
        [SerializeField] private TextMeshProUGUI _createdAtText = null!;

        [Header("Interactive Components")] [SerializeField]
        private Toggle _completedToggle = null!;

        [SerializeField] private Button _deleteButton = null!;

        [Header("Visual States")] [SerializeField]
        private GameObject _completedOverlay = null!;

        [SerializeField] private Color _completedTextColor = Color.gray;
        [SerializeField] private Color _normalTextColor = Color.black;

        private TodoTask _currentTask;
        private Action<string> _onToggleComplete;
        private Action<string> _onDelete;
        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            ValidateSerializedFields();
        }

        /// <summary>
        /// TodoItemViewを初期化し、イベントコールバックを設定
        /// </summary>
        /// <param name="task">表示するTodoTask</param>
        /// <param name="onToggleComplete">完了切り替え時のコールバック</param>
        /// <param name="onDelete">削除時のコールバック</param>
        public void Setup(TodoTask task, Action<string> onToggleComplete, Action<string> onDelete)
        {
            _currentTask = task;
            _onToggleComplete = onToggleComplete;
            _onDelete = onDelete;

            // UIを更新
            UpdateUI();

            // イベントをバインド
            BindEvents();
        }

        /// <summary>
        /// リソースをクリーンアップ
        /// </summary>
        public void Cleanup()
        {
            _disposables?.Dispose();
            _currentTask = null;
            _onToggleComplete = null;
            _onDelete = null;
        }

        /// <summary>
        /// 現在のタスク情報に基づいてUIを更新
        /// </summary>
        private void UpdateUI()
        {
            if (_currentTask == null) return;

            // テキスト情報の更新
            UpdateTexts();

            // 完了状態の更新
            UpdateCompletionState();

            // 視覚的状態の更新
            UpdateVisualState();
        }

        /// <summary>
        /// テキスト情報を更新
        /// </summary>
        private void UpdateTexts()
        {
            _titleText.text = _currentTask.Title;

            _descriptionText.text = string.IsNullOrEmpty(_currentTask.Description)
                ? ""
                : _currentTask.Description;

            _createdAtText.text = _currentTask.CreatedAt.ToString("yyyy/MM/dd HH:mm");
        }

        /// <summary>
        /// 完了状態を更新
        /// </summary>
        private void UpdateCompletionState()
        {
            // イベント発火を一時的に無効化
            _completedToggle.SetIsOnWithoutNotify(_currentTask.IsCompleted);
        }

        /// <summary>
        /// 視覚的状態を更新（完了/未完了の見た目）
        /// </summary>
        private void UpdateVisualState()
        {
            // 完了オーバーレイの表示/非表示
            _completedOverlay.SetActive(_currentTask.IsCompleted);

            // テキストカラーの変更
            Color textColor = _currentTask.IsCompleted ? _completedTextColor : _normalTextColor;
            _titleText.color = textColor;
            _descriptionText.color = textColor;

            // 完了時のテキスト装飾（打ち消し線など）
            _titleText.fontStyle = _currentTask.IsCompleted
                ? FontStyles.Strikethrough
                : FontStyles.Normal;
        }

        /// <summary>
        /// UIイベントをバインド
        /// </summary>
        private void BindEvents()
        {
            // 完了切り替えToggle
            _completedToggle.onValueChanged.AsObservable()
                .Subscribe(OnToggleChanged)
                .AddTo(_disposables);

            // 削除ボタン
            _deleteButton.onClick.AsObservable()
                .Subscribe(_ => OnDeleteClicked())
                .AddTo(_disposables);
        }

        /// <summary>
        /// 完了Toggle変更時のコールバック
        /// </summary>
        /// <param name="isOn">新しい完了状態</param>
        private void OnToggleChanged(bool isOn)
        {
            if (_currentTask == null || _onToggleComplete == null) return;

            // タスクの状態を即座に更新（UIの反応性向上）
            if (_currentTask.IsCompleted != isOn)
            {
                _onToggleComplete.Invoke(_currentTask.Id);
            }
        }

        /// <summary>
        /// 削除ボタンクリック時のコールバック
        /// </summary>
        private void OnDeleteClicked()
        {
            if (_currentTask == null || _onDelete == null) return;

            // 確認ダイアログを表示することも可能（今回は直接削除）
            _onDelete.Invoke(_currentTask.Id);
        }

        /// <summary>
        /// デバッグ用：現在のタスク情報をログ出力
        /// </summary>
        [ContextMenu("Debug Task Info")]
        private void DebugTaskInfo()
        {
            if (_currentTask == null)
            {
                Debug.Log("No task assigned");
                return;
            }

            Debug.Log($"Task ID: {_currentTask.Id}");
            Debug.Log($"Title: {_currentTask.Title}");
            Debug.Log($"Description: {_currentTask.Description}");
            Debug.Log($"IsCompleted: {_currentTask.IsCompleted}");
            Debug.Log($"CreatedAt: {_currentTask.CreatedAt}");
            Debug.Log($"CompletedAt: {_currentTask.CompletedAt}");
        }

        /// <summary>
        /// SerializedFieldのnullチェック
        /// </summary>
        private void ValidateSerializedFields()
        {
            if (_titleText == null)
                throw new NullReferenceException($"[{gameObject.name}] _titleText is null! Please assign the TitleText component in the inspector.");

            if (_descriptionText == null)
                throw new NullReferenceException($"[{gameObject.name}] _descriptionText is null! Please assign the DescriptionText component in the inspector.");

            if (_createdAtText == null)
                throw new NullReferenceException($"[{gameObject.name}] _createdAtText is null! Please assign the CreatedAtText component in the inspector.");

            if (_completedToggle == null)
                throw new NullReferenceException($"[{gameObject.name}] _completedToggle is null! Please assign the CompletedToggle component in the inspector.");

            if (_deleteButton == null)
                throw new NullReferenceException($"[{gameObject.name}] _deleteButton is null! Please assign the DeleteButton component in the inspector.");

            if (_completedOverlay == null)
                throw new NullReferenceException($"[{gameObject.name}] _completedOverlay is null! Please assign the CompletedOverlay GameObject in the inspector.");
        }

        private void OnDestroy()
        {
            Cleanup();
        }
    }
}
