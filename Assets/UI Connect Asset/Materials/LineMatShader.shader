﻿// from http://answers.unity.com/answers/804481/view.html

Shader "SolidColorLineAA"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1)
        _Threshold("Threshold", Range(0, 10)) = 1
    }

    SubShader 
    {
        Tags { "RenderType" = "Transparent" }
        
        // Render both front and back facing polygons.
        Cull Off
        // TODO
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        pass
        {
            // Vertex & Fragment shader
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // access shader property in Cg
            fixed4 _Color;
            half   _Threshold;
            
            // vertex input
            struct vertexIn
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            
            // vertex output
            struct vertexOut 
            {
                float4 pos : SV_POSITION;
                float4 uv  : TEXCOORD0;
            };
            
            vertexOut vert (vertexIn v)
            {
                vertexOut o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = float4(v.texcoord.xy, 0, 0);
                return o; 
            }
            
            half4 frag (vertexOut i) : COLOR
            {
                // Shader Parameters
                half4 color      = _Color;
                half amount = _Threshold; // pixels

                half v = frac(i.uv)[1];

                half threshold = fwidth(i.uv.y) * amount; 

                // alpha fade the lower and upper regions (regions set by threshold)
                v = (v < threshold) ? (v/threshold) : (v > (1 - threshold)) ? (1-v) / threshold : 1;

                return half4(color.rgb, v);
            }

            ENDCG
        }
    }

}