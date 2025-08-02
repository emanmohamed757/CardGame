using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace CardGame.Logic.Helpers
{
    internal static class ExtensionMethods
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();
            int index = list.Count;

            while (--index >= 0)
            {
                int randomIndex = random.Next(index + 1);

                T temp = list[index];
                list[index] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements.");
            }

            TSource lowest = source.First();
            foreach (TSource item in source)
            {
                if (selector(item).CompareTo(selector(lowest)) < 0)
                {
                    lowest = item;
                }
            }

            return lowest;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements.");
            }

            TSource lowest = source.First();
            foreach (TSource item in source)
            {
                if (selector(item).CompareTo(selector(lowest)) > 0)
                {
                    lowest = item;
                }
            }

            return lowest;
        }
    }
}
