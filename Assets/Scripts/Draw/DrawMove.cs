using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //

public class DrawMove : MonoBehaviour
{
    public float width = 2f;
    public float thickness = 0.1f;
    public int density = 10;

    public float depth = 2f;
    public float depthDelta = 0.1f;
    public float minDepth = 1.5f;//
    public float maxDepth = 5;//

    public float minDist = 0.2f;
    public float minAngle = 5f;
    public float positionSmooth = 0.3f;
    public float forwardSmooth = 0.3f;
    public float rightSmooth = 0.3f;

    AimingControl aiming;
    public Color colorAvail;
    public Color colorUnavail;

    [HideInInspector] public Vector3 prevMove;
    [HideInInspector] public Vector3 lastMove;
    [HideInInspector] public Vector3 prevForward;
    [HideInInspector] public Vector3 prevRight;
    [HideInInspector] public Vector3 forward;
    [HideInInspector] public Vector3 right;
    [HideInInspector] public Vector3 up;
    [HideInInspector] public Vector3 center;
    //[HideInInspector] public Vector3[] corners;

    new Camera camera;

    bool obstacle;
    RaycastHit hit;

    void Start()
    {
        aiming = transform.Find("AimingControl").GetComponent<AimingControl>();
        camera = Camera.main; //GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        obstacle = false;
        lastMove = camera.transform.position + camera.transform.forward * depth;
        //corners = new Vector3[8];

        ResetDirection();

        AimInitialize();
    }

    public void UpdatePrevMove()
    { prevMove = lastMove; }
    public void UpdateLastMove()
    { lastMove = Vector3.Lerp(lastMove, camera.transform.position + camera.transform.forward * depth, positionSmooth); }

    public void UpdateDirection()
    {
        prevForward = forward;
        prevRight = right;

        forward = Vector3.Normalize(lastMove - prevMove);
        if (Vector3.Angle(prevForward, forward) > 90) forward = -forward;
        forward = Vector3.Normalize(Vector3.Lerp(prevForward, forward, forwardSmooth));

        right = Vector3.Normalize(Vector3.Cross(forward, camera.transform.forward));
        if (Vector3.Angle(prevRight, right) > 90) right = -right;
        right = Vector3.Normalize(Vector3.Lerp(prevRight, right, rightSmooth));

        up = Vector3.Normalize(Vector3.Cross(forward, right));
    }
    public void ResetDirection()//
    {
        forward = Vector3.Normalize(lastMove - prevMove);
        right = Vector3.Normalize(Vector3.Cross(forward, camera.transform.forward));
    }

    public bool IfMove()
    { return Vector3.Distance(lastMove, prevMove) >= minDist || Vector3.Angle(prevRight, right) >= minAngle; }
    public bool IfObstacle()
    { return obstacle; }


    //public Vector3 GetCenter()
    //{ return Vector3.Lerp(prevMove, lastMove, 0.5f); }
    public Quaternion GetRotation()
    { return Quaternion.LookRotation(forward, Vector3.Cross(forward, right)); }
    public Vector3 GetSize(bool zAxis)
    { return new Vector3(width, thickness, (zAxis)? Vector3.Distance(prevMove, lastMove) : 1); }

    public float GetDistance()
    { return Vector3.Distance(prevMove, lastMove); }

    public void SetDepth(bool positive)
    {
        depth += (positive ? 1 : -1) * depthDelta;
        if (depth < minDepth || depth > maxDepth) {
            //sound
            depth = Mathf.Clamp(depth, minDepth, maxDepth);
        }
    }

    public void SetObstacle(bool obstacle)
    {
        if (this.obstacle == obstacle) return;

        //if (!obstacle && Physics.Raycast(prevMove, lastMove - prevMove, (lastMove - prevMove).magnitude))
        //     return;

        this.obstacle = obstacle;
        if (obstacle) MaterialControl.SetEmissionColor(aiming.GetComponent<Renderer>().material, colorUnavail);
        else MaterialControl.SetEmissionColor(aiming.GetComponent<Renderer>().material, colorAvail);
    }

    Vector3 RandomV3(Vector3 min, Vector3 max)
    { return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z)); }

    public List<Vector3> GetDrawPoints()
    {
        List<Vector3> positions = new List<Vector3>();

        int n = density;
        Vector3 start = lastMove - 0.5f * width * right;
        Vector3 end = lastMove + 0.5f * width * right;
        Vector3 delta = (end - start) / (float)(n - 1);

        Vector3 noiseMax = Vector3.Normalize(forward + right) * 0.03f;
        Vector3 noiseMin = noiseMax * -1;

        positions.Add(start + RandomV3(noiseMin, noiseMax));
        for (int i = 0; i < n - 2; ++i) positions.Add(positions.Last() + delta + RandomV3(noiseMin, noiseMax));
        positions.Add(end + RandomV3(noiseMin, noiseMax));

        return positions;
    }

    public List<Vector3> GetDirForwards()
    {
        List<Vector3> forwards = new List<Vector3>();

        for (int i = 0; i < density; ++i)
        {
            forwards.Add(forward);
        }
        return forwards;
    }

    public List<Vector3> GetDirRights()
    {
        List<Vector3> rights = new List<Vector3>();

        for (int i = 0; i < density; ++i)
        {
            rights.Add(right);
        }
        return rights;
    }

    public void UpdateAimTransform()
    {
        aiming.transform.position = lastMove;
        aiming.transform.rotation = GetRotation();
    }
    void AimInitialize()
    {
        aiming.transform.localScale = GetSize(false);
        UpdateAimTransform();
    }
}
