// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/DiffuseByVertex"
{
    Properties
    {
		_Diffuse ("Diffuse", Color) = (1,1,1,1)
		_Specular ("Specular", Color) = (1,1,1,1)
		_Gloss ("Gloss", Range(8.0, 256.0)) =8.0
	}
	SubShader {
			Pass {  //顶点着色器和片元着色器需要写在Pass语义块中
				Tags{"LightMode" = "ForwardBase"}

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "Lighting.cginc"

				fixed4 _Diffuse;
				fixed4 _Specular;
				float _Gloss;

				struct a2v {
					float4 vertex :	POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {

					float4 pos : SV_POSITION;
					fixed3 color : COLOR;
				};

				//顶点着色器
				v2f vert(a2v v) {
					//float4 vv : SV_POSITION; error
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					// Get ambient information
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

					//
					//fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject)); //mul函数重载类型
					fixed3 worldNormal = normalize(mul( (float3x3)unity_ObjectToWorld, v.normal));

					fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);

					//compute diffuse term
					fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));

					//compute reflect vector
					fixed3 relect = normalize(reflect(-worldLight, worldNormal));

					//compute view vector
					fixed3 view = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex));
					//compute specular term
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(view, relect)), _Gloss);


					o.color = ambient + diffuse + specular;

					return o;
				}

				//fragement shader
				fixed4 frag(float4 pos: SV_POSITION, float3 color : COLOR) : SV_Target{
					return fixed4(color, 1.0);
				}


				ENDCG
			}
    }
    FallBack "Diffuse"
}
