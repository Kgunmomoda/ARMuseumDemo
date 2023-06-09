﻿//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

Shader "EasyAR/CameraImage_YUV_I420_YV12"
{
    Properties
    {
        _yTexture("Texture", 2D) = "white" {}
        _uTexture("Texture", 2D) = "white" {}
        _vTexture("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _yTexture;
            float4 _yTexture_ST;
            sampler2D _uTexture;
            sampler2D _vTexture;
            float4x4 _DisplayTransform;
            float _RowRatio;
            float _RowMaxY;
            float _RowMaxUV;

            v2f vert(appdata i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.texcoord = TRANSFORM_TEX(i.texcoord, _yTexture);
                o.texcoord = float2(o.texcoord.x, 1.0 - o.texcoord.y);
                o.texcoord = mul(_DisplayTransform, float3(o.texcoord, 1.0f)).xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 texcoordY = float2(clamp(i.texcoord.x * _RowRatio, 0, _RowMaxY), i.texcoord.y);
                float2 texcoordUV = float2(clamp(i.texcoord.x * _RowRatio, 0, _RowMaxUV), i.texcoord.y);

                const float4x4 ycbcrToRGBTransform = float4x4(
                    float4(1.0, +0.0000, +1.4020, -0.7010),
                    float4(1.0, -0.3441, -0.7141, +0.5291),
                    float4(1.0, +1.7720, +0.0000, -0.8860),
                    float4(0.0, +0.0000, +0.0000, +1.0000)
                    );
                float y = tex2D(_yTexture, texcoordY).a;
                float cb = tex2D(_uTexture, texcoordUV).a;
                float cr = tex2D(_vTexture, texcoordUV).a;
                float4 ycbcr = float4(y, cb, cr, 1.0);
                float4 col = mul(ycbcrToRGBTransform, ycbcr);
#ifndef UNITY_COLORSPACE_GAMMA
                col.xyz = GammaToLinearSpace(col.xyz);
#endif
                return col;
            }
            ENDCG
        }
    }
}
