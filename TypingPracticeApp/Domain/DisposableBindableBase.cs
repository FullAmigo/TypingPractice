#region References

using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;

#endregion

namespace TypingPracticeApp.Domain
{
    /// <summary>
    /// Dispose パターンを実装した基本となるオブジェクトを表します。
    /// </summary>
    public abstract class DisposableBindableBase : BindableBase, ICancelable
    {
        /// <summary>Dispose 可能なインスタンスのコンテナを表します。</summary>
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>Dispose メソッドが呼び出されたかをスレッドセーフで管理する値を表します。</summary>
        private long disposableState;

        /// <summary>
        /// <see cref="DisposableBindableBase" /> クラスの新しいインスタンスを生成します。
        /// </summary>
        protected DisposableBindableBase()
        {
            DebugLog.Print($"■ {this.GetType().FullName}.ctor...");
        }

        /// <summary>
        /// Dispose されたかを取得します。
        /// </summary>
        /// <value>
        /// 値を表す <see cref="bool" /> 型。
        /// <para>Dispose された場合 true。既定値は false です。</para>
        /// </value>
        public bool IsDisposed => Interlocked.Read(ref this.disposableState) == 1L;

        /// <summary>
        /// Dispose 可能なインスタンスのコンテナを取得します。
        /// </summary>
        /// <value>
        /// 値を表す <see cref="CompositeDisposable" /> 型。
        /// <para>Dispose 可能なインスタンスのコンテナ。既定値は null です。</para>
        /// </value>
        protected CompositeDisposable Disposables => this.disposables;

        /// <summary>
        /// アンマネージ リソースの解放およびリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dipose するときにログを出力します。
        /// </summary>
        [Conditional("DEBUG")]
        protected virtual void WriteLogAtDisposing() => DebugLog.Print($"■ {this.GetType().FullName}.Disposing...");

        /// <summary>
        /// Dipose したときにログを出力します。
        /// </summary>
        [Conditional("DEBUG")]
        protected virtual void WriteLogAtDisposed()
        {
        }

        /// <summary>
        /// マネージ リソースを解放します。
        /// </summary>
        protected virtual void DisposeManagedInstances()
        {
        }

        /// <summary>
        /// アンマネージ リソースを解放します。
        /// </summary>
        protected virtual void DisposeUnmanagedInstances()
        {
        }

        /// <summary>
        /// Dispose されていれば例外を発生させます。
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// <see cref="DisposableBindableBase" /> クラスのインスタンスによって使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージ リソースとアンマネージ リソースの両方を解放する場合は true。アンマネージ リソースだけを解放する場合は false。</param>
        private void Dispose(bool disposing)
        {
            // GC からファイナライザ経由で Dispose されたとき lock (this.thisLock) の箇所で
            // Monitor.ReliableEnter(Object obj, Boolean& lockTaken) 内で ArgumentNullException 例外が発生します。
            // ファイナライザ経由では、既に本インスタンスが Dispose されているため、参照インスタンス (class) に触れることができません。
            // よって、プリミティブな値を利用してロックするようにします。
            if (Interlocked.CompareExchange(ref this.disposableState, 1L, 0L) == 0L)
            {
                this.WriteLogAtDisposing();
                if (disposing)
                {
                    // マネージ リソース (IDisposable の実装インスタンス) の解放処理をこの位置に記述します。
                    // 例）
                    // ((IDisposable)this.disposableSomeField1).Close();
                    // ((IDisposable)this.disposableSomeField2).Dispose();
                    this.DisposeManagedInstances();
                    this.disposables.Dispose();
                }

                // アンマネージ リソース (IDisposable の非実装インスタンス) の解放処理をこの位置に記述します。
                // 例）
                // NativeMethod.CloseHandle(this.otherSomeField1);
                // this.otherSomeField1 = IntPtr.Zero;
                this.DisposeUnmanagedInstances();

                this.WriteLogAtDisposed();
            }
        }
    }
}
