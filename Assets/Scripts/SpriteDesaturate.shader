Shader "Custom/SpriteDesaturate"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _SaturationAmount ("Saturation Amount", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _SaturationAmount;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, IN.texcoord) * IN.color;
                float grayscale = dot(texCol.rgb, float3(0.299, 0.587, 0.114));
                texCol.rgb = lerp(float3(grayscale, grayscale, grayscale), texCol.rgb, _SaturationAmount);
                return texCol;
            }
            ENDCG
        }
    }
}