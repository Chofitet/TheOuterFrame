Shader"Custom/UI_GlitchTransmission"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Glitch Intensity", Range(0,1)) = 0.4
        _Speed ("Horizontal Speed", Range(0,10)) = 3
        _LineStrength ("Line Strength", Range(0,1)) = 0.1
        _RGBSplit ("RGB Split", Range(0,5)) = 1
        _VerticalScroll ("Vertical Scroll Speed", Range(-5,5)) = 0.5
    }

    SubShader
    {
Lighting Off
Cull Off
ZWrite Off


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Intensity;
            float _Speed;
            float _LineStrength;
            float _RGBSplit;
            float _VerticalScroll;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                            // Desplazamiento vertical de líneas
                float scroll = _Time.y * _VerticalScroll;
                float2 uv = i.uv + float2(0, scroll);

                            // Líneas horizontales móviles
                float linePos = frac(uv.y * 240.0 + _Time.y * _Speed);
                float lineMask = step(0.98, linePos);

                            // Glitch horizontal aleatorio
                float noise = rand(float2(_Time.y * _Speed, uv.y * 10.0));
                float glitchOffset = (noise - 0.5) * _Intensity;
                uv.x += glitchOffset;

                            // Separación RGB
                float2 offset = float2(_RGBSplit * 0.002, 0);
                float3 col;
                col.r = tex2D(_MainTex, uv + offset).r;
                col.g = tex2D(_MainTex, uv).g;
                col.b = tex2D(_MainTex, uv - offset).b;

                            // Añadir líneas de interferencia
                col += lineMask * _LineStrength;

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }

   
}
