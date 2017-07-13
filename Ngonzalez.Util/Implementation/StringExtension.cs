
using System;
using System.Collections.Generic;
using System.Text;

namespace Ngonzalez.Util
{
    public static class StringExtension
    {
        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            for (int i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                    chunkLength = str.Length - i;

                yield return str.Substring(i, chunkLength);
            }
        }

    }
}