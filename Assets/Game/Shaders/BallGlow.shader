Shader "GravityThread/BallGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Ball Color", Color) = (0.8, 0.85, 0.9, 1)
        _GlowColor ("Glow Color", Color) = (0.6, 0.8, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2.0
        _FresnelPower ("Fresnel Power", Range(0.5, 5)) = 2.0
        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseAmount ("Pulse Amount", Range(0, 0.5)) = 0.1
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

        // Glow halo pass (additive)
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _PulseSpeed;
            float _PulseAmount;

            v2f vert(appdata v)
            {
                v2f o;
                // Scale up for glow halo
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float4 scaledVertex = v.vertex;
                scaledVertex.xy *= 1.3 * pulse;
                o.vertex = UnityObjectToClipPos(scaledVertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) * 2.0;
                float glow = saturate(1.0 - dist);
                glow = pow(glow, 2.0);
                float alpha = glow * _GlowIntensity * 0.25;
                return fixed4(_GlowColor.rgb * _GlowIntensity, alpha);
            }
            ENDCG
        }

        // Main ball pass
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _FresnelPower;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Simulate fresnel on 2D circle
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) * 2.0;
                float fresnel = pow(dist, _FresnelPower);

                fixed4 col = tex * _Color;
                col.rgb = lerp(col.rgb, _GlowColor.rgb * _GlowIntensity, fresnel * 0.5);

                // Circular mask
                col.a *= saturate(1.0 - dist);

                return col;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
