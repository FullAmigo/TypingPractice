#region References

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

#endregion

namespace TypingPracticeApp
{
    public static class DesignerSupport
    {
        /// <summary>プロセスがデザインモードで実行されているかどうかを示す値を表します。</summary>
        public static readonly bool IsInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        /// <summary>
        /// デザインモード以外で実行されている場合に限り例外を発生させます。
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static void ThrowIfNotInDesignMode()
        {
            if (!DesignerSupport.IsInDesignMode)
            {
                throw new InvalidOperationException("デザイン時のみ有効です。");
            }
        }
    }
}
