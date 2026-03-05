namespace BuildingBlocks.Primitives.Collections;

public static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        public int RemoveAll(Func<T, bool> predicate)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            // Fast paths
            if (collection is List<T> list)
                return list.RemoveAll(new(predicate));

            if (collection is HashSet<T> set)
                return set.RemoveWhere(new(predicate));

            // Generic safe fallback
            var toRemove = collection.Where(predicate).ToList();
            foreach (var item in toRemove)
                collection.Remove(item);

            return toRemove.Count;
        }

        public int AddRange(IEnumerable<T> items)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            if (items is null) throw new ArgumentNullException(nameof(items));

            // HashSet / ISet fast-path: gerçekten eklenen sayıyı döndür
            if (collection is ISet<T> set)
            {
                var before = set.Count;
                set.UnionWith(items);
                return set.Count - before;
            }

            // List fast-path
            if (collection is List<T> list)
            {
                if (items is ICollection<T> ic)
                {
                    list.AddRange(ic);
                    return ic.Count; // List'te "eklenememe" yok; tamamı eklenir
                }

                // IEnumerable tek geçiş: hem ekle hem say
                var count = 0;
                foreach (var item in items)
                {
                    list.Add(item);
                    count++;
                }

                return count;
            }

            // Generic fallback
            var added = 0;
            foreach (var item in items)
            {
                collection.Add(item);
                added++;
            }

            return added;
        }
    }
}