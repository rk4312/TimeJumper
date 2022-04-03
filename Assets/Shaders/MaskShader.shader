Shader "Custom/MaskShader"
{
    SubShader{
        Tags{ "Queue" = "Transparent"}

        Pass{
            Blend Zero One
        }
    }
}