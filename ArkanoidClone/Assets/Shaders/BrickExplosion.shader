// Procedural GPU explosion effect for brick destruction.
// Renders N particles flying outward from center with a short central flash.
// _Progress (0→1) is driven from C# via MaterialPropertyBlock — no gameplay logic here.
Shader "Arkanoid/BrickExplosion"
{
    Properties
    {
        _Color       ("Color",         Color)             = (1, 0.6, 0.1, 1)
        _Progress    ("Progress",      Range(0, 1))       = 0
        _ParticleSize("Particle Size", Range(0.01, 0.15)) = 0.055
    }

    // ── URP SubShader ────────────────────────────────────────────────────────
    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent+1"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector"= "True"
        }

        Pass
        {
            Name "BrickExplosionUnlit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull   Off

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _Progress;
                float _ParticleSize;
            CBUFFER_END

            // ── helpers ──────────────────────────────────────────────────────
            float hash(float n) { return frac(sin(n) * 43758.5453123); }

            float softDisk(float2 uv, float2 center, float radius)
            {
                return 1.0 - smoothstep(radius * 0.4, radius, length(uv - center));
            }

            // ── vertex ───────────────────────────────────────────────────────
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // ── fragment ─────────────────────────────────────────────────────
            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv     = IN.uv;
                float  t      = _Progress;
                float2 center = float2(0.5, 0.5);

                float acc = 0.0;

                // 20 particles, deterministic pseudo-random per index
                UNITY_UNROLL
                for (int p = 0; p < 20; p++)
                {
                    float seed  = float(p) * 1.6180339;
                    float angle = hash(seed)       * 6.28318530;
                    float speed = hash(seed + 3.7) * 0.35 + 0.15;
                    float sz    = _ParticleSize * (hash(seed + 7.3) * 0.5 + 0.75) * (1.0 - t * 0.55);

                    float2 pos = center + float2(cos(angle), sin(angle)) * speed * t;
                    acc += softDisk(uv, pos, sz);
                }

                // Central flash — sharp bright disk that disappears by t = 0.25
                float flashT  = 1.0 - smoothstep(0.0, 0.25, t);
                acc += softDisk(uv, center, 0.38 * flashT) * flashT;

                acc = saturate(acc);

                // Global fade-out in second half of the animation
                float fade = 1.0 - smoothstep(0.45, 1.0, t);
                acc *= fade;

                return half4(_Color.rgb, acc * _Color.a);
            }
            ENDHLSL
        }
    }

    // ── Built-in render pipeline fallback ────────────────────────────────────
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull   Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float  _Progress;
            float  _ParticleSize;

            float hash(float n) { return frac(sin(n) * 43758.5453123); }
            float softDisk(float2 uv, float2 c, float r)
            {
                return 1.0 - smoothstep(r * 0.4, r, length(uv - c));
            }

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f     { float4 pos    : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float  t      = _Progress;
                float2 center = float2(0.5, 0.5);
                float  acc    = 0.0;

                for (int p = 0; p < 20; p++)
                {
                    float seed  = (float)p * 1.6180339;
                    float angle = hash(seed)       * 6.28318530;
                    float speed = hash(seed + 3.7) * 0.35 + 0.15;
                    float sz    = _ParticleSize * (hash(seed + 7.3) * 0.5 + 0.75) * (1.0 - t * 0.55);
                    float2 pos  = center + float2(cos(angle), sin(angle)) * speed * t;
                    acc += softDisk(i.uv, pos, sz);
                }

                float flashT = 1.0 - smoothstep(0.0, 0.25, t);
                acc += softDisk(i.uv, center, 0.38 * flashT) * flashT;
                acc  = saturate(acc);
                acc *= 1.0 - smoothstep(0.45, 1.0, t);

                return fixed4(_Color.rgb, acc * _Color.a);
            }
            ENDCG
        }
    }
}
