using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBrushType : MonoBehaviour
{
    public float colorFactor;

    public Slider paintVolumeSlider;
    public GameObject paintVolumeCutPoint;
    public Image volumeBar;
    public Image brushIcon;

    RectTransform brushIconTransform;
    Vector2 brushIconSize;

    void Start()
    {
        brushIconTransform = brushIcon.GetComponent<RectTransform>();
        brushIconSize = brushIconTransform.sizeDelta;
    }

    public void Initialize(float maxVolume, float cutValue1, float cutValue2, BrushType.Type type)
    {
        CreateSliderIcon(maxVolume, cutValue1, cutValue2);
        UpdateColor(type);
    }
    void CreateSliderIcon(float maxVolume, float cutValue1, float cutValue2)
    {
        float w = paintVolumeSlider.GetComponent<RectTransform>().sizeDelta.x;

        GameObject temp = Instantiate(paintVolumeCutPoint, paintVolumeSlider.transform, false);
        temp.transform.position += -w * cutValue1 * Vector3.right;

        temp = Instantiate(paintVolumeCutPoint, paintVolumeSlider.transform, false);
        temp.transform.position += -w * cutValue2 * Vector3.right;
    }

    public void UpdatePaintVolume(float value)
    {
        paintVolumeSlider.value = value;
    }

    public void UpdateColor(BrushType.Type type)
    {
        volumeBar.color = BrushType.GetColor(type) * colorFactor;
        brushIcon.color = BrushType.GetColor(type) * colorFactor;
    }

    public void UpdateBrushType(BrushType.Type type)
    {
        UpdateColor(type);
        GetComponent<Animator>().SetTrigger("SwitchBrush");

        //StartBrushTypeAnimation();
    }

    void StartBrushTypeAnimation()
    {
        brushIconTransform.sizeDelta = brushIconSize * 1.3f;
        //StartCoroutine(BrushTypeAnimation());
    }
}
