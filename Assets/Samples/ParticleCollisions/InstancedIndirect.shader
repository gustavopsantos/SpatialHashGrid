Shader "Custom/InstancedIndirect"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow
        #pragma target 4.5


        // Required for DrawMeshInstancedIndirect
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

        sampler2D _MainTex;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float3> positionBuffer;
        StructuredBuffer<float4> colorBuffer;
        #endif


        half _Glossiness;
        half _Metallic;

        struct Input
        {
            float4 color : COLOR;
        };

        void setup()
        {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

            float3 position = positionBuffer[unity_InstanceID];

            unity_ObjectToWorld = 0;
            unity_ObjectToWorld._m00 = 1;
            unity_ObjectToWorld._m11 = 1;
            unity_ObjectToWorld._m22 = 1;
            unity_ObjectToWorld._m33 = 1;

            unity_ObjectToWorld._m03 = position.x;
            unity_ObjectToWorld._m13 = position.y;
            unity_ObjectToWorld._m23 = position.z;

            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._m03 = -position.x;
            unity_WorldToObject._m13 = -position.y;
            unity_WorldToObject._m23 = -position.z;

            #endif
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 instanceColor = 1;

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            instanceColor = colorBuffer[unity_InstanceID];
            #endif

            fixed4 c = instanceColor;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}