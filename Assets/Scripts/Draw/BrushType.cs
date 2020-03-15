using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushType : MonoBehaviour
{
    [SerializeField] static BrushType BT;
    public Material material;
    public Color[] defaultColors; //FF0000, 00D8FF
    public Color[] colors;
    public float alpha;
    public int brushNum;

    public enum Type
    { Normal, Rigid, TypeNum }

    void Awake()
    {
        if (BT != null) GameObject.Destroy(gameObject);
        else BT = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        defaultColors[0] = new Color(1, 0, 0);
        defaultColors[1] = new Color(0, 1, 1);
        colors[0] = new Color(1, 0, 0);
        colors[1] = new Color(0, 1, 1);
        //SetColors(defaultColors);
        brushNum = (int)Type.TypeNum;
        //Save.LoadColor();

        alpha = Mathf.Clamp01(alpha);
    }

    static public Material GetMaterial(Type type)
    {
        Material m = new Material(BT.material);
        Color color = GetColor(type);
        color.a = BT.alpha;
        m.color = color;
        return m;
    }

    static public Color GetColor(Type type)
    { return BT.colors[(int)type]; }

    static public Color[] GetColors()
    { return BT.colors; }

    static public void SetColor(Type type, Color color)
    { BT.colors[(int)type] = color; }

    //static public void SetColors(Color[] src, bool deep = true) //deep copy in C#?
    //{
    //    int minLength = Mathf.Min(BT.colors.Length, src.Length);
    //    if (deep) { for (int i = 0; i < minLength; ++i) { BT.colors[i] = new Color(src[i].r, src[i].g, src[i].b, src[i].a); } }
    //    else { for (int i = 0; i < minLength; ++i) { BT.colors[i] = src[i]; } }
    //}

    static public Type SetType(Type origin)
    { return (Type)(((int)origin + 1) % (int)Type.TypeNum); }
}
