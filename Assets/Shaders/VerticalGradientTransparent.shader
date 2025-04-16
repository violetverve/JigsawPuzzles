Shader "Unlit/VerticalGradientTransparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StartColor ("Start Color", Color) = (0, 0, 0, 1)
        _EndColor ("End Color", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _StartColor;
            fixed4 _EndColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float gradientFactor = i.uv.y;
                
                fixed4 gradientColor = lerp(_StartColor, _EndColor, gradientFactor);

                fixed4 texColor = tex2D(_MainTex, i.uv);

                return texColor * gradientColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
