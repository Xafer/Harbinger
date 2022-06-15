Shader "Unlit/SkyboxGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorMap ("Colors", 2D) = "white" {}
        _Offset("Offset", Range(0,1)) = 1
        _Scale("Scale", float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
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
            sampler2D _ColorMap;

            float4 _MainTex_ST;

            float _Offset;
            float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float y = i.uv.y;// +sin(_Time * 10);
                //y = floor(y * 8) / 8;
                y = (y + 1) /8 / _Scale + _Offset;

                float colorIndex1 = ((y) * 16);
                float colorIndex2 = ((y) * 16 + 1) % 16;

                float4 colorA;
                float4 colorB;

                float2 colormapUV1;
                colormapUV1.x = (colorIndex1 % 4);
                colormapUV1.y = (colorIndex1 - colormapUV1.x) / 4.0;

                colormapUV1 /= 4;


                colorA = tex2D(_ColorMap, colormapUV1);

                float2 colormapUV2;
                colormapUV2.x = (colorIndex2 % 4);
                colormapUV2.y = (colorIndex2 - colormapUV2.x) / 4.0;

                colormapUV2 /= 4;

                colorB = tex2D(_ColorMap, colormapUV2);

                float gradient = frac(y * 16);

                fixed4 col = (gradient* colorB) + ((1 - gradient) * colorA);

                return col;
            }
            ENDCG
        }
    }
}
