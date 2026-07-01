using System.Collections.Generic;

namespace MultiCellSpatialHashing
{
    public class SparseSet<T>
    {
        private readonly List<T> _items;
        private readonly Dictionary<T, int> _index = new();

        public IReadOnlyList<T> Items => _items;

        public SparseSet()
        {
            _items = new List<T>();
        }

        public SparseSet(int initialCapacity)
        {
            _index = new Dictionary<T, int>(initialCapacity);
            _items = new List<T>(initialCapacity);
        }

        public bool Add(T item)
        {
            var added = _index.TryAdd(item, _items.Count);
        
            if (added)
            {
                _items.Add(item);
            }

            return added;
        }

        public bool Remove(T item)
        {
            var removed = _index.Remove(item, out var index);

            if (removed)
            {
                var lastIndex = _items.Count - 1;
            
                if (index != lastIndex) // Only swap when removing non-tail elements
                {
                    var lastItem = _items[lastIndex];
                    _items[index] = lastItem;
                    _index[lastItem] = index;
                }
            
                _items.RemoveAt(lastIndex);
            }
        
            return removed;
        }
    
        public bool Contains(T item)
        {
            return _index.ContainsKey(item);
        }

        public void Clear()
        {
            _items.Clear();
            _index.Clear();
        }
    }
}