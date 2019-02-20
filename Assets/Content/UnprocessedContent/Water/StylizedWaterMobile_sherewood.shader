// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge

Shader "IW/StylizedWater/StylizedWaterMobile3" {
    Properties {
         _MainTex ("MainTex", 2D) = "white" {}
         _BottomTex ("BottomTex", 2D) = "white" {}

        _WaveSpeed ("Wave speed", Vector) = (1,-1,0.1,0.1)
        _WaveSpeed2 ("Wave speed2", Vector) = (1,-1,0.1,0.1)

        _Exposure ("Exposure", Float) = 0.05
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Background"
            "RenderType"="Opaque"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
            
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D_float _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex;
            uniform sampler2D _BottomTex;

            uniform float2 _MainTex_ST; 
    		uniform float2 _BottomTex_ST;

        
    		uniform half _Exposure;
			uniform float4 _WaveSpeed;
			uniform float4 _WaveSpeed2;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
            };
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.projPos = ComputeScreenPos (o.pos);

                return o;
            }
            
            fixed3 blendNormal(fixed3 base, fixed3 blend) {
                return blend;
            }
			
			fixed3 blendNormal(fixed3 base, fixed3 blend, fixed opacity) {
			
                return (blendNormal(base, blend) * opacity + base * (1.0 - opacity));
            }
            
            float blendAdd(float base, float blend) {
                return min(base+blend,1.0);
            }
            
            float3 blendAdd(float3 base, float3 blend) {
                return min(base+blend,float3(1.0, 1.0, 1.0));
            }
            
            float3 blendAdd(float3 base, float3 blend, float opacity) {
                return (blendAdd(base, blend) * opacity + base * (1.0 - opacity));
            }
            
            float4 frag(VertexOutput i) : COLOR {
            
                float4 mainColor = tex2D(_MainTex, i.uv0);
				half2 texcoordBottomOffset = i.uv0 * _BottomTex_ST.xy;

				fixed4 tex = tex2D(_BottomTex, half2
				(
				    texcoordBottomOffset.x + (_WaveSpeed.x * _Time.x), 
				    texcoordBottomOffset.y + (_WaveSpeed.y * _Time.x))
                );
                
                fixed4 tex2 = tex2D(_BottomTex, half2
				(
				    texcoordBottomOffset.x + (_WaveSpeed2.x * _Time.x), 
				    texcoordBottomOffset.y + (_WaveSpeed2.y * _Time.x))
                );
                
                
                float3 waveColor = blendNormal(tex.rgb, tex2.rgb, tex.r);

                mainColor.rgb = blendAdd(mainColor.rgb, waveColor.rgb, tex2.r) * _Exposure;
                
				return mainColor;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
