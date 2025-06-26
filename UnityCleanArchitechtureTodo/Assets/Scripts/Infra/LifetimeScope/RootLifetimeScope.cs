using System.IO;
using UnityEngine;
using VContainer;
using UnityCleanArchitectureTodo.Domain.Repositories;
using UnityCleanArchitectureTodo.App.UseCases;
using UnityCleanArchitectureTodo.Infra.Repositories;
using UnityCleanArchitectureTodo.Presentation.ViewModels;
using UnityCleanArchitectureTodo.Presentation.Presenters;
using UnityCleanArchitectureTodo.Presentation.Views;
using VContainer.Unity;

namespace UnityCleanArchitectureTodo.Infra
{
    /// <summary>
    /// VContainer DI設定のルートLifetimeScope
    /// Clean Architectureの全レイヤーの依存関係を配線する
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [Header("Settings")] [SerializeField] private string _csvFileName = "todos.csv";
        [SerializeField] private bool _useCustomFilePath = false;
        [SerializeField] private string _customFilePath = "";

        protected override void Configure(IContainerBuilder builder)
        {
            // CSV ファイルパスの決定
            var csvFilePath = GetCsvFilePath();
            Debug.Log($"[RootLifetimeScope] Using CSV file path: {csvFilePath}");

            // === Infrastructure層 ===
            RegisterRepositories(builder, csvFilePath);

            // === Application層 ===
            RegisterUseCases(builder);

            // === Presentation層 ===
            RegisterViewModels(builder);
            RegisterPresenters(builder);
            RegisterViews(builder);

            Debug.Log("[RootLifetimeScope] DI configuration completed successfully.");
        }

        /// <summary>
        /// Repository層の依存関係を登録
        /// </summary>
        private void RegisterRepositories(IContainerBuilder builder, string csvFilePath)
        {
            // ITodoRepository -> CsvTodoRepository (Singleton)
            builder.Register<ITodoRepository>(_ => new CsvTodoRepository(csvFilePath),
                Lifetime.Singleton);

            Debug.Log("[RootLifetimeScope] Repositories registered.");
        }

        /// <summary>
        /// UseCase層の依存関係を登録
        /// </summary>
        private void RegisterUseCases(IContainerBuilder builder)
        {
            // TodoUseCase (統合型UseCase) - Transient
            builder.Register<TodoUseCase>(Lifetime.Transient);

            Debug.Log("[RootLifetimeScope] UseCases registered.");
        }

        /// <summary>
        /// ViewModel層の依存関係を登録
        /// </summary>
        private void RegisterViewModels(IContainerBuilder builder)
        {
            // TodoListViewModel - Singleton (UIバインディングのため状態を保持)
            builder.Register<TodoListViewModel>(Lifetime.Singleton);

            Debug.Log("[RootLifetimeScope] ViewModels registered.");
        }

        /// <summary>
        /// Presenter層の依存関係を登録
        /// </summary>
        private void RegisterPresenters(IContainerBuilder builder)
        {
            // TodoListPresenter - EntryPoint (自動初期化)
            builder.RegisterEntryPoint<TodoListPresenter>();

            Debug.Log("[RootLifetimeScope] Presenters registered as EntryPoints.");
        }

        /// <summary>
        /// View層の依存関係を登録
        /// </summary>
        private void RegisterViews(IContainerBuilder builder)
        {
            // TodoListView - シーン内のコンポーネントを自動検索・注入
            builder.RegisterComponentInHierarchy<TodoListView>();

            Debug.Log("[RootLifetimeScope] Views registered from hierarchy.");
        }

        /// <summary>
        /// CSVファイルパスを取得
        /// </summary>
        private string GetCsvFilePath()
        {
            if (_useCustomFilePath && !string.IsNullOrEmpty(_customFilePath))
            {
                // カスタムパスを使用（開発時デバッグ用）
                var directory = Path.GetDirectoryName(_customFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return _customFilePath;
            }

            // 標準のpersistentDataPathを使用（本番用）
            return Path.Combine(Application.persistentDataPath, _csvFileName);
        }

        /// <summary>
        /// デバッグ用：現在のDI設定を検証
        /// </summary>
        [ContextMenu("Validate DI Configuration")]
        private void ValidateDIConfiguration()
        {
            Debug.Log("=== DI Configuration Validation ===");
            Debug.Log($"CSV File Name: {_csvFileName}");
            Debug.Log($"Use Custom Path: {_useCustomFilePath}");
            Debug.Log($"Custom Path: {_customFilePath}");
            Debug.Log($"Resolved CSV Path: {GetCsvFilePath()}");
            Debug.Log($"PersistentDataPath: {Application.persistentDataPath}");
            Debug.Log("===================================");
        }

        /// <summary>
        /// デバッグ用：CSVファイルの状態を確認
        /// </summary>
        [ContextMenu("Check CSV File Status")]
        private void CheckCsvFileStatus()
        {
            var csvPath = GetCsvFilePath();
            Debug.Log("=== CSV File Status ===");
            Debug.Log($"File Path: {csvPath}");
            Debug.Log($"File Exists: {File.Exists(csvPath)}");

            if (File.Exists(csvPath))
            {
                try
                {
                    var content = File.ReadAllText(csvPath);
                    var lineCount = content.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).Length;
                    Debug.Log($"File Size: {new FileInfo(csvPath).Length} bytes");
                    Debug.Log($"Line Count: {lineCount}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error reading CSV file: {ex.Message}");
                }
            }

            Debug.Log("=======================");
        }

        /// <summary>
        /// エディタ用：CSVファイルを削除（リセット用）
        /// </summary>
        [ContextMenu("Delete CSV File (Reset)")]
        private void DeleteCsvFile()
        {
            var csvPath = GetCsvFilePath();
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
                Debug.Log($"CSV file deleted: {csvPath}");
            }
            else
            {
                Debug.Log($"CSV file does not exist: {csvPath}");
            }
        }
    }
}
