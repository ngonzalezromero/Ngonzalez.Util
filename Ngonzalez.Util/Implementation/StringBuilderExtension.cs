
using System;
using System.Text;

namespace Ngonzalez.Util
{
    public static class StringBuilderExtension
    {
        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb.Length != 0)
            {
                int length = 0;
                int num2 = sb.Length;
                while ((sb[length] == ' ') && (length < num2))
                {
                    length++;
                }
                if (length > 0)
                {
                    sb.Remove(0, length);
                    num2 = sb.Length;
                }
                length = num2 - 1;
                while ((sb[length] == ' ') && (length > -1))
                {
                    length--;
                }
                if (length < (num2 - 1))
                {
                    sb.Remove(length + 1, (num2 - length) - 1);
                }
            }
            return sb;
        }
    }
}