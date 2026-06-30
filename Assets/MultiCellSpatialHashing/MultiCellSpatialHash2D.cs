using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiCellSpatialHashing
{
    public class MultiCellSpatialHash2D<T>
    {
        private readonly float _cellSize;
        private readonly Dictionary<T, GridArea> _occupiedCellsPerObject = new(); // Merge value into Entry/EntryInfo
        private readonly Dictionary<T, Bounds> _boundsPerObject = new(); // Merge value into Entry/EntryInfo 
        private readonly Dictionary<(int, int), Cell<T>> _cells = new();

        public MultiCellSpatialHash2D(float cellSize)
        {
            _cellSize = cellSize;
        }

        public void Clear()
        {
            _occupiedCellsPerObject.Clear();
            _boundsPerObject.Clear();

            foreach (var cell in _cells.Values)
            {
                cell.Clear();
            }
        }

        public void AddObject(T obj, Bounds bounds)
        {
            var area = GetGridArea(bounds);

            if (_occupiedCellsPerObject.TryAdd(obj, area))
            {
                for (var y = area.MinY; y <= area.MaxY; y++)
                {
                    for (var x = area.MinX; x <= area.MaxX; x++)
                    {
                        var cell = GetOrCreateCell(x, y);
                        cell.AddObject(obj);
                    }
                }
                
                _boundsPerObject.Add(obj, bounds);
            }
            else
            {
                throw new Exception("Object already exists, call UpdateObject() instead.");
            }
        }

        public void UpdateObject(T obj, Bounds bounds)
        {
            var oldArea = _occupiedCellsPerObject[obj];
            var newArea = GetGridArea(bounds);
            var hasIntersection = GridArea.Intersects(oldArea, newArea, out var intersection);

            // Exited = old cells except by intersection
            for (var y = oldArea.MinY; y <= oldArea.MaxY; y++)
            {
                for (var x = oldArea.MinX; x <= oldArea.MaxX; x++)
                {
                    if (hasIntersection && intersection.Contains(x, y))
                    {
                        continue;
                    }

                    _cells[(x, y)].RemoveObject(obj);
                }
            }

            // Entered = new cells except by intersection
            for (var y = newArea.MinY; y <= newArea.MaxY; y++)
            {
                for (var x = newArea.MinX; x <= newArea.MaxX; x++)
                {
                    if (hasIntersection && intersection.Contains(x, y))
                    {
                        continue;
                    }

                    var cell = GetOrCreateCell(x, y);
                    cell.AddObject(obj);
                }
            }

            _occupiedCellsPerObject[obj] = newArea;
            _boundsPerObject[obj] = bounds;
        }

        public void RemoveObject(T obj)
        {
            if (_occupiedCellsPerObject.Remove(obj, out var area))
            {
                for (var y = area.MinY; y <= area.MaxY; y++)
                {
                    for (var x = area.MinX; x <= area.MaxX; x++)
                    {
                        var cell = _cells[(x, y)];
                        cell.RemoveObject(obj);
                    }
                }
                
                _boundsPerObject.Remove(obj);
            }
        }

        public void QueryObjects(Bounds bounds, IList<T> result)
        {
            using var pooled = HashSetPool<T>.Get(out var visited);
            var area = GetGridArea(bounds);

            for (var y = area.MinY; y <= area.MaxY; y++)
            {
                for (var x = area.MinX; x <= area.MaxX; x++)
                {
                    var cellId = (x, y);
                    if (_cells.TryGetValue(cellId, out var cell)) // Don't create cells when querying
                    {
                        var objs = cell.GetAllObjects();
                        for (var i = 0; i < objs.Count; i++)
                        {
                            var obj = objs[i];
                            if (visited.Add(obj) && _boundsPerObject[obj].Intersects(bounds))
                            {
                                result.Add(obj);
                            }
                        }
                    }
                }
            }
        }

        public void QueryCells(Bounds bounds, IList<Cell<T>> result)
        {
            var area = GetGridArea(bounds);

            for (var y = area.MinY; y <= area.MaxY; y++)
            {
                for (var x = area.MinX; x <= area.MaxX; x++)
                {
                    var cell = GetOrCreateCell(x, y);
                    result.Add(cell);
                }
            }
        }

        public Cell<T> GetOrCreateCell(int x, int y)
        {
            var cellId = (x, y);

            if (!_cells.TryGetValue(cellId, out var cell))
            {
                _cells[cellId] = cell = new Cell<T>();
            }

            return cell;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GridArea GetGridArea(Bounds bounds)
        {
            var min = bounds.min;
            var max = bounds.max;

            return new GridArea
            {
                MinX = (int)Math.Floor(min.x / _cellSize),
                MaxX = (int)Math.Floor(max.x / _cellSize),
                MinY = (int)Math.Floor(min.y / _cellSize),
                MaxY = (int)Math.Floor(max.y / _cellSize),
            };
        }
    }
}