using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovementEvent : MonoBehaviour
{
    public Vector3 direction;
    public float distance;
    public float smooth = 0.5f;

    Vector3 initialPos;
    Vector3 targetPos;

    void Start()
    {
        initialPos = transform.position;
        targetPos = initialPos;
        direction = direction.normalized;
    }

    void Update()
    {
        if (transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smooth);
        }
    }

    public void PositionMove(bool back = false)
    {
        if (back) targetPos = initialPos;
        else targetPos = initialPos + direction * distance;
    }
}
