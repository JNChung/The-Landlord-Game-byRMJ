Shader "Custom/SelectedOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _Selected ("Selected", Range(0, 1)) = 0
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
        fixed4 _OutlineColor;
        float _OutlineWidth;
        float _Selected;
 
        struct Input
        {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
 
            if (_Selected > 0)
            {
                // 计算纹理坐标偏移量
                float2 offset = _OutlineWidth * _OutlineWidth * normalize(IN.uv_MainTex - 0.5);
 
                // 绘制外框
                if (offset.x < 0 || offset.x > _OutlineWidth || offset.y < 0 || offset.y > _OutlineWidth)
                {
                    c = _OutlineColor;
                }
            }
 
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}