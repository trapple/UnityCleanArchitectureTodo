using UnityEngine;

namespace UnityCleanArchitectureTodo.Presentation.UI
{
    /// <summary>
    /// SafeArea対応コンポーネント
    /// Canvasに貼るだけで自動的にSafeAreaに対応したレイアウトを提供
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHandler : MonoBehaviour
    {
        [Header("Safe Area Settings")]
        [SerializeField] private bool _enableSafeArea = true;
        [SerializeField] private bool _enableTop = true;
        [SerializeField] private bool _enableBottom = true;
        [SerializeField] private bool _enableLeft = true;
        [SerializeField] private bool _enableRight = true;

        [Header("Debug")]
        [SerializeField] private bool _showDebugInfo = false;

        private RectTransform _rectTransform = null!;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            // SafeAreaが変更された場合や画面サイズが変更された場合に再適用
            if (HasSafeAreaChanged() || HasScreenSizeChanged())
            {
                ApplySafeArea();
            }
        }

        /// <summary>
        /// SafeAreaが変更されたかチェック
        /// </summary>
        private bool HasSafeAreaChanged()
        {
            return Screen.safeArea != _lastSafeArea;
        }

        /// <summary>
        /// 画面サイズが変更されたかチェック
        /// </summary>
        private bool HasScreenSizeChanged()
        {
            return Screen.width != _lastScreenSize.x || Screen.height != _lastScreenSize.y;
        }

        /// <summary>
        /// SafeAreaを適用
        /// </summary>
        public void ApplySafeArea()
        {
            if (!_enableSafeArea)
            {
                ResetToFullScreen();
                return;
            }

            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);

            // SafeAreaを正規化（0-1の範囲に変換）
            var anchorMin = new Vector2(
                _enableLeft ? safeArea.x / screenSize.x : 0f,
                _enableBottom ? safeArea.y / screenSize.y : 0f
            );

            var anchorMax = new Vector2(
                _enableRight ? (safeArea.x + safeArea.width) / screenSize.x : 1f,
                _enableTop ? (safeArea.y + safeArea.height) / screenSize.y : 1f
            );

            // RectTransformに適用
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            // 状態を保存
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            if (_showDebugInfo)
            {
                LogDebugInfo(safeArea, screenSize, anchorMin, anchorMax);
            }
        }

        /// <summary>
        /// フルスクリーンに戻す
        /// </summary>
        private void ResetToFullScreen()
        {
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// デバッグ情報をログ出力
        /// </summary>
        private void LogDebugInfo(Rect safeArea, Vector2 screenSize, Vector2 anchorMin, Vector2 anchorMax)
        {
            Debug.Log($"[SafeAreaHandler] Screen: {screenSize}, SafeArea: {safeArea}");
            Debug.Log($"[SafeAreaHandler] AnchorMin: {anchorMin}, AnchorMax: {anchorMax}");
            Debug.Log($"[SafeAreaHandler] SafeArea Size: {safeArea.width}x{safeArea.height}");
            
            var topInset = screenSize.y - (safeArea.y + safeArea.height);
            var bottomInset = safeArea.y;
            var leftInset = safeArea.x;
            var rightInset = screenSize.x - (safeArea.x + safeArea.width);
            
            Debug.Log($"[SafeAreaHandler] Insets - Top: {topInset}, Bottom: {bottomInset}, Left: {leftInset}, Right: {rightInset}");
        }

        /// <summary>
        /// SafeArea対応を有効/無効にする
        /// </summary>
        public void SetSafeAreaEnabled(bool enabled)
        {
            _enableSafeArea = enabled;
            ApplySafeArea();
        }

        /// <summary>
        /// 特定の方向のSafeArea対応を設定
        /// </summary>
        public void SetSafeAreaDirection(bool top, bool bottom, bool left, bool right)
        {
            _enableTop = top;
            _enableBottom = bottom;
            _enableLeft = left;
            _enableRight = right;
            ApplySafeArea();
        }

        /// <summary>
        /// デバッグ情報表示を切り替え
        /// </summary>
        public void SetDebugMode(bool enabled)
        {
            _showDebugInfo = enabled;
        }

        /// <summary>
        /// 現在のSafeArea情報を取得
        /// </summary>
        public SafeAreaInfo GetSafeAreaInfo()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2(Screen.width, Screen.height);
            
            return new SafeAreaInfo
            {
                ScreenSize = screenSize,
                SafeArea = safeArea,
                TopInset = screenSize.y - (safeArea.y + safeArea.height),
                BottomInset = safeArea.y,
                LeftInset = safeArea.x,
                RightInset = screenSize.x - (safeArea.x + safeArea.width),
                IsEnabled = _enableSafeArea
            };
        }

        /// <summary>
        /// Inspector用：手動でSafeAreaを適用
        /// </summary>
        [ContextMenu("Apply Safe Area")]
        private void ManualApplySafeArea()
        {
            ApplySafeArea();
        }

        /// <summary>
        /// Inspector用：フルスクリーンにリセット
        /// </summary>
        [ContextMenu("Reset to Full Screen")]
        private void ManualResetToFullScreen()
        {
            ResetToFullScreen();
        }

        /// <summary>
        /// Inspector用：現在のSafeArea情報をログ出力
        /// </summary>
        [ContextMenu("Log Safe Area Info")]
        private void ManualLogSafeAreaInfo()
        {
            var info = GetSafeAreaInfo();
            Debug.Log($"[SafeAreaHandler] Current Info: {info}");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Inspectorで値が変更された時に自動適用
            if (_rectTransform != null && Application.isPlaying)
            {
                ApplySafeArea();
            }
        }
#endif
    }

    /// <summary>
    /// SafeArea情報を格納する構造体
    /// </summary>
    [System.Serializable]
    public struct SafeAreaInfo
    {
        public Vector2 ScreenSize;
        public Rect SafeArea;
        public float TopInset;
        public float BottomInset;
        public float LeftInset;
        public float RightInset;
        public bool IsEnabled;

        public override string ToString()
        {
            return $"Screen: {ScreenSize}, SafeArea: {SafeArea}, " +
                   $"Insets(T:{TopInset:F1}, B:{BottomInset:F1}, L:{LeftInset:F1}, R:{RightInset:F1}), " +
                   $"Enabled: {IsEnabled}";
        }
    }
}