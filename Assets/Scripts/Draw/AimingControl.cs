using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingControl : MonoBehaviour
{
    DrawMove drawMove;

    int collisionCount = 0;

    void Start()
    {
        drawMove = transform.parent.GetComponent<DrawMove>();
    }

    void Update()
    {
        //Debug.Log("collisionCount: " + collisionCount);
        drawMove.SetObstacle(collisionCount != 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Stroke"))
            ++collisionCount;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Stroke"))
            --collisionCount;
    }
}
