Shader "Custom/AnimatedArray"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "gray" {}
        _SpecularMap("SpecularMap", 2D) = "gray" {}
        _EmissiveMap("EmissiveMap", 2D) = "black" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Subdivisions("Subdivision", Int) = 1
        _FrameAmount("Frame Amount", Int) = 4
        _FrameDelay("Frame Delay", Range(0,100)) = 50
    }
    SubShader
    {
        Tags { "RenderType"="ClipAlpha" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SpecularMap;
        sampler2D _EmissiveMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4 _MainTex_TexelSize;


        int _Subdivisions;
        int _FrameAmount;
        float _FrameDelay;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float2 getOffset(float i)
        {
            int x = i % (_Subdivisions + 1.0f);
            int y = (i - x) * (_Subdivisions + 1.0f);

            return float2(x, y);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float frame = (_Time * 100 / _FrameDelay) % _FrameAmount;
            float2 uv = IN.uv_MainTex / (_Subdivisions + 1.0f);

            uv.xy += getOffset((frame)).yx / (_Subdivisions + 1.0f);

            uv.y += floor(frame* _FrameAmount);

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, uv) * _Color;

            clip(c.a - 0.1f);

            o.Normal = UnpackNormal((tex2D(_NormalMap, uv)));

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = (tex2D(_SpecularMap, uv)).r * _Glossiness;
            o.Alpha = c.a;

            o.Emission = tex2D(_EmissiveMap, uv);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
