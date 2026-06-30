namespace Samples.ParticleCollisions
{
    public interface ICheckParticleCollision
    {
        void OnParticlesUpdated();
        bool IsColliding(int particleIndex);
    }
}