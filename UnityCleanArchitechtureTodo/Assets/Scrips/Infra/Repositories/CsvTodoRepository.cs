using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;
using UnityCleanArchitectureTodo.Domain.Repositories;

namespace UnityCleanArchitectureTodo.Infra.Repositories
{
    /// <summary>
    /// CSVファイルを使用してTodoタスクを永続化するリポジトリの実装
    /// </summary>
    public class CsvTodoRepository : ITodoRepository
    {
        private readonly string _filePath;

        public CsvTodoRepository(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// 全てのTodoタスクを取得する
        /// </summary>
        public async UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        {
            // ファイルが存在しない場合は空のリストを返す
            if (!File.Exists(_filePath))
            {
                return new List<TodoTask>().AsReadOnly();
            }

            // ファイルの内容を読み込み
            var fileContent = await File.ReadAllTextAsync(_filePath);
            
            // ファイルが空の場合は空のリストを返す
            if (string.IsNullOrEmpty(fileContent))
            {
                return new List<TodoTask>().AsReadOnly();
            }

            // CSVデータをパースしてタスクリストを作成
            return ParseCsvContent(fileContent);
        }

        /// <summary>
        /// 指定されたIDのTodoタスクを取得する
        /// </summary>
        /// <param name="id">取得対象のタスクID</param>
        public UniTask<TodoTask> GetByIdAsync(string id)
        {
            // TDD Red Phase - まず失敗する実装
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Todoタスクを保存する（新規作成または更新）
        /// </summary>
        /// <param name="task">保存対象のタスク</param>
        public UniTask SaveAsync(TodoTask task)
        {
            // TDD Red Phase - まず失敗する実装
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 指定されたIDのTodoタスクを削除する
        /// </summary>
        /// <param name="id">削除対象のタスクID</param>
        public UniTask DeleteAsync(string id)
        {
            // TDD Red Phase - まず失敗する実装
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// CSVコンテンツをパースしてTodoTaskのリストを作成する
        /// </summary>
        /// <param name="csvContent">CSVファイルの内容</param>
        /// <returns>TodoTaskのリスト</returns>
        private IReadOnlyList<TodoTask> ParseCsvContent(string csvContent)
        {
            var tasks = new List<TodoTask>();
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            // ヘッダー行をスキップ（最初の行）
            if (lines.Length <= 1)
            {
                return tasks.AsReadOnly();
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                    var task = ParseCsvLine(line);
                    if (task != null)
                    {
                        tasks.Add(task);
                    }
                }
                catch
                {
                    // パースエラーは無視して次の行に進む
                    continue;
                }
            }

            return tasks.AsReadOnly();
        }

        /// <summary>
        /// CSV行を解析してTodoTaskオブジェクトを作成する
        /// </summary>
        /// <param name="csvLine">CSV行</param>
        /// <returns>TodoTaskオブジェクト、または解析失敗時はnull</returns>
        private TodoTask ParseCsvLine(string csvLine)
        {
            var fields = csvLine.Split(',');
            if (fields.Length < 6)
                return null;

            var id = fields[0].Trim();
            var title = fields[1].Trim();
            var description = fields[2].Trim();
            var isCompleted = bool.Parse(fields[3].Trim());
            var createdAt = DateTime.Parse(fields[4].Trim(), null, DateTimeStyles.RoundtripKind);
            var completedAtStr = fields[5].Trim();
            DateTime? completedAt = string.IsNullOrEmpty(completedAtStr) 
                ? null 
                : DateTime.Parse(completedAtStr, null, DateTimeStyles.RoundtripKind);

            // TodoTaskを作成（仮のタイトルで作成してから必要なプロパティを設定）
            var task = new TodoTask(title, description);
            
            // リフレクションを使用してプライベートフィールドを設定
            var taskType = typeof(TodoTask);
            
            // Idプロパティを設定
            var idField = taskType.GetField("<Id>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            idField?.SetValue(task, id);
            
            // CreatedAtプロパティを設定
            var createdAtField = taskType.GetField("<CreatedAt>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            createdAtField?.SetValue(task, createdAt);
            
            // CompletedAtプロパティを設定
            var completedAtField = taskType.GetField("<CompletedAt>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            completedAtField?.SetValue(task, completedAt);
            
            // IsCompletedプロパティを設定
            var isCompletedField = taskType.GetField("<IsCompleted>k__BackingField", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            isCompletedField?.SetValue(task, isCompleted);

            return task;
        }
    }
}