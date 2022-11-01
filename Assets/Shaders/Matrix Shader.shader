// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Matrix Shader" 
{
	Properties
	{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct VertexData // appdata
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Interpolators // v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float3 normal : TEXCOORD2;

				UNITY_VERTEX_OUTPUT_STEREO
			};

			struct TriplanarUV
			{
				float2 x;
				float2 y;
				float2 z;
			};

			TriplanarUV GetTriplanarUV(float3 pos)
			{
				TriplanarUV triUV;

				triUV.x = pos.zy; // Use Z and Y for UVs when seen from the X-axis
				triUV.y = pos.xz; // Use X and Z for UVs when seen from the Y-axis
				triUV.z = pos.xy; // Use X and Y for UVs when seen from the Z-axis

				// Animate along V-axis of UV
				triUV.x.y = triUV.x.y - _Time.y * 0.1f; 
				triUV.y.y = triUV.y.y - _Time.y * 0.1f;
				triUV.z.y = triUV.z.y - _Time.y * 0.1f;

				// Scale and tile texture
				triUV.x = triUV.x * _MainTex_ST.xy; 
				triUV.y = triUV.y * _MainTex_ST.xy;
				triUV.z = triUV.z * _MainTex_ST.xy;

				return triUV;
			}

			float3 GetTriplanarWeights(float3 norm)
			{
				float3 triW = abs(norm);
				return triW / (triW.x + triW.y + triW.z); // Create weights from normal values
			}

			// VERTEX
			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;

				UNITY_SETUP_INSTANCE_ID(v);				   // Sets variables for each eye
				UNITY_INITIALIZE_OUTPUT(Interpolators, i); // Initializes all Interpolators values to 0 ???
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(i);  // Tells GPU which eye to render to

				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.worldPosition = v.position;
				i.position = UnityObjectToClipPos(v.position);
				i.normal = v.normal;

				return i;
			}

			// FRAGMENT
			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				TriplanarUV triUV = GetTriplanarUV(i.worldPosition);

				float3 texX = tex2D(_MainTex, triUV.x).rgb;
				float3 texY = tex2D(_MainTex, triUV.y).rgb;
				float3 texZ = tex2D(_MainTex, triUV.z).rgb;

				float3 triW = GetTriplanarWeights(i.normal);

				float3 combinedTex = texX * triW.x + texY * triW.y + texZ * triW.z;
				float4 tex = float4(combinedTex, 1.0f);

				float4 finalColor = tex * _Tint;

				// Apply "random" bright spot effect
				float radius = 4.5f;
				float rho = _Time.w * 0.3f;
				float theta = _Time.w * 0.3f;
				float sphereX = radius * sin(rho) * cos(theta);
				float sphereY = radius * sin(rho) * sin(theta) / 2.0f;
				float sphereZ = radius * cos(rho) - 5.0f;
				float4 spherePos = float4(sphereX, sphereY, sphereZ, 1.0f);

				float4x4 mvpInverse = unity_ObjectToWorld;
				float4 worldPos = mul(mvpInverse, i.worldPosition);
				float d = distance(worldPos, spherePos);
				
				// Set values
				float linearAttenuation = 0.7f;
				float quadraticAttenuation = 1.8f;

				float attenuation = 1.0f / (1.0f + linearAttenuation * d + quadraticAttenuation * d * d);
				float4 spotLight = float4(1.0f, 1.0f, 1.0f, 1.0f) * attenuation;

				// Ambient light
				float4 ambient = float4(0.2f, 0.2f, 0.2f, 0.2f);

				// Diffuse light using camera view instead of light source
				//float3 cameraPos = _WorldSpaceCameraPos;
				//float3 cameraDir = -1.0f * normalize(worldPos.xyz - cameraPos);

				//float4 cameraForward = mul(unity_CameraToWorld, float4(0.0f, 0.0f, 1.0f, 1.0f));
				//float3 cameraDir = cameraForward.xyz;
				
				//float diff = max(dot(i.normal, cameraPos), 0.01f);
				//float4 diffuse = float4(1.0f, 1.0f, 1.0f, 1.0f) * diff;
				float4 diffuse = float4(0.0f, 0.0f, 0.0f, 0.0f);
				
				return (ambient + diffuse + spotLight) * finalColor;
			}

			ENDCG
		}
	}
}