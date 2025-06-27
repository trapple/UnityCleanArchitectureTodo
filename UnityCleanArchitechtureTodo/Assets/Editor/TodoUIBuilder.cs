using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityCleanArchitectureTodo.Infra;
using UnityCleanArchitectureTodo.Presentation.Views;

namespace UnityCleanArchitectureTodo.Editor
{
    /// <summary>
    /// TodoアプリのUI構築用EditorScript
    /// 何度実行しても同じ結果になるよう冪等性を保証
    /// </summary>
    public class TodoUIBuilder : EditorWindow
    {
        [MenuItem("Todo App/Build UI")]
        public static void ShowWindow()
        {
            GetWindow<TodoUIBuilder>("Todo UI Builder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Todo App UI Builder", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Scene Setup", EditorStyles.boldLabel);
            if (GUILayout.Button("1. Setup Main Scene", GUILayout.Height(30)))
            {
                SetupMainScene();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("2. Create TodoItemView Prefab", GUILayout.Height(30)))
            {
                CreateTodoItemViewPrefab();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("3. Setup TodoListView UI", GUILayout.Height(30)))
            {
                SetupTodoListViewUI();
            }

            GUILayout.Space(10);
            GUILayout.Label("Batch Operations", EditorStyles.boldLabel);
            if (GUILayout.Button("Build All UI (Complete Setup)", GUILayout.Height(40)))
            {
                BuildAllUI();
            }

            GUILayout.Space(10);
            GUILayout.Label("Reset", EditorStyles.boldLabel);
            if (GUILayout.Button("Reset Scene (Clean Start)", GUILayout.Height(30)))
            {
                ResetScene();
            }
        }

        /// <summary>
        /// 全UIを一括構築
        /// </summary>
        public static void BuildAllUI()
        {
            Debug.Log("[TodoUIBuilder] Starting complete UI build...");

            SetupMainScene();
            CreateTodoItemViewPrefab();
            SetupTodoListViewUI();

            Debug.Log("[TodoUIBuilder] Complete UI build finished!");
        }

        /// <summary>
        /// メインシーンの基本設定
        /// </summary>
        public static void SetupMainScene()
        {
            Debug.Log("[TodoUIBuilder] Setting up Main Scene...");

            // Canvas設定
            var canvas = SetupCanvas();

            // RootLifetimeScope設定
            SetupRootLifetimeScope();

            // EventSystem確認
            EnsureEventSystem();

            EditorUtility.SetDirty(canvas);
            Debug.Log("[TodoUIBuilder] Main Scene setup completed.");
        }

        /// <summary>
        /// Canvas設定（冪等）
        /// </summary>
        private static Canvas SetupCanvas()
        {
            // 既存のCanvasを探す
            var existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                Debug.Log("[TodoUIBuilder] Canvas already exists, updating settings...");
                ConfigureCanvas(existingCanvas);
                return existingCanvas;
            }

            // 新規Canvas作成
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            ConfigureCanvas(canvas);

            Debug.Log("[TodoUIBuilder] Canvas created.");
            return canvas;
        }

        /// <summary>
        /// Canvas設定
        /// </summary>
        private static void ConfigureCanvas(Canvas canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }
        }

        /// <summary>
        /// RootLifetimeScope設定（冪等）
        /// </summary>
        private static void SetupRootLifetimeScope()
        {
            // 既存のRootLifetimeScopeを探す
            var existing = FindObjectOfType<RootLifetimeScope>();
            if (existing != null)
            {
                Debug.Log("[TodoUIBuilder] RootLifetimeScope already exists.");
                return;
            }

            // 新規RootLifetimeScope作成
            var lifetimeScopeGO = new GameObject("RootLifetimeScope");
            var lifetimeScope = lifetimeScopeGO.AddComponent<RootLifetimeScope>();

            Debug.Log("[TodoUIBuilder] RootLifetimeScope created.");
        }

