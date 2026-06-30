using UnityEngine;

namespace MultiCellSpatialHashing.PerformanceTests
{
    public class PlayPerfTest : MonoBehaviour
    {
        public const int CellSize = 10;
        public const int Samples = 10_000;
        public const int WorldSize = 1_000; // 1km x 1km
        public const float WorldExtends = (float)WorldSize / 2;

        private readonly MultiCellSpatialHash2D<StationaryObject> _map = new(CellSize);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                for (var i = 0; i < Samples; i++)
                {
                    var obj = GenerateObject();
                    _map.AddObject(obj, obj.Bounds);
                }
            }
        }

        public static StationaryObject GenerateObject()
        {
            // Position
            var x = Random.Range(-WorldExtends, +WorldExtends);
            var y = Random.Range(-WorldExtends, +WorldExtends);
            var position = new Vector2(x, y);

            // Bounds
            var width = Random.Range(0, CellSize * 1.2f); // Let's have some objects bigger than cells
            var height = Random.Range(0, CellSize * 1.2f); // Let's have some objects bigger than cells
            var size = new Vector2(width, height);
            var bounds = new Bounds(position, size);

            // Object
            return new StationaryObject(position, bounds);
        }
    }
}