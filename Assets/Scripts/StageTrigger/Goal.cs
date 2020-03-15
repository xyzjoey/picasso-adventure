using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [HideInInspector] public bool triggered;

    void Start()
    {
        triggered = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) triggered = true;
    }
}
