using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using Reactive.Bindings.Extensions;

namespace TypingPracticeApp.Domain
{
    /// <summary>
    /// シーケンスカウントを発行します。
    /// </summary>
    public class SequenceCountNotifier : DisposableBindableBase, IObservable<int>
    {
        #region フィールド

        /// <summary>現在のインスタンスのロックオブジェクトを表します。</summary>
        private readonly object _gate = new object();
        
        /// <summary>サブジェクトを表します。</summary>
        private readonly Subject<int> _trigger;

        /// <summary>現在値を表します。</summary>
        private int _value;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// <see cref="SequenceCountNotifier" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="initialValue">初期値。</param>
        /// <param name="min">最小値。</param>
        /// <param name="max">最大値。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Simplify API")]
        public SequenceCountNotifier(int initialValue = 0, int min = int.MinValue, int max = int.MaxValue)
        {
            this._trigger = new Subject<int>().AddTo(this.Disposables);
            this.MinValue = Math.Min(min, max);
            this.MaxValue = Math.Max(min, max);

            this._value = Math.Min(Math.Max(initialValue, this.MinValue), this.MaxValue); // this._value = initialValue < min ? min : max < initialValue ? max : initialValue;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 最大値を取得します。
        /// </summary>
        /// <value>
        /// 値を表す <see cref="int" /> 型。
        /// <para>最大値。既定値は <see cref="int.MaxValue" /> です。</para>
        /// </value>
        public int MaxValue { get; }

        /// <summary>
        /// 最小値を取得します。
        /// </summary>
        /// <value>
        /// 値を表す <see cref="int" /> 型。
        /// <para>最小値。既定値は <see cref="int.MinValue" /> です。</para>
        /// </value>
        public int MinValue { get; }

        /// <summary>
        /// 現在値を取得します。
        /// </summary>
        /// <value>
        /// 値を表す <see cref="int" /> 型。
        /// <para>現在値。既定値は 0 です。</para>
        /// </value>
        public int Value
        {
            get => this._value;
            private set => this.SetProperty(ref this._value, value);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// インクリメントして現在値を発行します。
        /// </summary>
        public void Increment()
        {
            int nextValue;
            lock (this._gate)
            {
                // Min ≦ value ≦ Max の値域でインクリメントします。
                nextValue = unchecked(this._value + 1);
                if (this.MaxValue < nextValue)
                {
                    nextValue = this.MinValue;
                }

                // 現在値を更新し、現在値を発行します。
                this.Value = nextValue;
            }

            this._trigger.OnNext(nextValue);
        }

        /// <summary>
        /// オブザーバーが通知を受け取ることをプロバイダーに通知します。
        /// </summary>
        /// <param name="observer">通知を受け取るオブジェクト。</param>
        /// <returns>プロバイダーが通知の送信を完了する前に、オブザーバーが通知の受信を停止できるインターフェイスへの参照。</returns>
        public IDisposable Subscribe(IObserver<int> observer)
        {
            return this._trigger.Subscribe(observer);
        }

        #endregion
    }
}
