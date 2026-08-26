// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/BOXOPHOBIC/Helpers/Channel Preview"
{
	Properties
	{
		_PreviewChannel("PreviewChannel", Float) = 0
		_PreviewLinear("PreviewLinear", Float) = 0
		_PreviewTex("PreviewTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		

		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		

		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			#define ASE_VERSION 19801


			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _PreviewChannel;
			uniform sampler2D _PreviewTex;
			uniform float4 _PreviewTex_ST;
			uniform float _PreviewLinear;


			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}

			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float ifLocalVar15 = 0;
				if( _PreviewChannel == 5.0 )
				ifLocalVar15 = ceil( ( i.ase_texcoord1.xy.x * 4.0 ) );
				else if( _PreviewChannel < 5.0 )
				ifLocalVar15 = _PreviewChannel;
				float Mask19 = ifLocalVar15;
				float2 uv_PreviewTex = i.ase_texcoord1.xy * _PreviewTex_ST.xy + _PreviewTex_ST.zw;
				float4 tex2DNode10 = tex2D( _PreviewTex, uv_PreviewTex );
				float3 linearToGamma11 = LinearToGammaSpace( tex2DNode10.rgb );
				float3 lerpResult13 = lerp( linearToGamma11 , tex2DNode10.rgb , _PreviewLinear);
				float3 ifLocalVar3 = 0;
				if( Mask19 == 0.0 )
				ifLocalVar3 = lerpResult13;
				float4 appendResult14 = (float4(lerpResult13 , tex2DNode10.a));
				float4 break2 = appendResult14;
				float ifLocalVar5 = 0;
				if( Mask19 == 1.0 )
				ifLocalVar5 = break2.x;
				float ifLocalVar6 = 0;
				if( Mask19 == 2.0 )
				ifLocalVar6 = break2.y;
				float ifLocalVar7 = 0;
				if( Mask19 == 3.0 )
				ifLocalVar7 = break2.z;
				float ifLocalVar9 = 0;
				if( Mask19 == 4.0 )
				ifLocalVar9 = break2.w;
				

				finalColor = float4( ( ifLocalVar3 + ifLocalVar5 + ifLocalVar6 + ifLocalVar7 + ifLocalVar9 ) , 0.0 );
				return finalColor;
			}
			ENDCG
		}
	}
	
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TexCoordVertexDataNode;16;-1408,1280;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-1408,0;Inherit;True;Property;_PreviewTex;PreviewTex;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-832,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.LinearToGammaNode;11;-1088,0;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1088,256;Inherit;False;Property;_PreviewLinear;PreviewLinear;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;18;-640,1280;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-640,1152;Inherit;False;Property;_PreviewChannel;PreviewChannel;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;-832,0;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ConditionalIfNode;15;-384,1152;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;5;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;14;-640,128;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-64,1152;Float;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;2;-384,128;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;20;-200.4165,0.01461792;Inherit;False;19;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;6;128,320;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;5;128,160;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;3;128,0;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ConditionalIfNode;9;128,640;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;4;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;7;128,480;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-1184,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1024,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;448,0;Inherit;False;5;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1024,0;Float;False;True;-1;3;;100;5;Hidden/BOXOPHOBIC/Helpers/Channel Preview;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;0;1;False;;1;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;17;0;16;1
WireConnection;11;0;10;5
WireConnection;18;0;17;0
WireConnection;13;0;11;0
WireConnection;13;1;10;5
WireConnection;13;2;12;0
WireConnection;15;0;8;0
WireConnection;15;3;18;0
WireConnection;15;4;8;0
WireConnection;14;0;13;0
WireConnection;14;3;10;4
WireConnection;19;0;15;0
WireConnection;2;0;14;0
WireConnection;6;0;20;0
WireConnection;6;3;2;1
WireConnection;5;0;20;0
WireConnection;5;3;2;0
WireConnection;3;0;20;0
WireConnection;3;3;13;0
WireConnection;9;0;20;0
WireConnection;9;3;2;3
WireConnection;7;0;20;0
WireConnection;7;3;2;2
WireConnection;25;0;16;1
WireConnection;25;1;16;2
WireConnection;26;0;25;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;4;2;6;0
WireConnection;4;3;7;0
WireConnection;4;4;9;0
WireConnection;0;0;4;0
ASEEND*/
//CHKSM=14E7F4788F2F85A1EA428A475AC5D5AED5C53982