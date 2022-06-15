Shader "Custom/TriplanarDoubled"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)

        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("NormalMap", 2D) = "gray" {}
        _SpecularMap ("SpecularMap", 2D) = "gray" {}

        _MainTex2 ("Texture2", 2D) = "white" {}
        _NormalMap2 ("NormalMap2", 2D) = "gray" {}
        _SpecularMap2 ("SpecularMap2", 2D) = "gray" {}

        _Glossiness("Smoothness", Range(0,1)) = 0.0
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Offset("Offset",Range(0,1)) = 0.0
        _FlipX("Flip X Texture",Int) = 0
        _FlipY("Flip Y Texture",Int) = 0
        _FlipZ("Flip Z Texture",Int) = 0
        _EmissionAmount("Emission Amount",Range(0,1)) = 0
        _UseWorldCoordinates("Use WorldCoordinate",Int) = 0
        _Scale("Texture Scale", Range(0,10)) = 1
        _Scale2("Texture Scale2", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SpecularMap;

        sampler2D _MainTex2;
        sampler2D _NormalMap2;
        sampler2D _SpecularMap2;

        struct Input
        {
            float3 worldPos;
            float3 viewDir;
            float3 worldNormal; INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        float _Offset;
        fixed4 _Color;

        float _FlipX;
        float _FlipY;
        float _FlipZ;

        int _UseWorldCoordinates;
        
        float _Scale;
        float _Scale2;

        float _EmissionAmount;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldNormal = WorldNormalVector(IN, o.Normal);
            float3 localPos = _UseWorldCoordinates != 0 ? IN.worldPos : mul(unity_WorldToObject, float4(IN.worldPos, 1));
            float3 localNormal = mul((float3x3)unity_WorldToObject, worldNormal);

            float2 texPos;
            bool invertBumpmap = false;

            float3 normalPixel;

            if (abs(localNormal.x) >= max(abs(localNormal.y), abs(localNormal.z)))
            {
                texPos = _FlipX > 0 ? localPos.yz : localPos.zy;
                normalPixel = UnpackNormal((tex2D(_NormalMap, texPos * _Scale)));
                if (_FlipX > 0)normalPixel = float3(-normalPixel.g, -normalPixel.r, normalPixel.b);
                if (localNormal.x > 0)texPos.x *= -1;
            } else if (abs(localNormal.y) >= abs(localNormal.z))
            {
                texPos = _FlipY > 0 ? -localPos.xz : -localPos.zx;
                normalPixel = UnpackNormal((tex2D(_NormalMap, texPos * _Scale)));
                if (_FlipY > 0)normalPixel = float3(-normalPixel.g, -normalPixel.r, normalPixel.b);
                if (localNormal.y < 0)texPos.x *= -1;
            } else {
                texPos = _FlipZ > 0 ? localPos.yx : localPos.xy;
                normalPixel = UnpackNormal((tex2D(_NormalMap, texPos * _Scale)));
                if (_FlipZ > 0)normalPixel = float3(-normalPixel.g, -normalPixel.r, normalPixel.b);
                if (localNormal.z < 0)texPos.x *= -1;
            }

            float2 texPos2 = texPos * _Scale2;

            texPos *= _Scale;

            float4 overlayColor = (tex2D(_MainTex2, texPos2));

            float4 finalColor = overlayColor.a > 0.1f ? overlayColor : (tex2D(_MainTex, texPos) * _Color);

            float4 albedo = finalColor;

            clip(albedo.a - 0.2f);

            o.Albedo = albedo;

            float smoothness = overlayColor.a > 0.1f ? (tex2D(_SpecularMap2, texPos2)).r :(tex2D(_SpecularMap, texPos)).r;

            o.Normal = normalPixel;
            //o.Albedo = normalPixel; float3(1, 1, 1);

            o.Metallic = _Metallic;
            o.Smoothness = smoothness;
            o.Emission = o.Albedo * _EmissionAmount;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
