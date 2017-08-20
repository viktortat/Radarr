﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NzbDrone.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();

            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static IEnumerable<TFirst> IntersectBy<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first, Func<TFirst, TKey> firstKeySelector,
                                                                             IEnumerable<TSecond> second, Func<TSecond, TKey> secondKeySelector,
                                                                             IEqualityComparer<TKey> keyComparer)
        {
            var keys = new HashSet<TKey>(second.Select(secondKeySelector), keyComparer);

            foreach (var element in first)
            {
                var key = firstKeySelector(element);

                // Remove the key so we only yield once
                if (keys.Remove(key))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TFirst> ExceptBy<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first, Func<TFirst, TKey> firstKeySelector,
                                                                             IEnumerable<TSecond> second, Func<TSecond, TKey> secondKeySelector,
                                                                             IEqualityComparer<TKey> keyComparer)
        {
            var keys = new HashSet<TKey>(second.Select(secondKeySelector), keyComparer);
            var matchedKeys = new HashSet<TKey>();

            foreach (var element in first)
            {
                var key = firstKeySelector(element);

                if (!keys.Contains(key) && !matchedKeys.Contains(key))
                {
                    // Store the key so we only yield once
                    matchedKeys.Add(key);
                    yield return element;
                }
            }
        }

        public static void AddIfNotNull<TSource>(this List<TSource> source, TSource item)
        {
            if (item == null)
            {
                return;
            }

            source.Add(item);
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static bool NotAll<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.All(predicate);
        }

        public static List<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate)
        {
            return source.Select(predicate).ToList();
        }

        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source, int n)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (n < 0)
                throw new ArgumentOutOfRangeException("n",
                    "Argument n should be non-negative.");

            return InternalDropLast(source, n);
        }

        private static IEnumerable<T> InternalDropLast<T>(IEnumerable<T> source, int n)
        {
            Queue<T> buffer = new Queue<T>(n + 1);

            foreach (T x in source)
            {
                buffer.Enqueue(x);

                if (buffer.Count == n + 1)
                    yield return buffer.Dequeue();
            }
        }
        public static bool In<T>(this T source, List<T> list)
        {
            return list.Contains(source);
        }
    }
}