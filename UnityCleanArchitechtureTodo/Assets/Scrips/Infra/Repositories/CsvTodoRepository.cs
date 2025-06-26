using System.Collections.Generic;
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
        public UniTask<IReadOnlyList<TodoTask>> GetAllAsync()
        {
            // TDD Red Phase - まず失敗する実装
            throw new System.NotImplementedException();
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
    }
}