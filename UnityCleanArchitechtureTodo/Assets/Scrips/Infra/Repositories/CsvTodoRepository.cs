using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        public async UniTask<TodoTask> GetByIdAsync(string id)
        {
            // 全タスクを取得してから指定されたIDを検索
            var allTasks = await GetAllAsync();
            return allTasks.FirstOrDefault(task => task.Id == id);
        }

        /// <summary>
        /// Todoタスクを保存する（新規作成または更新）
        /// </summary>
        /// <param name="task">保存対象のタスク</param>
        public async UniTask SaveAsync(TodoTask task)
        {
            // 既存のタスクリストを取得
            var existingTasks = (await GetAllAsync()).ToList();
            
            // 同じIDのタスクが存在するかチェック
            var existingIndex = existingTasks.FindIndex(t => t.Id == task.Id);
            
            if (existingIndex >= 0)
            {
                // 既存タスクを更新
                existingTasks[existingIndex] = task;
            }
            else
            {
                // 新規タスクを追加
                existingTasks.Add(task);
            }
            
            // CSVファイルに書き込み
            await WriteCsvFileAsync(existingTasks);
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

            // 復元用コンストラクタを使用してTodoTaskを作成
            return new TodoTask(id, title, description, isCompleted, createdAt, completedAt);
        }

        /// <summary>
        /// タスクリストをCSVファイルに書き込む
        /// </summary>
        /// <param name="tasks">書き込み対象のタスクリスト</param>
        private async UniTask WriteCsvFileAsync(IList<TodoTask> tasks)
        {
            var csvLines = new List<string>();
            
            // ヘッダー行を追加
            csvLines.Add("Id,Title,Description,IsCompleted,CreatedAt,CompletedAt");
            
            // 各タスクをCSV行に変換
            foreach (var task in tasks)
            {
                var completedAtStr = task.CompletedAt?.ToString("O") ?? "";
                var csvLine = $"{task.Id},{task.Title},{task.Description},{task.IsCompleted},{task.CreatedAt:O},{completedAtStr}";
                csvLines.Add(csvLine);
            }
            
            // ファイルに書き込み
            var csvContent = string.Join("\n", csvLines);
            await File.WriteAllTextAsync(_filePath, csvContent);
        }
    }
}