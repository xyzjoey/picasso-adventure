using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;//
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public int index;
    public TriggerGroupInfo groupInfo;

    public GameObject objectWithMaterial;

    public void SetGroup(TriggerGroupInfo groupInfo)
    {
        this.groupInfo = groupInfo;
        SetMaterial();
    }

    public void SetMaterial()
    {
        MaterialSetting ms = objectWithMaterial.AddComponent<MaterialSetting>();
        ms.SetEmission(groupInfo.color, 1.65f);
    }
}
