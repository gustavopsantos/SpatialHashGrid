using System.Collections.Generic;
using MultiCellSpatialHashing;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Samples.ParticleCollisions
{
    public class CheckParticleCollisionSpatialHashing : ICheckParticleCollision
    {
        private readonly IReadOnlyList<Vector2> _positions;
        private readonly MultiCellSpatialHash2D<int> _spatialHash;

        public CheckParticleCollisionSpatialHashing(IReadOnlyList<Vector2> positions, Bounds worldBounds)
        {
            _positions = positions;
            _spatialHash = new MultiCellSpatialHash2D<int>(worldBounds, cellSize: 1f);
        
            for (var i = 0; i < _positions.Count; i++)
            {
                var position = _positions[i];
                var bounds = new Bounds(position, Vector2.one);
                _spatialHash.AddObject(i, bounds);
            } 
        }

        public void OnParticlesUpdated()
        {
            for (var i = 0; i < _positions.Count; i++)
            {
                var position = _positions[i];
                var bounds = new Bounds(position, Vector3.one);
                _spatialHash.UpdateObject(i, bounds);
            } 
        }

        public bool IsColliding(int particleIndex)
        {
            var position = _positions[particleIndex];
            var bounds = new Bounds(position, Vector3.one);
            using var pooled = ListPool<int>.Get(out var result);
            _spatialHash.QueryObjects(bounds, result);

            foreach (var i in result)
            {
                if (i == particleIndex) // Dont check collision against self
                {
                    continue;
                }
                
                var otherPosition = _positions[i];
                const float particleRadius = 0.5f; // Fixed radius for sample simplicity
                const float combinedRadius = particleRadius + particleRadius;

                if ((position - otherPosition).sqrMagnitude <= combinedRadius * combinedRadius)
                {
                    return true;
                }
            }
        
            return false;
        }
    }
}