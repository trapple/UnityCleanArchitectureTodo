using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityCleanArchitectureTodo.Domain.Entities;

namespace UnityCleanArchitectureTodo.Domain.Repositories
{
    /// <summary>
    /// TodoTaskの永続化を担当するリポジトリインターフェース
    /// Clean Architecture: Domain層でのインターフェース定義
    /// 実装はInfra層で行い、依存関係逆転の原則に従う
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// 全てのTodoタスクを取得する
        /// </summary>
        /// <returns>Todoタスクの読み取り専用リスト</returns>
        UniTask<IReadOnlyList<TodoTask>> GetAllAsync();

        /// <summary>
        /// 指定されたIDのTodoタスクを取得する
        /// </summary>
        /// <param name="id">検索対象のタスクID</param>
        /// <returns>見つかったタスク、存在しない場合はnull</returns>
        UniTask<TodoTask> GetByIdAsync(string id);

        /// <summary>
        /// Todoタスクを保存する（新規作成または更新）
        /// </summary>
        /// <param name="task">保存対象のタスク</param>
        /// <returns>非同期処理</returns>
        UniTask SaveAsync(TodoTask task);

        /// <summary>
        /// 指定されたIDのTodoタスクを削除する
        /// </summary>
        /// <param name="id">削除対象のタスクID</param>
        /// <returns>非同期処理</returns>
        UniTask DeleteAsync(string id);
    }
}
