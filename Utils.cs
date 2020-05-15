using System;
using System.Collections.Generic;

namespace CTDDeath
{
    public static class Utils
    {
        public static void Log(string s)
        {
            NetScriptFramework.Main.Log.AppendLine(s);
        }

        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);
        }
    }
}
