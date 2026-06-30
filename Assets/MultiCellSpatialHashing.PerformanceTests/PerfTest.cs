using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

namespace MultiCellSpatialHashing.PerformanceTests
{
    public class PerfTest
    {
        public const int CellSize = 10;
        public const int Samples = 10_000;
        public const int WorldSize = 1_000; // 1km x 1km
        public const float WorldExtends = (float)WorldSize / 2;

        [Test, Performance]
        public void Add_10_000_Objects()
        {
            MultiCellSpatialHash2D<StationaryObject> map = null;

            Measure.Method(() => AddObjects(map))
                .SetUp(Setup)
                .CleanUp(Cleanup)
                .WarmupCount(4)
                .MeasurementCount(4)
                .GC()
                .Run();

            void Setup()
            {
                Random.InitState(0);
                map = new MultiCellSpatialHash2D<StationaryObject>(CellSize);
            }

            void Cleanup()
            {
                map.Clear();
                map = null;
            }
        }

        private static void AddObjects(MultiCellSpatialHash2D<StationaryObject> map)
        {
            for (var i = 0; i < Samples; i++)
            {
                var obj = GenerateObject();
                map.AddObject(obj, obj.Bounds);
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