using System;
using System.Diagnostics;
using Samples.ParticleCollisions;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleCollisionSimulator : MonoBehaviour
{
    [SerializeField] private uint _particleCount = 1_000;
    [SerializeField] private uint _simulationSize = 10;
    [SerializeField] private ParticleRenderer _particleRenderer;
    [SerializeField] private bool _useNaiveCollision;

    private Bounds _positionBounds;
    private Bounds _volumeBounds;

    private ICheckParticleCollision _checkParticleCollision;

    // Particles
    private Vector2[] _positions;
    private Vector2[] _velocities;
    private Color[] _colors;

    private void Start()
    {
        _positions = new Vector2[_particleCount];
        _velocities = new Vector2[_particleCount];
        _colors = new Color[_particleCount];
        _positionBounds = new Bounds(Vector3.zero, Vector2.one * _simulationSize);
        _volumeBounds = new Bounds(Vector3.zero, Vector2.one * (_simulationSize + 2));

        for (var i = 0; i < _particleCount; i++)
        {
            _positions[i] = GetRandomPositionInsideBounds(_positionBounds);
            _velocities[i] = GetRandomVelocity(1);
            _colors[i] = Random.ColorHSV();
        }

        _particleRenderer.Init(_positions, _colors);

        _checkParticleCollision = _useNaiveCollision
            ? new CheckParticleCollisionNaive(_positions)
            : new CheckParticleCollisionSpatialHashing(_positions, _volumeBounds);
    }

    private void Update()
    {
        UpdateParticlePositions();
        UpdateParticleColorsBasedOnCollision();
        _particleRenderer.Render(in _positionBounds);
    }

    private void UpdateParticlePositions()
    {
        for (var i = 0; i < _particleCount; i++)
        {
            ref var position = ref _positions[i];
            ref var velocity = ref _velocities[i];
            position += velocity * Time.deltaTime;
            var boundsMin = _positionBounds.min;
            var boundsMax = _positionBounds.max;

            if (position.x < boundsMin.x)
            {
                velocity.x = +Mathf.Abs(velocity.x);
            }

            if (position.x > boundsMax.x)
            {
                velocity.x = -Mathf.Abs(velocity.x);
            }

            if (position.y < boundsMin.y)
            {
                velocity.y = +Mathf.Abs(velocity.y);
            }

            if (position.y > boundsMax.y)
            {
                velocity.y = -Mathf.Abs(velocity.y);
            }
        }

        _particleRenderer.UpdatePositions(_positions);
        var before = Stopwatch.GetTimestamp();
        _checkParticleCollision.OnParticlesUpdated();
        _updateMapTicks = Stopwatch.GetTimestamp() - before;
    }

    private void UpdateParticleColorsBasedOnCollision()
    {
        var before = Stopwatch.GetTimestamp();
        for (var i = 0; i < _particleCount; i++)
        {
            if (_checkParticleCollision.IsColliding(i))
            {
                _colors[i] = Color.red;
            }
            else
            {
                _colors[i] = Color.black;
            }
        }
        _checkCollisionTicks = Stopwatch.GetTimestamp() - before;
        _particleRenderer.UpdateColors(_colors);
    }

    [SerializeField] private long _checkCollisionTicks;
    [SerializeField] private long _updateMapTicks;

    private static Vector3 GetRandomPositionInsideBounds(in Bounds bounds)
    {
        var x = Random.Range(bounds.min.x, bounds.max.x);
        var y = Random.Range(bounds.min.y, bounds.max.y);
        var z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    private static Vector3 GetRandomVelocity(float range)
    {
        var x = Random.Range(-range, +range);
        var y = Random.Range(-range, +range);
        var z = Random.Range(-range, +range);
        return new Vector3(x, y, z);
    }
}