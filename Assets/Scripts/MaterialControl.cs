using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialControl : MonoBehaviour
{
    public static MaterialControl MC;

    public enum BlendMode
    { Opaque, Cutout, Fade, Transparent }

    public static void SetColor(Material m, Color color, string property = "_Color")
    {
        m.SetColor(property, color);
    }

    public static void SetEmissionColor(Material m, Color color, float intensity = 1)
    {
        m.EnableKeyword("_EMISSION");
        m.SetColor("_EmissionColor", color * intensity);
    }

    public static void SetEmissionColor(Material m, bool enable)
    {
        if (enable) m.EnableKeyword("_EMISSION");
        else m.DisableKeyword("_EMISSION");
    }

    //public static void SetColorDisable(Material m, string property)
    //{
    //    m.DisableKeyword(property);
    //}

    public static void setRenderingMode(Material m, BlendMode mode)
    {
        switch (mode)
        {
            case BlendMode.Opaque:
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.SetInt("_ZWrite", 1);
                m.DisableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.SetInt("_ZWrite", 1);
                m.EnableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                m.DisableKeyword("_ALPHATEST_ON");
                m.EnableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                m.DisableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHABLEND_ON");
                m.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = 3000;
                break;
        }
    }

    void Awake()
    {
        if (MC != null) GameObject.Destroy(gameObject);
        else MC = this;
        DontDestroyOnLoad(this);
    }
}
