using System.Collections.Generic;
using UnityEngine;

namespace Samples.ParticleCollisions
{
    public class CheckParticleCollisionNaive : ICheckParticleCollision
    {
        private readonly IReadOnlyList<Vector2> _positions;

        public CheckParticleCollisionNaive(IReadOnlyList<Vector2> positions)
        {
            _positions = positions;
        }

        public void OnParticlesUpdated()
        {
        }

        public bool IsColliding(int particleIndex)
        {
            var position = _positions[particleIndex];

            for (var i = 0; i < _positions.Count; i++)
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