#region References

using System;
using System.Diagnostics;

#endregion

namespace TypingPracticeApp.Domain
{
    public static class DebugLog
    {
        public static void Print(string message)
        {
            Debug.Print($"{DateTime.Now:HH:mm:ss.fff} {message}");
        }
    }
}
