using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGroupInfo
{
    public int index;
    public Color color;
    public string tagName;

    public TriggerGroupInfo(int index, Color color)
    {
        this.index = index;
        this.color = color;
        tagName = "ObjectFromGenerator";
    }

    public string GetTag()
    {
        return tagName;
    }
}
