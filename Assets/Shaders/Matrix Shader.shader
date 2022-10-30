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
				i.position = UnityObjectToClipPos(v.position);
				i.worldPosition = v.position;
				i.normal = v.normal;

				return i;
			}

			// FRAGMENT
			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				//float3 worldPos = mul(unity_ObjectToWorld, i.position);

				TriplanarUV triUV = GetTriplanarUV(i.worldPosition);

				float3 colorX = tex2D(_MainTex, triUV.x).rgb;
				float3 colorY = tex2D(_MainTex, triUV.y).rgb;
				float3 colorZ = tex2D(_MainTex, triUV.z).rgb;

				float3 triW = GetTriplanarWeights(i.normal);

				float3 totColor = colorX * triW.x + colorY * triW.y + colorZ * triW.z;

				return float4(totColor, 1.0f);
			}

			ENDCG
		}
	}
}