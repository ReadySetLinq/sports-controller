using NUnit.Framework;
using System;
using System.Linq;

namespace SportsController.Shared
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, bool ignoreCase)
        {
            if (ignoreCase)
            {
                try
                {
                    Assert.IsTrue(source.ToUpper().Contains(toCheck.ToUpper()));
                    return true;
                }
                catch { return false; }
            }
            else
                return source.Contains(toCheck);
        }

        public static string[] Wrap(this string text, int max)
        {
            var charCount = 0;
            var lines = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return lines.GroupBy(w => (charCount += (((charCount % max) + w.Length + 1 >= max)
                            ? max - (charCount % max) : 0) + w.Length + 1) / max)
                        .Select(g => string.Join(" ", g.ToArray()))
                        .ToArray();
        }
    }
}
