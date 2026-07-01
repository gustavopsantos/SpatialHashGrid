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

        public readonly bool Intersects(GridArea other, out GridArea intersection)
        {
            var intersectionMinX = Math.Max(MinX, other.MinX);
            var intersectionMaxX = Math.Min(MaxX, other.MaxX);
            var intersectionMinY = Math.Max(MinY, other.MinY);
            var intersectionMaxY = Math.Min(MaxY, other.MaxY);

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

        public readonly bool Equals(GridArea other)
        {
            return MinX == other.MinX &&
                   MaxX == other.MaxX &&
                   MinY == other.MinY &&
                   MaxY == other.MaxY;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is GridArea other && Equals(other);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(MinX, MaxX, MinY, MaxY);
        }

        public static bool operator ==(GridArea a, GridArea b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridArea a, GridArea b)
        {
            return !a.Equals(b);
        }
    }
}