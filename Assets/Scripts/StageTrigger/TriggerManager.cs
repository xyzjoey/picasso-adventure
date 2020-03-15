using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GenSwitchInd
{
    public int[] switchInd;
    public int Length() { return switchInd.Length; }
    public int this[int i]
    {
        get { return switchInd[i]; }
        set { }
    }
}

public class TriggerManager : MonoBehaviour
{
    ObjectPicker picker;
    Switch pickedSwitch = null;
    public Vector3 pickedSwitchPos;

    int groupNum;
    public Color[] groupColors;
    TriggerGroupInfo[] groupInfos;

    public Switch[] switches; //small protable switch
    public int[] switchGrpInd;

    public ObjectGenerator[] generators;
    public GenSwitchInd[] genSwitchInd;
    //int[] genGrpInd;

    public Trigger[] triggers; //button on the floor
    public int[] triggerGrpInd;

    int[] trigNumInGrp;
    int[] countInGrp;

    public UnityEvent[] grpEventTrue;
    public UnityEvent[] grpEventFalse;

    public UnityEvent allEventTrue;
    public UnityEvent allEventFalse;

    Transform player;

    void Start()
    {
        picker = GameObject.Find("ObjectPicker").GetComponent<ObjectPicker>();
        player = GameObject.FindWithTag("Player").transform;

        if (switches.Length != switchGrpInd.Length) Debug.LogError("switches.Length != switchGrpInd.Length");
        if (generators.Length != genSwitchInd.Length) Debug.LogError("generators.Length != genSwitchInd.Length");
        if (triggers.Length != triggerGrpInd.Length) Debug.LogError("triggers.Length != triggerGrpInd.Length");
        //if (grpEventTrue.Length != groupColors.Length) Debug.LogError("grpEventTrue.Length != groupColors.Length");
        //if (grpEventFalse.Length != groupColors.Length) Debug.LogError("grpEventFalse.Length != groupColors.Length");

        //grpNum, grpInfo
        groupNum = groupColors.Length;
        groupInfos = new TriggerGroupInfo[groupNum];
        for (int i = 0; i < groupNum; ++i) groupInfos[i] = new TriggerGroupInfo(i, groupColors[i]);

        //switch
        for (int i = 0; i < switches.Length; ++i)
        {
            switches[i].index = i;
            switches[i].SetGroup(groupInfos[switchGrpInd[i]]);
        }

        //generators
        for (int i = 0; i < generators.Length; ++i)
        {
            generators[i].SetGroup(genSwitchInd[i].Length());
            for (int j = 0; j < genSwitchInd[i].Length(); ++j)
                generators[i].SetGroup(groupInfos[ switchGrpInd[ genSwitchInd[i][j] ] ], j);
        }

        //triggers
        trigNumInGrp = new int[groupNum];
        countInGrp = new int[groupNum];
        for (int i = 0; i < groupNum; ++i) trigNumInGrp[i] = 0;
        for (int i = 0; i < triggers.Length; ++i)
        {
            triggers[i].SetGroup(groupInfos[triggerGrpInd[i]]);
            ++trigNumInGrp[triggerGrpInd[i]];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("f") && picker.hitSwitch)
            PickSwitch(picker.hittedObject);
        else if (Input.GetKeyDown("f") && pickedSwitch != null)
            CallSwitchEvent(pickedSwitch.index);

        UpdatePickedSwitch();
        CheckTriggers();
    }

    void PickSwitch(GameObject target)
    {
        Switch targetSwitch = target.GetComponent<Switch>();

        if (pickedSwitch != null)
        {
            pickedSwitch.gameObject.layer = LayerMask.NameToLayer("Default");//Physics.DefaultRaycastLayers;
            pickedSwitch.GetComponent<Rigidbody>().isKinematic = false;

            Vector3 pos = targetSwitch.transform.position;
            pos.y = player.position.y + 1;
            pickedSwitch.transform.position = pos;
        }

        pickedSwitch = targetSwitch;
        pickedSwitch.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //Physics.IgnoreRaycastLayer;
        pickedSwitch.GetComponent<Rigidbody>().isKinematic = true;
    }

    void UpdatePickedSwitch()
    {
        if (pickedSwitch == null) return;

        pickedSwitch.transform.position = player.position + (player.right * pickedSwitchPos.x + player.up * pickedSwitchPos.z + player.forward * pickedSwitchPos.z);
        pickedSwitch.transform.eulerAngles = new Vector3(0, player.eulerAngles.y, 0);
    }

    void CallSwitchEvent(int switchIndex)
    {
        for (int i = 0; i < generators.Length; ++i)
            for (int j = 0; j < genSwitchInd[i].Length(); ++j)
                if (genSwitchInd[i][j] == switchIndex)
                {
                    generators[i].Generate(j);
                    break;
                }

        ResetTrigger();
    }

    void ResetTrigger()
    { for (int i = 0; i < triggers.Length; ++i) triggers[i].pressed = false; }

    void CheckTriggers()
    {
        int total = 0;

        for (int i = 0; i < groupNum; ++i) countInGrp[i] = 0; //reset count

        for (int i = 0; i < triggers.Length; ++i) //count
            if (triggers[i].pressed)
            {
                ++countInGrp[triggerGrpInd[i]];
                ++total;
            }

        for (int i = 0; i < groupNum; ++i) //call group event
        {
            if (countInGrp[i] == trigNumInGrp[i])
            { if (i < grpEventTrue.Length) grpEventTrue[i].Invoke(); }
            else
            { if (i < grpEventFalse.Length) grpEventFalse[i].Invoke(); }
        }

        if (total == triggers.Length) allEventTrue.Invoke(); //call event for all
        else allEventFalse.Invoke(); 
    }
}