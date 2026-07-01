using System;
using System.Runtime.CompilerServices;
using MultiCellSpatialHashing;
using UnityEngine;
using UnityEngine.Pool;

public class InterestManager : MonoBehaviour
{
    public event Action<Cell<object>> OnCellLoaded;
    public event Action<Cell<object>> OnCellUnloaded;
    
    private const float CellSize = 1f;

    private Vector2 _position;
    private Bounds _loadBounds;
    private Bounds _unloadBounds;

    private readonly Bounds _worldBounds = new(Vector3.zero, Vector3.one * 32);
    private MultiCellSpatialHash2D<object> _spatialHashMap;
    private readonly SparseSet<Cell<object>> _loadedCells = new(initialCapacity: 64);

    private void Start()
    {
        _spatialHashMap = new MultiCellSpatialHash2D<object>(_worldBounds, CellSize);
    }

    private void Update()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var motion = Vector2.ClampMagnitude(input, 1f);
        SetPositionAndRaiseLoadAndUnloadEvents(_position + motion * Time.deltaTime);
    }

    public void SetPositionAndRaiseLoadAndUnloadEvents(Vector2 position)
    {
        _position = position;
        
        // Update bounds
        var x = position.x;
        var y = position.y;
        const float loadBoundsSize = 1.8f;
        const float loadBoundsHalfSize = loadBoundsSize / 2;
        const float unloadBoundsSize = 3.8f;
        const float unloadBoundsHalfSize = unloadBoundsSize / 2;
        _loadBounds.SetMinMax(new Vector2(x - loadBoundsHalfSize, y - loadBoundsHalfSize), new Vector2(x + loadBoundsHalfSize, y + loadBoundsHalfSize));
        _unloadBounds.SetMinMax(new Vector2(x - unloadBoundsHalfSize, y - unloadBoundsHalfSize), new Vector2(x + unloadBoundsHalfSize, y + unloadBoundsHalfSize));

        // Unload cells
        for (var i = _loadedCells.Items.Count - 1; i >= 0; i--)
        {
            var cell = _loadedCells.Items[i];
            var cellBounds = GetCellBounds(cell.X, cell.Y, _spatialHashMap.CellSize);

            if (!cellBounds.Intersects(_unloadBounds))
            {
                _loadedCells.Remove(cell);
                OnCellUnloaded?.Invoke(cell);
            }
        }

        // Load cells
        using var polled = ListPool<Cell<object>>.Get(out var potentialCellsToBeLoaded);
        _spatialHashMap.QueryCells(_loadBounds, potentialCellsToBeLoaded);

        foreach (var cell in potentialCellsToBeLoaded)
        {
            if (_loadedCells.Add(cell))
            {
                OnCellLoaded?.Invoke(cell);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var cell in _loadedCells.Items)
        {
            DrawCell(cell.X, cell.Y, CellSize, Color.yellow);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_loadBounds.center, _loadBounds.size);

        Gizmos.color = Color.dodgerBlue;
        Gizmos.DrawWireCube(_unloadBounds.center, _unloadBounds.size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bounds GetCellBounds(int x, int y, float cellSize)
    {
        var centerX = (x + 0.5f) * cellSize;
        var centerY = (y + 0.5f) * cellSize;
        var center = new Vector2(centerX, centerY);
        return new Bounds(center, Vector2.one * cellSize);
    }

    private static void DrawCell(int x, int y, float cellSize, Color color)
    {
        var bounds = GetCellBounds(x, y, cellSize);
        Gizmos.color = color;
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}