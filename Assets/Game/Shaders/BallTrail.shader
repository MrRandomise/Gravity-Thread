Shader "GravityThread/BallTrail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Trail Color", Color) = (1, 0.8, 0.2, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2.0
        _SpeedFactor ("Speed Factor", Range(0, 1)) = 0.0
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 3.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha One
        Cull Off
        ZWrite Off

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
            float _GlowIntensity;
            float _SpeedFactor;
            float _PulseSpeed;

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

                // Fade along trail length (uv.x = 0 at start, 1 at end)
                float fade = 1.0 - i.uv.x;
                fade = pow(fade, 1.5);

                // Pulse based on speed
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _SpeedFactor * 0.3;

                // Intensity increases with speed
                float intensity = _GlowIntensity * (0.5 + _SpeedFactor * 0.5) * pulse;

                fixed4 col = _Color * i.color * intensity;
                col.a = tex.a * fade * i.color.a;

                return col;
            }
            ENDCG
        }
    }
    Fallback Off
}
