using System;

namespace UnityCleanArchitectureTodo.Tests.TestUtils
{
    /// <summary>
    /// テスト用のTimeProviderモック
    /// 固定された時刻を返すことでテスト可能にする
    /// </summary>
    public class MockTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        /// <summary>
        /// モックTimeProviderのコンストラクタ
        /// </summary>
        /// <param name="utcNow">返すUTC時刻</param>
        public MockTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        /// <summary>
        /// 固定されたUTC時刻を返す
        /// </summary>
        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }

        /// <summary>
        /// モックの時刻を設定する
        /// </summary>
        /// <param name="utcNow">新しいUTC時刻</param>
        public void SetUtcNow(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        /// <summary>
        /// DateTime版のSetUtcNowメソッド（利便性のため）
        /// </summary>
        /// <param name="dateTime">新しい時刻</param>
        public void SetUtcNow(DateTime dateTime)
        {
            _utcNow = new DateTimeOffset(dateTime);
        }
    }
}