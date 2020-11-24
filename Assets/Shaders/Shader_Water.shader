Shader "Custom/Shader_Water"
{
    Properties
    {
        _Tess ("Tessellation", Range (1,32)) = 4
        _TessMinDistance ("Tessellation Min Distance", Float) = 5.0
        _TessMaxDistance ("Tessellation Max Distance", Float) = 15.0
        _Depth ("Depth", Float) = 1.0
        _Strength ("Strength", Range(0, 2)) = 0.0
        _ShallowColor ("Shallow Color", COLOR) = (1, 1, 1, 1)
        _DeepColor ("Deep Color", COLOR) = (1, 1, 1, 1)
        _ShallowSSSColor ("Shallow SSS Color", COLOR) = (1, 1, 1, 1)
        _DeepSSSColor ("Deep SSS Color", COLOR) = (1, 1, 1, 1)
        _MainNormal ("Main Normal", 2D) = "bump" {}
        _SecondNormal ("Second Normal", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 1.0
        _Displacement ("Displacement", Float) = 0.5
        //_WaveDirection ("Wave Direction", Vector) = (1.0, 1.0, 0.0, 0.0)
        //_Wave("Amp, Length, Speed, Steepness", Vector) = (0.0, 0.0, 0.0, 0.0)
        _FoamTexture ("Foam Texture", 2D) = "white" {}
        _FoamStrength ("Foam Strength", Range(0, 10)) = 1.0
        _FoamPower ("Foam Power", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        GrabPass
        {
            "_CameraColorTexture"
        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert tessellate:tess fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.6

        #include "Tessellation.cginc"

        sampler2D _CameraDepthTexture;
        sampler2D _CameraColorTexture;
        sampler2D _MainNormal;
        sampler2D _SecondNormal;
        sampler2D _ColorRampTexture;
        sampler2D _FoamTexture;

        float _Tess;
        float _TessMinDistance;
        float _TessMaxDistance;
        float _Depth;
        float _Strength;
        fixed4 _ShallowColor;
        fixed4 _DeepColor;
        fixed4 _ShallowSSSColor;
        fixed4 _DeepSSSColor;
        float _NormalStrength;
        float _Displacement;
        float _FoamStrength;
        float _FoamPower;

        #define NUM_WAVES 2
        float _WaveDirection[NUM_WAVES * 2];
        float4 _Wave[NUM_WAVES] ;
        //float _WaveDirectionArray[3];

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float3 ApplyGerstnerWave (half2 direction, half amplitude, half length, half speed, half steepness,
                                  float3 position, inout float3 normal, inout float3 tangent, inout float3 binormal)
        {
            //half frequency = sqrt (9.8 * 6.28318 / length);
            half frequency = 2.0 / length;
            half phase = speed * frequency;
            steepness = steepness / (frequency * amplitude * NUM_WAVES);

            half f = frequency * dot (direction, position.xz) + phase * _Time.y;
            half g = steepness * amplitude;

            half wa = frequency * amplitude;
            half s = sin (f);
            half c = cos (f);

            //position += float3 (
            //    g * direction.x * c,
            //    amplitude * s,
            //    g * direction.y * c
            //);

            normal += float3 (
                -direction.x * wa * c,
                -steepness * wa * s,
                -direction.y * wa * c
            );

            tangent += float3 (
            	-steepness * direction.x * direction.y * wa * s,
            	direction.y * wa * c,
            	-steepness * direction.y * direction.y * wa * s
            );

            binormal += float3 (
            	-steepness * direction.x * direction.x * wa * s,
            	direction.x * wa * c,
            	-steepness * direction.x * direction.y * wa * s
            );

            return float3 (
                g * direction.x * c,
                amplitude * s,
                g * direction.y * c
            );
        }

        void vert (inout appdata_full v)
        {
            float3 vertex = v.vertex.xyz;
            float3 vertexOffset = 0;

            float3 normal = 0;
            float3 tangent = 0;
            float3 binormal = 0;

            for (int i = 0; i < NUM_WAVES; i++)
            {
                float2 direction = -normalize (float2 (_WaveDirection[i * NUM_WAVES], _WaveDirection[i * NUM_WAVES + 1]));
                float amplitude = _Wave[i].x;
                float length = _Wave[i].y;
                float speed = _Wave[i].z;
                float steepness = _Wave[i].w;

                vertexOffset += ApplyGerstnerWave (direction, amplitude, length, speed, steepness, vertex, normal, tangent, binormal);
            }

            normal.y = 1.0 - normal.y;
            tangent.z = 1.0 - tangent.z;
            binormal.x = 1.0 - binormal.x;

            v.vertex.xyz = float4 (vertex + vertexOffset, 1.0);
            v.normal = normalize (normal);
            v.tangent.xyz = normalize (tangent);
            //v.binormal.xyz = binormal;
        }

        float4 tess (appdata_full v0, appdata_full v1, appdata_full v2)
        {
            return UnityDistanceBasedTess (v0.vertex, v1.vertex, v2.vertex, _TessMinDistance, _TessMaxDistance, _Tess);
        }

        struct Input
        {
            float2 uv_MainNormal;
            float2 uv_SecondNormal;
            float2 uv_FoamTexture;
            float3 worldPos;
            float4 screenPos;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float cameraDepth01 = Linear01Depth(tex2D(_CameraDepthTexture, screenUV));
            float cameraDepth = cameraDepth01 * _ProjectionParams.z;
            float depth = saturate ((cameraDepth - (IN.screenPos.w + _Depth)) * _Strength);

            float time = _Time.y;
            float2 mainNormalUV = IN.uv_MainNormal + float2 (time / 50.0, time / 50.0);
            float2 secondNormalUV = IN.uv_SecondNormal + float2 (time / -25.0, time / -25.0);
            float3 mainNormal = UnpackNormal (tex2D (_MainNormal, mainNormalUV));
            float3 secondNormal = UnpackNormal (tex2D (_SecondNormal, secondNormalUV));
            float3 normal = normalize (mainNormal + secondNormal);
            float normalStrength = lerp (0, _NormalStrength, depth);
            normal = normalize (float3(normal.xy * normalStrength, lerp (1.0, normal.z, normalStrength)));

            float simpleFresnel = 1.0 - dot (IN.viewDir, normal);

            float colorBlend = saturate (depth);
            fixed4 absorptionColor = tex2D (_ColorRampTexture, float2 (colorBlend, 0.25));// lerp (_ShallowColor, _DeepColor, colorBlend);

            float surfaceDepth01 = IN.screenPos.w * _ProjectionParams.w;
            float2 refractionDepthUV = screenUV + normal * 0.8;
            float refractionDepth = Linear01Depth(tex2D(_CameraDepthTexture, refractionDepthUV));
            float2 refractionUV = (refractionDepth - surfaceDepth01) > 0.0 ? refractionDepthUV : screenUV;
            fixed3 refractionColor = tex2D (_CameraColorTexture, refractionUV);
            fixed3 refraction = refractionColor * absorptionColor.rgb;// *saturate (depth);
            //fixed3 refraction = refractionColor * waterColor.rgb;

            fixed3 sss = tex2D (_ColorRampTexture, float2 (colorBlend, 0.75));//lerp(_ShallowSSSColor, _DeepSSSColor, depth).rgb;

            fixed4 foamColor = tex2D(_FoamTexture, IN.uv_FoamTexture);
            float foamMask = foamColor.a * (1.0 - saturate(pow(depth, _FoamPower) * _FoamStrength));
            fixed3 foam = foamColor.aaa * foamMask;

            fixed3 color = refraction + sss;
            color = lerp(color, foam, foamMask);

            o.Albedo = color;
            o.Normal = normal;
            o.Metallic = 0.0;
            o.Smoothness = simpleFresnel;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
