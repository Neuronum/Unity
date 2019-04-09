Shader "Custom/B-P DiffByPixel"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
		_Specular("Specular", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0, 256.0)) = 8.0
    }
    
	SubShader	{

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
				float3 worldNormal : TEXCOORD0;
				float3 worldVertex : TEXCOORD1;
			};
			
			//vertex shader
			v2f vert(a2v v) {
				v2f o;
				//transfrom to clip space
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldNormal = v.normal;
				//compute view vector
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			//fragment shader
			fixed4 frag(v2f i) : SV_Target{
					// Get ambient term
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				// Get the normal in world space
				//fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldNormal = mul((float3x3)unity_ObjectToWorld, i.worldNormal);
				worldNormal = normalize(worldNormal);

				// Get the light direction in world space
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				// Compute diffuse term
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

				//compute view direction
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldVertex);

				//compute vector h in Blinn-Phong model
				fixed3 h = normalize(viewDir + worldLightDir);

				//compute specular term
				fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, h)), _Gloss);

				fixed3 color = ambient + diffuse + specular;

				return fixed4(color, 1.0);
			}


			ENDCG
		}
	
	}

    FallBack "Diffuse"
}
