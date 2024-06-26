// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef UNITY_SPRITES_INCLUDED
#define UNITY_SPRITES_INCLUDED

#include "UnityCG.cginc"

#ifdef UNITY_INSTANCING_ENABLED

UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
// SpriteRenderer.Color while Non-Batched/Instanced.
UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
// this could be smaller but that's how bit each entry is regardless of type
UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

#define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
#define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

#endif // instancing

CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
fixed4 _RendererColor;
fixed2 _Flip;
#endif
float _EnableExternalAlpha;
CBUFFER_END

// Material Color.
fixed4 _Color;

struct appdata_t
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
{
    return float4(pos.xy * flip, pos.z, 1.0);
}

uniform float _ScaleX;
uniform float _ScaleY;
uniform float _MaxDistance;

inline float4 Billboard(float4 vertex)
{
    return mul(UNITY_MATRIX_P,
        mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
        + float4(vertex.x, vertex.y, 0.0, 0.0)
        * float4(_ScaleX, _ScaleY, 1.0, 1.0));
}

v2f SpriteVert(appdata_t IN)
{
    v2f OUT;

    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

    float3 worldPos = mul(UNITY_MATRIX_M, IN.vertex).xyz;
    float3 camPos = _WorldSpaceCameraPos; // Camera's world position
    float distance = length(camPos - worldPos);

    // Adjust alpha based on distance
    float alphaMultiplier = saturate(1.0 - (distance - _MaxDistance) / 5.0); // Gradually fade over 5 units

    OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
    OUT.vertex = Billboard(OUT.vertex);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color * _Color * _RendererColor;
    OUT.color.a *= alphaMultiplier; // Apply the alpha fade

#ifdef PIXELSNAP_ON
    OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

    return OUT;
}

sampler2D _MainTex;
sampler2D _AlphaTex;

fixed4 SampleSpriteTexture(float2 uv)
{
    fixed4 color = tex2D(_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
    fixed4 alpha = tex2D(_AlphaTex, uv);
    color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
#endif

    return color;
}

fixed4 SpriteFrag(v2f IN) : SV_Target
{
    fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
    c.rgb *= c.a;
    return c;
}

#endif // UNITY_SPRITES_INCLUDED