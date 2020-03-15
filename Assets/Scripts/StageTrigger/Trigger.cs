using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject[] objectsWithMaterial;
    public bool pressed;

    TriggerGroupInfo groupInfo;
    MaterialSetting[] ms;

    void Start()
    {
        pressed = false;
    }

    public void SetGroup(TriggerGroupInfo groupInfo)
    {
        this.groupInfo = groupInfo;
        SetMaterial();
    }
    public void SetMaterial()
    {
        ms = new MaterialSetting[objectsWithMaterial.Length];
        for (int i = 0; i < ms.Length; ++i)
        {
            ms[i] = objectsWithMaterial[i].AddComponent<MaterialSetting>();
            ms[i].SetEmission(groupInfo.color);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(groupInfo.GetTag())) return;
        if (groupInfo.index != other.gameObject.GetComponent<TriggerGroupInfoHolder>().groupInfo.index) return;

        pressed = true;
        for (int i = 0; i < ms.Length; ++i)
            ms[i].SetEmission(groupInfo.color);
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(groupInfo.GetTag())) return;
        if (groupInfo.index != other.gameObject.GetComponent<TriggerGroupInfoHolder>().groupInfo.index) return;

        pressed = false;
        for (int i = 0; i < ms.Length; ++i)
            ms[i].SetEmission(groupInfo.color);
    }
}
