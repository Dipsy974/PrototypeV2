// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MaskShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 

            struct vertin {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct vertout {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float4 worldpos:TEXCOORD1;
            };

            vertout vert(vertin v) {
                vertout o;
                o.worldpos = (unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                v.uv = v.uv * 2.0 - 1.0;
                v.vertex.x = v.uv.x;
                v.vertex.y = v.uv.y;
                v.vertex.z = 1;

                o.pos = v.vertex;

                return o;
            };

            float4 frag(vertout o) :COLOR{
                return float4(1,0,0,1); 
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
