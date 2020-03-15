using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for testing material in runtime
public class MaterialSetting : MonoBehaviour
{
    public bool mainColor = false;
    public Color color;

    public bool emission = false;
    public Color emissionColor;
    public float emissionIntensity = 3f;

    public Material material;

    void Start()
    {
        //UpdateMaterial();
    }

    void Update()
    {
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        if (material == null) material = GetComponent<Renderer>().material;
        if (mainColor) MaterialControl.SetColor(material, color);
        if (emission) MaterialControl.SetEmissionColor(material, emissionColor, emissionIntensity);
        //else MaterialControl.setColorDisable(material, "_EMISSION");
    }

    public void SetColor(Color color)
    {
        mainColor = true;
        this.color = color;
        UpdateMaterial();
    }

    public void SetEmission(Color color, float intensity = 3f)
    {
        emission = true;
        emissionColor = color;
        emissionIntensity = intensity;
        UpdateMaterial();
    }
}
