Shader "Custom/TriplanarTrue"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "gray" {}
        _SpecularMap("SpecularMap", 2D) = "gray" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.0
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Offset("Offset",Range(0,1)) = 0.0
        _FlipX("Flip X Texture",Int) = 0
        _FlipY("Flip Y Texture",Int) = 0
        _FlipZ("Flip Z Texture",Int) = 0
        _EmissionAmount("Emission Amount",Range(0,1)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard vertex:vertWorld

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _SpecularMap;

            half _Glossiness;
            half _Metallic;
            float _Offset;
            fixed4 _Color;

            int _UseWorldCoordinates;

            float _Scale;

            float _EmissionAmount;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_INSTANCING_BUFFER_END(Props)

            struct Input {
                float4 worldToTangent0;
                float4 worldToTangent1;
                float4 worldToTangent2;
                float3 worldPos;
                INTERNAL_DATA
            };

            // Vertex program is determined in pragma above
            void vertWorld(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);

                // really shouldn't have to do this, but looks like surface shaders are bugged
                // and aren't passing the world normal to the surface shader with using:
                // struct Input { float3 worldNormal; }
                // luckily we have free float3 so we don't need another interpolator
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                // unity macro for getting the object to tangent space matrix as var "rotation"
                TANGENT_SPACE_ROTATION;

                // calculate the full world to tangent matrix
                float3x3 worldToTangent = mul(rotation, (float3x3)unity_WorldToObject);
                o.worldToTangent0 = float4(worldToTangent[0], worldNormal.x);
                o.worldToTangent1 = float4(worldToTangent[1], worldNormal.y);
                o.worldToTangent2 = float4(worldToTangent[2], worldNormal.z);
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {
                // extract world normal from the unused w component of world to tangent matrix
                float3 worldNormal = float3(IN.worldToTangent0.w, IN.worldToTangent1.w, IN.worldToTangent2.w);
                float3 projNormal = saturate(pow(worldNormal * 1.4, 16));

                float3 localPos = _UseWorldCoordinates != 0 ? IN.worldPos : mul(unity_WorldToObject, float4(IN.worldPos, 1));
                float3 localNormal = mul((float3x3)unity_WorldToObject, worldNormal);

                // "normalize" projNormal x+y+z to equal 1, ensures even blend
                projNormal /= projNormal.x + projNormal.y + projNormal.z;

                // SIDE X
                float xsign = sign(worldNormal.x);
                float2 zy = IN.worldPos.zy * float2(xsign, 1.0);
                float3 xAlbedo = tex2D(_MainTex, zy);
                float3 xSpecular = tex2D(_SpecularMap, zy);
                float3 xNorm = UnpackNormal(tex2D(_NormalMap, zy));

                // TOP / BOTTOM
                float ysign = sign(worldNormal.y);
                float2 zx = IN.worldPos.zx;
                float3 yAlbedo = tex2D(_MainTex, zx);
                float3 ySpecular = tex2D(_SpecularMap, zx);
                float3 yNorm = UnpackNormal(tex2D(_NormalMap, zx));

                // SIDE Z
                float zsign = sign(worldNormal.z);
                float2 xy = IN.worldPos.xy * float2(-zsign, 1.0);
                float3 zAlbedo = tex2D(_MainTex, xy);
                float3 zSpecular = tex2D(_SpecularMap, xy);
                float3 zNorm = UnpackNormal(tex2D(_NormalMap, xy));

                // use "UDN" style normal blending to wrap normal map to surface normal
                // prevents normals from just being on a "box"
                // world normal components are swizzled so their major axis is along "z", normal map-like
                xNorm = normalize(float3(xNorm.xy * float2(xsign, 1.0) + worldNormal.zy, worldNormal.x));
                yNorm = normalize(float3(yNorm.xy + worldNormal.zx, worldNormal.y));
                zNorm = normalize(float3(zNorm.xy * float2(-zsign, 1.0) + worldNormal.xy, worldNormal.z));

                // reorient normal maps to their world axis by swizzling the components
                xNorm = xNorm.zyx; // hackily swizzle channels to match unity "right"
                yNorm = yNorm.yzx; // hackily swizzle channels to match unity "up"
                zNorm = zNorm.xyz; // no swizzle needed!

                // blend normals together
                float3 wNorm = xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;

                // transform world space normals back to tangent space so that the surface shader
                // can transform them back to world space.
                o.Normal = normalize(
                    float3(
                        dot(wNorm, IN.worldToTangent0.xyz),
                        dot(wNorm, IN.worldToTangent1.xyz),
                        dot(wNorm, IN.worldToTangent2.xyz)
                        )
                );

                float3 albedo = xAlbedo * projNormal.x + yAlbedo * projNormal.y + zAlbedo * projNormal.z;
                float3 specular = xSpecular * projNormal.x + ySpecular * projNormal.y + zSpecular * projNormal.z;

                o.Albedo = albedo;// albedo;

                o.Metallic = _Metallic;
                o.Smoothness = specular;
                o.Emission = o.Albedo * _EmissionAmount;
                o.Alpha = 1;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
