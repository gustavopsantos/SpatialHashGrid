using UnityEngine;

namespace MultiCellSpatialHashing
{
    public class StationaryObject
    {
        public readonly Vector2 Position;
        public readonly Bounds Bounds;

        public StationaryObject(Vector2 position, Bounds bounds)
        {
            Position = position;
            Bounds = bounds;
        }
    }
}