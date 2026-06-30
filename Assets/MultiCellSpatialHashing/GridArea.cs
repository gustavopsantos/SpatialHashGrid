using System;

namespace MultiCellSpatialHashing
{
    public struct GridArea
    {
        public int MinX; // Cell inclusive
        public int MaxX; // Cell inclusive
        public int MinY; // Cell inclusive
        public int MaxY; // Cell inclusive
        
        public readonly bool Contains(int x, int y)
        {
            return x >= MinX && x <= MaxX &&
                   y >= MinY && y <= MaxY;
        }
        
        public static bool Intersects(GridArea a, GridArea b, out GridArea intersection)
        {
            var intersectionMinX = Math.Max(a.MinX, b.MinX);
            var intersectionMaxX = Math.Min(a.MaxX, b.MaxX);
            var intersectionMinY = Math.Max(a.MinY, b.MinY);
            var intersectionMaxY = Math.Min(a.MaxY, b.MaxY);
            
            if (intersectionMinX <= intersectionMaxX && intersectionMinY <= intersectionMaxY)
            {
                intersection = new GridArea
                {
                    MinX = intersectionMinX,
                    MaxX = intersectionMaxX,
                    MinY = intersectionMinY,
                    MaxY = intersectionMaxY,
                };
                return true;
            }

            intersection = default;
            return false;
        }
    }
}