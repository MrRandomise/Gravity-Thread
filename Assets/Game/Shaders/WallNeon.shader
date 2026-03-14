Shader "GravityThread/WallNeon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.05, 0.05, 0.15, 1)
        _EdgeColor ("Edge Color", Color) = (0.2, 0.4, 1, 1)
        _EdgeIntensity ("Edge Intensity", Range(0, 5)) = 2.0
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

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
            float4 _MainTex_ST;
            fixed4 _BaseColor;
            fixed4 _EdgeColor;
            float _EdgeIntensity;
            float _EdgeWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                // Edge detection based on UV distance from border
                float edgeX = min(i.uv.x, 1.0 - i.uv.x);
                float edgeY = min(i.uv.y, 1.0 - i.uv.y);
                float edge = min(edgeX, edgeY);
                float edgeFactor = 1.0 - smoothstep(0, _EdgeWidth, edge);

                fixed4 col = lerp(_BaseColor, _EdgeColor * _EdgeIntensity, edgeFactor);
                col.a = tex.a;
                return col;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
