// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

Shader "Sample/RecordingWatermark" {
        Properties {
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _Logo("Logo (RGBA)", 2D) = "white" {}
            _X("X (0, 1)", Range(0, 1)) = 0.8
            _Y("Y (0, 1)", Range(0, 1)) = 0.8
            _W("W (0, 1)", Range(0.01, 1)) = 0.15
            _H("H (0, 1)", Range(0.01, 1)) = 0.15
        }
        SubShader {
        Pass {
            Tags { "RenderType" = "Transparents" }

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Logo;
            float4 _Logo_ST;

            float _X;
            float _Y;
            float _W;
            float _H;

            struct v2f {
                float4 pos:SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : COLOR0 {

                float x = 0.0;
                float y = 0.0;
                if (_ScreenParams.y >= _ScreenParams.x)
                {
                    x = i.uv.x - _X;
                    y = i.uv.y - _Y * (_ScreenParams.x / _ScreenParams.y);
                    x = x / _W;
                    y = y / (_H * _ScreenParams.x / _ScreenParams.y);
                }
                else
                {
                    x = i.uv.x - _X * (_ScreenParams.y / _ScreenParams.x);
                    y = i.uv.y - _Y;
                    x = x / (_W * _ScreenParams.y / _ScreenParams.x);
                    y = y / _H ;
                }

                fixed4 color = 0.0;
                if (x > 1 || x < 0 || y < 0 || y > 1)
                {
                    color = tex2D(_MainTex, i.uv.xy);
                }
                else
                {
                    color = tex2D(_Logo, float2(x, y));
                    if (color.a == 0.0)
                    {
                        color = tex2D(_MainTex, i.uv.xy);
                    }
                }
                return color;
            }

            ENDCG
        }
    }
}
