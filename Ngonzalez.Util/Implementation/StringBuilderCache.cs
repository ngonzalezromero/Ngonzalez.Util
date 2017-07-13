
using System;
using System.Text;

namespace Ngonzalez.Util
{
    public static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder cachedStringBuilder;

        public static StringBuilder AcquireBuilder()
        {
            StringBuilder result = cachedStringBuilder;
            if (result == null)
            {
                return new StringBuilder();
            }
            result.Clear();
            cachedStringBuilder = null;
            return result;
        }

        public static string GetStringAndReleaseBuilder(StringBuilder sb)
        {
            string result = sb.ToString();
            cachedStringBuilder = sb;
            return result;
        }
    }
}