using UnityEngine;
using UnityEngine.Rendering;

namespace Samples.ParticleCollisions
{
    public class ParticleRenderer : MonoBehaviour
    {
        private static readonly int PositionBuffer = Shader.PropertyToID("positionBuffer");
        private static readonly int ColorBuffer = Shader.PropertyToID("colorBuffer");
    
        [SerializeField] private Mesh _particleMesh;
        [SerializeField] private Material _particleMaterial;

        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _colorBuffer;
        private ComputeBuffer _argsBuffer;

        public void Init(Vector2[] positions, Color[] colors)
        {
            // Position buffer
            _positionBuffer = new ComputeBuffer(positions.Length, sizeof(float) * 2); // sizeof Vector2
            _positionBuffer.SetData(positions);
            _particleMaterial.SetBuffer(PositionBuffer, _positionBuffer);

            // Color buffer
            _colorBuffer = new ComputeBuffer(colors.Length, sizeof(float) * 4); // sizeof Color
            _colorBuffer.SetData(colors);
            _particleMaterial.SetBuffer(ColorBuffer, _colorBuffer);

            // Args buffer
            var args = new uint[] // Must have at least 20 bytes that is equal to 5 integers 
            {
                _particleMesh.GetIndexCount(0),
                (uint)colors.Length,
                0,
                0,
                0,
            };

            _argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(args);
        }

        public void UpdatePositions(Vector2[] positions)
        {
            _positionBuffer.SetData(positions);
        }
        
        public void UpdateColors(Color[] colors)
        {
            _colorBuffer.SetData(colors);
        }

        public void Render(in Bounds bounds)
        {
            Graphics.DrawMeshInstancedIndirect(
                _particleMesh,
                submeshIndex: 0,
                _particleMaterial,
                bounds,
                _argsBuffer,
                argsOffset: 0,
                properties: null,
                ShadowCastingMode.Off,
                receiveShadows: false,
                layer: 0,
                camera: null,
                LightProbeUsage.Off);
        }

        private void OnDestroy()
        {
            _positionBuffer.Release();
            _colorBuffer.Release();
            _argsBuffer.Release();
        }
    }
}