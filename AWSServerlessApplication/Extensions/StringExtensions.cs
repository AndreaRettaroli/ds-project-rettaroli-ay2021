using System;
using System.Linq;

namespace AWSServerlessApplication.Extensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string s, string separator)
        {
            return s.Split(new[] { separator }, StringSplitOptions.None);
        }

        public static int ToInt(this string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out int i))
            {
                return i;
            }
            return defaultValue;
        }

        public static double ToDouble(this string s, double defaultValue = 0)
        {
            if (double.TryParse(s, out double d))
            {
                return d;
            }
            return defaultValue;
        }

        public static long ToLong(this string s, int defaultValue = 0)
        {
            if (int.TryParse(s, out int i))
            {
                return i;
            }
            return defaultValue;
        }

        public static bool ToBool(this string s, bool defaultValue = false)
        {
            if (bool.TryParse(s, out bool b))
            {
                return b;
            }
            return defaultValue;
        }

        public static DateTime ToDateTime(this string s, DateTime defaultValue = new DateTime())
        {
            if (DateTime.TryParse(s, out DateTime d))
            {
                return d;
            }
            return defaultValue;
        }

        public static string FirstCharToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static int HammingDistance(this string s, string t)
        {
            if (s.Length != t.Length)
            {
                return int.MaxValue;
            }

            return s.ToCharArray()
                .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);
        }
    }
}
