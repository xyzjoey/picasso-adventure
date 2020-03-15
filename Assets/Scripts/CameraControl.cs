using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float rotateSpdX = 4;
    public float rotateSpdY = 5;
    public float maxAngle = 65;
    public float minAngle = -70;
    public float positionX = 0;
    public float positionY = 1.48f;
    public float positionZ = -0.18f;

    new Transform transform; //problem: hide inherited member -> need "new"?
    Transform playerTransform;

    float rotateX;
    float rotateY;
    float rotateZ;
    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        playerTransform = GameObject.FindWithTag("Player").transform;

        rotateX = playerTransform.eulerAngles.x;
        rotateY = playerTransform.eulerAngles.y;
        rotateZ = playerTransform.eulerAngles.z;

        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePosition();
        //UpdateRotation();
    }

    void LateUpdate()
    {
        if (StageControl.state != StageControl.GameState.Play) return;

        UpdatePosition();
        UpdateRotation();
    }

    void UpdatePosition()
    {
        position = playerTransform.position;
        position += positionY * playerTransform.up + positionZ * playerTransform.forward;
        transform.position = position;
    }

    void UpdateRotation()
    {
        rotateX -= Input.GetAxis("Mouse Y") * rotateSpdX;
        if (rotateX > maxAngle)
            rotateX = maxAngle;
        else if (rotateX < minAngle)
            rotateX = minAngle;
        rotateY = playerTransform.eulerAngles.y;
        rotateZ = playerTransform.eulerAngles.z;

        transform.eulerAngles = new Vector3(rotateX, rotateY, rotateZ);
    }
}
