using System;
using System.Collections.Generic;
using System.IO;
using NetScriptFramework.Tools;

namespace CTDDeath
{
    public static class Utils
    {
        private static readonly LogFile LogFile;

        static Utils()
        {
            LogFile = new LogFile("CTD on Death", LogFileFlags.AppendFile | LogFileFlags.AutoFlush);
        }

        public static string ReadWString(this BinaryReader br)
        {
            var prefix = br.ReadUInt16();
            var result = new string(br.ReadChars(prefix));
            return result;
        }

        public static void Log(string s)
        {
            LogFile.AppendLine(s);
        }

        public static void Do<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);
        }
    }
}
