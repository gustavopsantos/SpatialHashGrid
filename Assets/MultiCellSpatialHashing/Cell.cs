using System;
using System.Collections.Generic;

namespace MultiCellSpatialHashing
{
    public class Cell<T>
    {
        public readonly int X;
        public readonly int Y;
        public event Action<T> OnObjectEnteredCell;
        public event Action<T> OnObjectLeftCell;
        private readonly SparseSet<T> _objects = new(initialCapacity: 1024);

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Clear()
        {
            _objects.Clear();
        }
        
        public bool AddObject(T obj)
        {
            var added = _objects.Add(obj);

            if (added)
            {
                OnObjectEnteredCell?.Invoke(obj);
            }
            
            return added;
        }

        public bool RemoveObject(T obj)
        {
            var removed = _objects.Remove(obj);

            if (removed)
            {
                OnObjectLeftCell?.Invoke(obj);
            }
            
            return removed;
        }

        public IReadOnlyList<T> GetAllObjects()
        {
            return _objects.Items;
        }
    }
}