        /// <summary>
        /// EventSystem確認（冪等）
        /// </summary>
        private static void EnsureEventSystem()
        {
            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("[TodoUIBuilder] EventSystem created.");
            }
            else
            {
                Debug.Log("[TodoUIBuilder] EventSystem already exists.");
            }
        }

        /// <summary>
        /// TodoItemView Prefab作成
        /// </summary>
        public static void CreateTodoItemViewPrefab()
        {
            Debug.Log("[TodoUIBuilder] Creating TodoItemView Prefab...");

            // Prefabsフォルダ作成
            string prefabPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            string prefabFullPath = prefabPath + "/TodoItemView.prefab";

            // 既存Prefabチェック
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullPath);
            if (existingPrefab != null)
            {
                Debug.Log("[TodoUIBuilder] TodoItemView Prefab already exists, updating...");
                // 既存Prefabを更新する場合はここで処理
                return;
            }

            // TodoItemView GameObject構築
            var prefabRoot = new GameObject("TodoItemView");

            // RectTransform設定
            var rectTransform = prefabRoot.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(800, 100);

            // TodoItemViewスクリプト追加
            var todoItemView = prefabRoot.AddComponent<TodoItemView>();

            // 背景Image
            var backgroundImage = prefabRoot.AddComponent<Image>();
            backgroundImage.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            // タイトルテキスト
            var titleGO = new GameObject("TitleText");
            titleGO.transform.SetParent(prefabRoot.transform);
            var titleRect = titleGO.AddComponent<RectTransform>();
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();

            titleRect.anchorMin = new Vector2(0.1f, 0.6f);
            titleRect.anchorMax = new Vector2(0.7f, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            titleText.text = "Sample Title";
            titleText.fontSize = 50;
            titleText.color = Color.black;
            titleText.fontStyle = FontStyles.Bold;

            // 説明テキスト
            var descGO = new GameObject("DescriptionText");
            descGO.transform.SetParent(prefabRoot.transform);
            var descRect = descGO.AddComponent<RectTransform>();
            var descText = descGO.AddComponent<TextMeshProUGUI>();

            descRect.anchorMin = new Vector2(0.1f, 0.2f);
            descRect.anchorMax = new Vector2(0.7f, 0.5f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;

            descText.text = "Sample Description";
            descText.fontSize = 50;
            descText.color = Color.gray;

            // 作成日時テキスト
            var createdAtGO = new GameObject("CreatedAtText");
            createdAtGO.transform.SetParent(prefabRoot.transform);
            var createdAtRect = createdAtGO.AddComponent<RectTransform>();
            var createdAtText = createdAtGO.AddComponent<TextMeshProUGUI>();

            createdAtRect.anchorMin = new Vector2(0.1f, 0.05f);
            createdAtRect.anchorMax = new Vector2(0.5f, 0.2f);
            createdAtRect.offsetMin = Vector2.zero;
            createdAtRect.offsetMax = Vector2.zero;

            createdAtText.text = "2024/01/01 12:00";
            createdAtText.fontSize = 50;
            createdAtText.color = Color.gray;

            // 完了Toggle
            var toggleGO = new GameObject("CompletedToggle");
            toggleGO.transform.SetParent(prefabRoot.transform);
            var toggleRect = toggleGO.AddComponent<RectTransform>();
            var toggle = toggleGO.AddComponent<Toggle>();

            toggleRect.anchorMin = new Vector2(0.75f, 0.4f);
            toggleRect.anchorMax = new Vector2(0.85f, 0.8f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;

            // Toggle背景
            var toggleBg = toggleGO.AddComponent<Image>();
            toggleBg.color = Color.white;

            // Toggleチェックマーク
            var checkmarkGO = new GameObject("Checkmark");
            checkmarkGO.transform.SetParent(toggleGO.transform);
            var checkmarkRect = checkmarkGO.AddComponent<RectTransform>();
            var checkmarkImage = checkmarkGO.AddComponent<Image>();

            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.offsetMin = Vector2.zero;
            checkmarkRect.offsetMax = Vector2.zero;

            checkmarkImage.color = Color.green;
            toggle.graphic = checkmarkImage;

            // 削除ボタン
            var deleteButtonGO = new GameObject("DeleteButton");
            deleteButtonGO.transform.SetParent(prefabRoot.transform);
            var deleteButtonRect = deleteButtonGO.AddComponent<RectTransform>();
            var deleteButton = deleteButtonGO.AddComponent<Button>();
            var deleteButtonImage = deleteButtonGO.AddComponent<Image>();

            deleteButtonRect.anchorMin = new Vector2(0.87f, 0.3f);
            deleteButtonRect.anchorMax = new Vector2(0.97f, 0.7f);
            deleteButtonRect.offsetMin = Vector2.zero;
            deleteButtonRect.offsetMax = Vector2.zero;

            deleteButtonImage.color = Color.red;

            // 削除ボタンテキスト
            var deleteTextGO = new GameObject("Text");
            deleteTextGO.transform.SetParent(deleteButtonGO.transform);
            var deleteTextRect = deleteTextGO.AddComponent<RectTransform>();
            var deleteText = deleteTextGO.AddComponent<TextMeshProUGUI>();

            deleteTextRect.anchorMin = Vector2.zero;
            deleteTextRect.anchorMax = Vector2.one;
            deleteTextRect.offsetMin = Vector2.zero;
            deleteTextRect.offsetMax = Vector2.zero;

            deleteText.text = "×";
            deleteText.fontSize = 50;
            deleteText.color = Color.white;
            deleteText.alignment = TextAlignmentOptions.Center;

            // 完了オーバーレイ
            var overlayGO = new GameObject("CompletedOverlay");
            overlayGO.transform.SetParent(prefabRoot.transform);
            var overlayRect = overlayGO.AddComponent<RectTransform>();
            var overlayImage = overlayGO.AddComponent<Image>();

            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            overlayImage.color = new Color(0f, 1f, 0f, 0.1f);
            overlayGO.SetActive(false);

            // TodoItemViewスクリプトのフィールド設定は手動で行う必要があります
            // （SerializedFieldの設定はEditorでしか行えないため）

            // Prefab作成
            var prefab = PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabFullPath);
            DestroyImmediate(prefabRoot);

            Debug.Log($"[TodoUIBuilder] TodoItemView Prefab created at: {prefabFullPath}");
        }

        /// <summary>
        /// TodoListView UI設定
        /// </summary>
        public static void SetupTodoListViewUI()
        {
            Debug.Log("[TodoUIBuilder] Setting up TodoListView UI...");

            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[TodoUIBuilder] Canvas not found! Please setup Main Scene first.");
                return;
            }

            // 既存のTodoListViewを探す
            var existingTodoListView = FindObjectOfType<TodoListView>();
            if (existingTodoListView != null)
            {
                Debug.Log("[TodoUIBuilder] TodoListView already exists, skipping creation.");
                return;
            }

            // TodoListView Root GameObject
            var todoListViewGO = new GameObject("TodoListView");
            todoListViewGO.transform.SetParent(canvas.transform);

            var todoListRect = todoListViewGO.AddComponent<RectTransform>();
            todoListRect.anchorMin = Vector2.zero;
            todoListRect.anchorMax = Vector2.one;
            todoListRect.offsetMin = Vector2.zero;
            todoListRect.offsetMax = Vector2.zero;

            // TodoListViewスクリプト追加
            var todoListView = todoListViewGO.AddComponent<TodoListView>();

            // ヘッダー
            CreateHeader(todoListViewGO);

            // 入力エリア
            CreateInputArea(todoListViewGO);

            // タスクリストエリア
            CreateTaskListArea(todoListViewGO);

            // フッター（統計情報）
            CreateFooter(todoListViewGO);

            // ローディング表示
            CreateLoadingIndicator(todoListViewGO);

            EditorUtility.SetDirty(todoListViewGO);
            Debug.Log("[TodoUIBuilder] TodoListView UI setup completed.");
        }

        /// <summary>
        /// ヘッダー作成
        /// </summary>
        private static void CreateHeader(GameObject parent)
        {
            var headerGO = new GameObject("Header");
            headerGO.transform.SetParent(parent.transform);

            var headerRect = headerGO.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.9f);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.offsetMin = Vector2.zero;
            headerRect.offsetMax = Vector2.zero;

            var headerImage = headerGO.AddComponent<Image>();
            headerImage.color = new Color(0.2f, 0.3f, 0.4f, 1f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(headerGO.transform);

            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = Vector2.zero;
            titleRect.anchorMax = Vector2.one;
            titleRect.offsetMin = new Vector2(20, 0);
            titleRect.offsetMax = new Vector2(-20, 0);

            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "Todo App - Clean Architecture";
            titleText.fontSize = 50;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
        }

        /// <summary>
        /// 入力エリア作成
        /// </summary>
        private static void CreateInputArea(GameObject parent)
        {
            var inputAreaGO = new GameObject("InputArea");
            inputAreaGO.transform.SetParent(parent.transform);

            var inputAreaRect = inputAreaGO.AddComponent<RectTransform>();
            inputAreaRect.anchorMin = new Vector2(0, 0.75f);
            inputAreaRect.anchorMax = new Vector2(1, 0.9f);
            inputAreaRect.offsetMin = Vector2.zero;
            inputAreaRect.offsetMax = Vector2.zero;

            var inputAreaImage = inputAreaGO.AddComponent<Image>();
            inputAreaImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);

            // タイトル入力
            CreateInputField(inputAreaGO, "NewTodoTitleInput", "タスクのタイトル...",
                new Vector2(0.05f, 0.55f), new Vector2(0.7f, 0.95f));

            // 説明入力
            CreateInputField(inputAreaGO, "NewTodoDescriptionInput", "説明（オプション）...",
                new Vector2(0.05f, 0.05f), new Vector2(0.7f, 0.45f));

            // 追加ボタン
            CreateAddButton(inputAreaGO);
        }

        /// <summary>
        /// 入力フィールド作成
        /// </summary>
        private static void CreateInputField(GameObject parent, string name, string placeholder, Vector2 anchorMin,
            Vector2 anchorMax)
        {
            var inputGO = new GameObject(name);
            inputGO.transform.SetParent(parent.transform);

            var inputRect = inputGO.AddComponent<RectTransform>();
            inputRect.anchorMin = anchorMin;
            inputRect.anchorMax = anchorMax;
            inputRect.offsetMin = Vector2.zero;
            inputRect.offsetMax = Vector2.zero;

            var inputField = inputGO.AddComponent<TMP_InputField>();
            var inputImage = inputGO.AddComponent<Image>();
            inputImage.color = Color.white;

            // Text Area
            var textAreaGO = new GameObject("Text Area");
            textAreaGO.transform.SetParent(inputGO.transform);
            var textAreaRect = textAreaGO.AddComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);

            var textAreaMask = textAreaGO.AddComponent<RectMask2D>();

            // Placeholder
            var placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(textAreaGO.transform);
            var placeholderRect = placeholderGO.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            var placeholderText = placeholderGO.AddComponent<TextMeshProUGUI>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 50;
            placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            placeholderText.fontStyle = FontStyles.Italic;

            // Text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(textAreaGO.transform);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "";
            text.fontSize = 50;
            text.color = Color.black;

            // InputField設定
            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
        }

        /// <summary>
        /// 追加ボタン作成
        /// </summary>
        private static void CreateAddButton(GameObject parent)
        {
            var buttonGO = new GameObject("AddButton");
            buttonGO.transform.SetParent(parent.transform);

            var buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.75f, 0.2f);
            buttonRect.anchorMax = new Vector2(0.95f, 0.8f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;

            var button = buttonGO.AddComponent<Button>();
            var buttonImage = buttonGO.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);

            var buttonTextGO = new GameObject("Text");
            buttonTextGO.transform.SetParent(buttonGO.transform);
            var buttonTextRect = buttonTextGO.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;

            var buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
            buttonText.text = "追加";
            buttonText.fontSize = 50;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.fontStyle = FontStyles.Bold;
        }

        /// <summary>
        /// タスクリストエリア作成
        /// </summary>
        private static void CreateTaskListArea(GameObject parent)
        {
            var scrollViewGO = new GameObject("TaskScrollView");
            scrollViewGO.transform.SetParent(parent.transform);

            var scrollRect = scrollViewGO.AddComponent<ScrollRect>();
            var scrollRectTransform = scrollViewGO.GetComponent<RectTransform>();
            scrollRectTransform.anchorMin = new Vector2(0, 0.1f);
            scrollRectTransform.anchorMax = new Vector2(1, 0.75f);
            scrollRectTransform.offsetMin = Vector2.zero;
            scrollRectTransform.offsetMax = Vector2.zero;

            var scrollImage = scrollViewGO.AddComponent<Image>();
            scrollImage.color = new Color(0.98f, 0.98f, 0.98f, 1f);

            // Viewport
            var viewportGO = new GameObject("Viewport");
            viewportGO.transform.SetParent(scrollViewGO.transform);
            var viewportRect = viewportGO.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            var viewportMask = viewportGO.AddComponent<Mask>();
            var viewportImage = viewportGO.AddComponent<Image>();
            viewportImage.color = Color.white;

            // Content
            var contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewportGO.transform);
            var contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            contentRect.pivot = new Vector2(0.5f, 1);

            // Vertical Layout Group
            var layoutGroup = contentGO.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.spacing = 5;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);

            // Content Size Fitter
            var contentSizeFitter = contentGO.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // ScrollRect設定
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.verticalScrollbar = CreateScrollbar(scrollViewGO);
        }

        /// <summary>
        /// スクロールバー作成
        /// </summary>
        private static Scrollbar CreateScrollbar(GameObject scrollView)
        {
            var scrollbarGO = new GameObject("Scrollbar Vertical");
            scrollbarGO.transform.SetParent(scrollView.transform);

            var scrollbarRect = scrollbarGO.AddComponent<RectTransform>();
            scrollbarRect.anchorMin = new Vector2(1, 0);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.offsetMin = new Vector2(-15, 0);
            scrollbarRect.offsetMax = Vector2.zero;

            var scrollbar = scrollbarGO.AddComponent<Scrollbar>();
            var scrollbarImage = scrollbarGO.AddComponent<Image>();
            scrollbarImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            // Handle
            var handleGO = new GameObject("Sliding Area");
            handleGO.transform.SetParent(scrollbarGO.transform);
            var handleAreaRect = handleGO.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(5, 5);
            handleAreaRect.offsetMax = new Vector2(-5, -5);

            var handleChildGO = new GameObject("Handle");
            handleChildGO.transform.SetParent(handleGO.transform);
            var handleRect = handleChildGO.AddComponent<RectTransform>();
            handleRect.anchorMin = Vector2.zero;
            handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = Vector2.zero;
            handleRect.offsetMax = Vector2.zero;

            var handleImage = handleChildGO.AddComponent<Image>();
            handleImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);

            scrollbar.handleRect = handleRect;
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            var scrollRectComponent = scrollView.GetComponent<ScrollRect>();
            scrollRectComponent.verticalScrollbar = scrollbar;

            return scrollbar;
        }

        /// <summary>
        /// フッター作成
        /// </summary>
        private static void CreateFooter(GameObject parent)
        {
            var footerGO = new GameObject("Footer");
            footerGO.transform.SetParent(parent.transform);

            var footerRect = footerGO.AddComponent<RectTransform>();
            footerRect.anchorMin = new Vector2(0, 0);
            footerRect.anchorMax = new Vector2(1, 0.1f);
            footerRect.offsetMin = Vector2.zero;
            footerRect.offsetMax = Vector2.zero;

            var footerImage = footerGO.AddComponent<Image>();
            footerImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);

            var statsTextGO = new GameObject("StatsText");
            statsTextGO.transform.SetParent(footerGO.transform);

            var statsRect = statsTextGO.AddComponent<RectTransform>();
            statsRect.anchorMin = Vector2.zero;
            statsRect.anchorMax = Vector2.one;
            statsRect.offsetMin = new Vector2(20, 0);
            statsRect.offsetMax = new Vector2(-20, 0);

            var statsText = statsTextGO.AddComponent<TextMeshProUGUI>();
            statsText.text = "タスク数: 0 | 完了: 0";
            statsText.fontSize = 50;
            statsText.color = Color.black;
            statsText.alignment = TextAlignmentOptions.Center;
        }

        /// <summary>
        /// ローディング表示作成
        /// </summary>
        private static void CreateLoadingIndicator(GameObject parent)
        {
            var loadingGO = new GameObject("LoadingIndicator");
            loadingGO.transform.SetParent(parent.transform);

            var loadingRect = loadingGO.AddComponent<RectTransform>();
            loadingRect.anchorMin = Vector2.zero;
            loadingRect.anchorMax = Vector2.one;
            loadingRect.offsetMin = Vector2.zero;
            loadingRect.offsetMax = Vector2.zero;

            var loadingImage = loadingGO.AddComponent<Image>();
            loadingImage.color = new Color(0, 0, 0, 0.5f);

            var loadingTextGO = new GameObject("LoadingText");
            loadingTextGO.transform.SetParent(loadingGO.transform);

            var loadingTextRect = loadingTextGO.AddComponent<RectTransform>();
            loadingTextRect.anchorMin = new Vector2(0.4f, 0.45f);
            loadingTextRect.anchorMax = new Vector2(0.6f, 0.55f);
            loadingTextRect.offsetMin = Vector2.zero;
            loadingTextRect.offsetMax = Vector2.zero;

            var loadingText = loadingTextGO.AddComponent<TextMeshProUGUI>();
            loadingText.text = "読み込み中...";
            loadingText.fontSize = 50;
            loadingText.color = Color.white;
            loadingText.alignment = TextAlignmentOptions.Center;
            loadingText.fontStyle = FontStyles.Bold;

            // 初期状態では非表示
            loadingGO.SetActive(false);
        }

        /// <summary>
        /// シーンリセット
        /// </summary>
        public static void ResetScene()
        {
            Debug.Log("[TodoUIBuilder] Resetting scene...");

            // 既存のUI要素を削除
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                DestroyImmediate(canvas.gameObject);
            }

            var lifetimeScope = FindObjectOfType<RootLifetimeScope>();
            if (lifetimeScope != null)
            {
                DestroyImmediate(lifetimeScope.gameObject);
            }

            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem != null)
            {
                DestroyImmediate(eventSystem.gameObject);
            }

            Debug.Log("[TodoUIBuilder] Scene reset completed.");
        }
    }
}