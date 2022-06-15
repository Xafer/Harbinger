Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap1("NM1", 2D) = "white" {}
        _NormalMap2("NM2", 2D) = "white" {}
        _HeightMap("Heightmap", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap1;
        sampler2D _NormalMap2;
        sampler2D _HeightMap;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float4 screenPos;
            float eyeDepth;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        sampler2D_float _CameraDepthTexture;
        float4 _CameraDepthTexture_TexelSize;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
            
        void vert(inout appdata_full v, out Input o) {

            float3 world = mul(unity_ObjectToWorld, v.vertex.xyz);

            fixed4 heightMap = tex2Dlod(_HeightMap, float4(world.xz*10 + _Time.x,0,0));

            v.vertex.z += heightMap.r/2;

            UNITY_INITIALIZE_OUTPUT(Input, o);
            COMPUTE_EYEDEPTH(o.eyeDepth);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.worldPos.zx/32 + float2(_Time.x,0)) * _Color;

            float3 normal1 = UnpackNormal(tex2D(_NormalMap1, IN.worldPos.xz*-0.2 + _Time.x));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap2, IN.worldPos.zx*float2(-0.2,0.2) + _Time.x));

            o.Normal = (normal1+normal2)/2;

            float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos));
            float sceneZ = LinearEyeDepth(rawZ);
            float partZ = IN.eyeDepth;

            float fade = (saturate((sceneZ - partZ) / 4));

            o.Metallic = 0;
            o.Smoothness = 1;

            if (frac(fade * 16 - _Time.y) < 0.5 && fade < 0.1)
            {
                fade = 0.2;
                c.rgb = float3(1, 1, 1);
                o.Smoothness = 0;
            }

            o.Albedo = c;
            o.Alpha = _Color.a * fade/2 + 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
