Shader "Unlit/Plume"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 0, 1, 1)
        _ColorFlame ("Flame Color", Color) = (1, 0, 1, 1)
        _MaxY ("Max", float) = 0
        _MinY ("Min", float) = -5
        _Radius ("Radius", float) = 5
        _OriginOffset ("OriginOffset", float) = 0
        _Resolution ("Resolution", int) = 500
        _RayLength ("Ray Length", float) = 5
        _Flare ("Flare", float) = 0
        _Throttle ("Throttle", Range(0, 1)) = 1
        _Multiplier ("Multiplier", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Off
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
                float3 worldPos : TEXCOORD1;
            };

            float4 _Color;
            float4 _ColorFlame;
            float _MaxY;
            float _MinY;
            float _Radius;
            float _OriginOffset;
            float _RayLength;
            float _Flare;
            float _Throttle;
            float _Multiplier;
            int _Resolution;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            float raycastAlpha(float3 position, float3 direction)
            {
                if (_Throttle == 0)
                {
                    return 0;
                }

                float hits = 0;
                float opaqueness = 0;
                for (int i = 0; i < _Resolution; i++)
                {
                    // float3 local = mul(unity_WorldToObject, position) + mul(unity_WorldToObject, float4(0, 0, 0, 1));
                    float3 local = mul(unity_WorldToObject, position) + mul(unity_WorldToObject, float4(0, 0, 0, 1));
                    // local = mul(UNITY_MATRIX_P, position);
                    if (local.y < _MaxY && local.y > _MinY && distance(local.xz, float2(0, 0)) < _Radius + local.y * _Flare)
                    {
                        opaqueness += abs(_MinY) / abs(local.y - _OriginOffset) * (abs(abs(local.y) - abs(_MinY)) / abs(_MinY));
                    }
                    position += direction * (_RayLength / _Resolution);
                }
                opaqueness /= _Resolution;
                return min(opaqueness * _Throttle * _Multiplier, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDirection = normalize(i.worldPos - _WorldSpaceCameraPos);
                float3 worldPosition = i.worldPos;
                float alpha = raycastAlpha(worldPosition, viewDirection);
                fixed4 col = lerp(_Color, _ColorFlame, alpha);
                col.a = alpha;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

            ENDHLSL
        }
    }
}
