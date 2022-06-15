Shader "Unlit/RadarSurface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightOffset("Height Offset", float) = -5
        _HeightRange("Height Range", float) = 5
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
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
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _HeightOffset;
            float _HeightRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld , v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvOffset = float2(floor(_Time.y*5)/3,floor(i.worldPos.y)) * 5;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.worldPos.xz/3 + uvOffset);
                // apply fog
                
                float heightFactor = (i.worldPos.y + _HeightOffset - _WorldSpaceCameraPos.y)/ _HeightRange;

                float3 heightColor = float3(  clamp(1 -abs(heightFactor+1),0, 1),
                                                    clamp(1 -abs(heightFactor),0, 1),
                                                    clamp(1 -abs(heightFactor-1),0, 1));

                col.rgb *= heightColor;

                clip(max(col.r,max(col.g,col.b)) - 0.1);

                return col;
            }
            ENDCG
        }
    }
}
