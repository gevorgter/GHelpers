﻿using System.Collections.Generic;
using System.Linq;

namespace GHelpers
{
    public static class LinqHelper
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> fullList, int batchSize)
        {
            var batch = new List<T>(batchSize);
            int count = 0;
            foreach (var t in fullList)
            {
                batch.Add(t);
                count++;
                if (count == batchSize)
                {
                    yield return batch;
                    count = 0;
                    batch.Clear();
                }
            }
            if (batch.Count != 0)
                yield return batch;
            else
                yield break;
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}
