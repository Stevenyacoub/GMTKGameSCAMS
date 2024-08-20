Shader "Unlit/PortalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _WaveSpeed ("Wave Speed", Float) = 3
        _WaveLength ("Wave Length", Float) = 5
        _WaveStrength ("Wave Strength", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _WaveStrength;
            float _WaveLength;
            float _WaveSpeed;

            v2f vert (appdata v)
            {
                float4 direction = normalize(v.vertex);
                float4 offset = sin((_Time * _WaveSpeed) + (v.vertex[0] * _WaveLength)) * direction * _WaveStrength;
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}
