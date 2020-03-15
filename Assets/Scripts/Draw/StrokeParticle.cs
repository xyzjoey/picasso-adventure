using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrokeParticle : MonoBehaviour
{
    Stroke stroke;
    int row;
    int col;
    
    //particle
    public Vector3 position;
    public Vector3 forward;
    public Vector3 right;
    public Vector3 up; //delete
    public Vector2 uv1;
    public Vector2 uv3; //dx in uv
    public Vector2 uv4; //dz in uv
    public Color color;
    //public bool tag; //notDestoryed/destroyed
    public bool isBoundary = true;
    //public bool isDirty = false;

    //collider
    new BoxCollider collider;
    BoxCollider trigger;

    public void Initialize(Stroke stroke, Vector3 position, Vector3 forward, Vector3 right, int row, int col, Color color)
    {
        this.stroke = stroke;
        this.row = row;
        this.col = col;

        gameObject.name = "StrokeParticle";
        gameObject.tag = "Stroke";
        gameObject.layer = LayerMask.NameToLayer("Stroke");
        gameObject.transform.parent = stroke.transform;

        this.position = position;
        this.forward = forward;
        this.right = right;
        this.up = Vector3.Normalize(Vector3.Cross(forward, right));

        this.color = color;
        //this.tag = true;

        uv3 = new Vector2(Vector3.Project(Vector3.right, right).magnitude, Vector3.Project(Vector3.right, forward).magnitude);
        uv4 = new Vector2(Vector3.Project(Vector3.forward, right).magnitude, Vector3.Project(Vector3.forward, forward).magnitude);
    }

    public void SetCollider(Vector3 center, Quaternion rotation, Vector3 size)
    {
        //set component
        collider = gameObject.AddComponent<BoxCollider>();
        trigger = gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;//

        //transform
        gameObject.transform.position = center;
        gameObject.transform.rotation = rotation;
        collider.size = size;
        trigger.size = size;
    }

    public void CollidePlayer(float velocity)
    { stroke.rippleControl.Add(velocity, row, col); }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjectFromGenerator")) stroke.rippleControl.Add(1, row, col);
        if (other.CompareTag("Stroke")) stroke.MixColor(row, col, other.GetComponent<StrokeParticle>().color);
    }
}

