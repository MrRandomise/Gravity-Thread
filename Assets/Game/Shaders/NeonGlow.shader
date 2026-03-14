Shader "GravityThread/NeonGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.2, 0.6, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5
        _GlowWidth ("Glow Width", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        // Pass 0: Glow pass (additive)
        Pass
        {
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowWidth;

            v2f vert(appdata v)
            {
                v2f o;
                // Expand vertices outward for glow
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, float3(0, 0, 1)));
                worldPos += worldNormal * _GlowWidth;
                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float alpha = tex.a * _GlowIntensity * 0.3;
                return fixed4(_GlowColor.rgb * _GlowIntensity, alpha);
            }
            ENDCG
        }

        // Pass 1: Main sprite
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = tex * _Color * i.color;
                // Add subtle glow tint
                col.rgb += _GlowColor.rgb * _GlowIntensity * 0.15;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
