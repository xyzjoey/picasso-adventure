using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject objectPrefab;
    public Vector3 position;
    public Vector3 direction;
    public float beginSpd = 10;

    public Vector3 cheatPos;

    GameObject generatedObj = null;

    TriggerGroupInfo[] groupInfo;

    void Start()
    {
        position = transform.position + position;
        direction = transform.up;
    }

    void Update()
    {
        //cheat
        //if (Input.GetKeyDown("v"))
        //{
        //    Vector3 temp = position;
        //    position = transform.position + cheatPos;
        //    Generate(0);
        //    position = temp;
        //}
    }

    public void Generate(int i)
    {
        if (generatedObj != null) Destroy(generatedObj);

        generatedObj = (GameObject)Instantiate(objectPrefab);
        generatedObj.tag = groupInfo[i].GetTag();
        TriggerGroupInfoHolder infoHolder = generatedObj.AddComponent<TriggerGroupInfoHolder>();
        infoHolder.groupInfo = groupInfo[i];

        MaterialSetting ms = generatedObj.AddComponent<MaterialSetting>();
        ms.SetColor(groupInfo[i].color);
        ms.SetEmission(groupInfo[i].color, 1);

        generatedObj.transform.position = position;
        generatedObj.GetComponent<Rigidbody>().velocity = direction * beginSpd;
    }

    public void SetGroup(TriggerGroupInfo groupInfo, int i)
    {
        this.groupInfo[i] = groupInfo;
    }

    public void SetGroup(int i) //number of switch for this generator
    {
        this.groupInfo = new TriggerGroupInfo[i];
    }

}
