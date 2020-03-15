//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class StrokeCollider
//{
//    GameObject gameobject;
//    BoxCollider collider;

//    public StrokeCollider(Transform parent, DrawMove drawMove)
//    {
//        gameobject = new GameObject();
//        gameobject.name = "StrokeCollider";
//        gameobject.tag = "Stroke";//
//        gameobject.layer = LayerMask.NameToLayer("Stroke");
//        gameobject.transform.parent = parent;
        
//        collider = gameobject.AddComponent<BoxCollider>();

//        SetTransform(drawMove);
//    }

//    void SetTransform(DrawMove drawMove)
//    {
//        gameobject.transform.position = drawMove.GetCenter();
//        gameobject.transform.rotation = drawMove.GetRotation();
//        collider.size = drawMove.GetSize(true);
//    }
//}

//public class Stroke
//{
//    public GameObject gameobject;

//    public MeshFilter mf;
//    public MeshRenderer mr;
//    public Mesh mesh;

//    public List<Vector3> vertices8P;
//    public List<int> triangles;
//    public List<Vector2> uvs;
//    Vector2[] uv = { new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) };

//    public List<StrokeCollider> colliders;

//    BrushType.Type type;

//    int backlefttop; //vertex index
//    bool front;

//    public Stroke(Transform parent, BrushType.Type type)
//    {
//        gameobject = new GameObject();
//        gameobject.name = "Stroke";
//        gameobject.tag = "Stroke";
//        gameobject.layer = LayerMask.NameToLayer("Stroke");
//        gameobject.transform.parent = parent;

//        mf = gameobject.AddComponent<MeshFilter>();
//        mr = gameobject.AddComponent<MeshRenderer>();
//        mr.material = new Material(BrushType.GetMaterial(type));
//        mesh = new Mesh();
//        mf.sharedMesh = mesh;

//        vertices8P = new List<Vector3>(); //8 points for a segment
//        triangles = new List<int>();
//        uvs = new List<Vector2>();

//        colliders = new List<StrokeCollider>();

//        this.type = type;

//        front = true;
//        backlefttop = 0;
//    }

//    public void DrawStroke(Vector3[] corners)
//    {
//        AddVertices(corners);
//        Add12Triangles();
//        AddUVs();
//        SetMesh();
//        SetCollider();
//        front = false;
//    }

//    public void EndStroke()
//    {
//        MaterialControl.setRenderingMode(mr.material, MaterialControl.BlendMode.Opaque);
//        if (type == BrushType.Type.Rigid) gameobject.AddComponent<Rigidbody>();
//    }

//    public void Destroy()
//    {
//        mr.enabled = false;
//        Object.Destroy(gameobject);//
//    }

//    public bool Empty()
//    { return vertices8P.Count == 0; }

//    void AddVertices(Vector3[] corners)
//    {
//        //problem: pass by reference?
//        int last4 = 4;

//        //start
//        if (front)
//        {
//            vertices8P.Add(corners[0]);
//            vertices8P.Add(corners[1]);
//            vertices8P.Add(corners[2]);
//            vertices8P.Add(corners[3]);
//        }
//        //top
//        vertices8P.Add(corners[last4]);
//        vertices8P.Add(corners[last4 + 1]);
//        vertices8P.Add(corners[0]);
//        vertices8P.Add(corners[1]);
//        //bottom
//        vertices8P.Add(corners[last4 + 3]);
//        vertices8P.Add(corners[last4 + 2]);
//        vertices8P.Add(corners[3]);
//        vertices8P.Add(corners[2]);
//        //left
//        vertices8P.Add(corners[4]);
//        vertices8P.Add(corners[0]);
//        vertices8P.Add(corners[last4 + 2]);
//        vertices8P.Add(corners[2]);
//        //right
//        vertices8P.Add(corners[1]);
//        vertices8P.Add(corners[last4 + 1]);
//        vertices8P.Add(corners[3]);
//        vertices8P.Add(corners[last4 + 3]);
//        //end
//        vertices8P.Add(corners[last4 + 1]);
//        vertices8P.Add(corners[4]);
//        vertices8P.Add(corners[last4 + 3]);
//        vertices8P.Add(corners[last4 + 2]);
//    }

//    public Vector3 GetCenter(int i, int j) //
//    { return ((vertices8P[i] + vertices8P[i + 1] + vertices8P[i + 2] + vertices8P[i + 3]) + (vertices8P[j] + vertices8P[j + 1] + vertices8P[j + 2] + vertices8P[j + 3])) / 8; }
//    public Vector3 GetCenter(int i) //
//    { return (vertices8P[i] + vertices8P[i + 1] + vertices8P[i + 2] + vertices8P[i + 3]) / 4; }

//    bool CheckSide(int i, Vector3 center) //
//    {
//        Vector3 surfaceCenter = GetCenter(i);
//        return Vector3.Angle(center - surfaceCenter, Vector3.Cross(vertices8P[i] - vertices8P[i + 1], surfaceCenter - vertices8P[i + 1])) > 90;
//    }

//    void Add2Triangles(int start, bool reverse)
//    {
//        triangles.Add(start);       triangles.Add(start + (reverse? 2 : 1));    triangles.Add(start + 3);
//        triangles.Add(start + 3);   triangles.Add(start + (reverse ? 1 : 2));   triangles.Add(start);
//    }

//    void ChangeEndTriangle(int start, bool reverse)
//    {
//        int i = triangles.Count - 6;
//        if (i < 0) return;

//        triangles[i] = start;           triangles[i + 1] = start + (reverse ? 2 : 1); triangles[i + 2] = start + 3;
//        triangles[i + 3] = start + 3;   triangles[i + 4] = start + (reverse ? 1 : 2); triangles[i + 5] = start;
//    }

//    void Add12Triangles() //i: vertex index
//    {
//        int i = backlefttop;

//        Vector3 center = GetCenter(i, i + 20);
//        //problem?
//        bool side1 = CheckSide(i, center);
//        bool side2 = CheckSide(i + 4, center);
//        bool side3 = CheckSide(i + 12, center);
//        //bool side4 = false;

//        if (front)
//        {
//            Add2Triangles(i, side1);
//            Add2Triangles(i + 4, side2);
//        }
//        else
//            ChangeEndTriangle(i + 4, side2);
//        Add2Triangles(i + 8, side2);
//        Add2Triangles(i + 12, side3);
//        Add2Triangles(i + 16, side3);
//        Add2Triangles(i + 20, !side1);

//        UpdateBackLeftTop();
//    }

//    void AddUV()
//    {
//        uvs.Add(uv[0]);
//        uvs.Add(uv[1]);
//        uvs.Add(uv[2]);
//        uvs.Add(uv[3]);
//    }
//    void AddUVs()
//    { while (uvs.Count < vertices8P.Count) AddUV(); }

//    void SetMesh()
//    {
//        mesh.Clear();
//        mesh.vertices = vertices8P.ToArray();
//        mesh.triangles = triangles.ToArray();
//        mesh.uv = uvs.ToArray();
//        //mesh.Optimize(); // MeshUtility.Optimize?
//        mesh.RecalculateNormals();
//        //mesh.RecalculateBounds();??
//    }
//    void SetCollider()
//    { colliders.Add(new StrokeCollider(gameobject.transform, GetDrawMove())); }

//    void UpdateBackLeftTop()
//    { backlefttop = vertices8P.Count - 4; }

//    DrawMove GetDrawMove()
//    { return gameobject.GetComponentInParent<DrawMove>(); }
//}

//public class StrokesControl : MonoBehaviour
//{
//    PlayerControl playerControl;
//    ObjectPicker picker;

//    DrawMove drawMove;

//    UIBrushType uiBrushType;

//    public float paintVolumeMax = 100;
//    public float paintVolumeCut1 = 0.5f;
//    public float paintVolumeCut2 = 0.2f;
//    public float paintedVolume;

//    List<Stroke> strokes;
//    BrushType.Type currType;

//    int curr; //stroke index
//    bool isDrawing;

//    void Start()
//    {
//        playerControl = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
//        picker = GameObject.Find("ObjectPicker").GetComponent<ObjectPicker>();
//        uiBrushType = GameObject.Find("UICanvas").GetComponent<UIBrushType>();

//        isDrawing = false;
//        strokes = new List<Stroke>();
//        drawMove = GetComponent<DrawMove>();

//        currType = BrushType.Type.Normal;

//        paintedVolume = 0;
//        uiBrushType.Initialize(paintVolumeMax, paintVolumeCut1, paintVolumeCut2, currType);
//    }

//    void Update()
//    {
//        if (StageControl.state != StageControl.GameState.Play) return;

//        SetBrushType();
//        SetDepth();
//        Draw();
//    }

//    void Draw()
//    {
//        drawMove.UpdateLastMove();
//        drawMove.UpdateDirection();
//        drawMove.UpdateAimTransform();

//        if (Input.GetMouseButtonDown(0) && !drawMove.IfObstacle()) //draw start
//        {
//            isDrawing = true;
//            curr = StartDraw();
//        }
//        else if (isDrawing)
//        {
//            if (Input.GetMouseButtonUp(0)) //draw end
//            {
//                isDrawing = false;
//                EndDraw(curr);
//            }
//            else if (Input.GetMouseButton(0) && drawMove.IfMove()) //draw drag
//            {
//                DragDraw(curr);
//            }
//        }
//        else
//            drawMove.UpdatePrevMove();

//        if (Input.GetMouseButton(1) && picker.hitStroke) //draw cancel
//        {
//            if (isDrawing && System.Object.ReferenceEquals(strokes[strokes.Count - 1].gameobject, picker.hittedObject))
//                isDrawing = false;
//            CancelDraw(picker.hittedObject);
//        }
//    }

//    int StartDraw()
//    {
//        drawMove.ResetDirection();
//        drawMove.UpdateCorners();
//        drawMove.UpdatePrevMove();

//        strokes.Add(new Stroke(transform, currType));
//        return strokes.Count - 1;
//    }
//    void DragDraw(int i)
//    {
//        if (drawMove.IfObstacle()) return;

//        drawMove.UpdateCorners();
//        strokes[curr].DrawStroke(drawMove.corners);
//        UpdatePaintVolume();
//        drawMove.UpdatePrevMove();
//    }
//    void EndDraw(int i)
//    {
//        if (strokes[i].Empty()) CancelDraw(i);
//        else strokes[i].EndStroke();
//    }

//    void CancelDraw(int i)
//    {
//        playerControl.SetIsGrounded(false);
//        strokes[i].Destroy();
//        strokes.RemoveAt(i);
//    }
//    void CancelDraw(GameObject strokeObj)
//    {
//        playerControl.SetIsGrounded(false);

//        for (int i = 0; i < strokes.Count; ++i)
//        {
//            if (System.Object.ReferenceEquals(strokes[i].gameobject, strokeObj))
//            {
//                strokes[i].Destroy();
//                strokes.RemoveAt(i);
//                return;
//            }
//        }
//    }

//    void SetDepth()
//    {
//        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
//            drawMove.SetDepth(true);
//        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
//            drawMove.SetDepth(false);
//    }

//    void SetBrushType()
//    {
//        if (!Input.GetKeyDown(KeyCode.E)) return;
//        currType = BrushType.SetType(currType);
//        uiBrushType.UpdateBrushType(currType);
//    }

//    void UpdatePaintVolume()
//    {
//        paintedVolume += drawMove.GetDistance();
//        uiBrushType.UpdatePaintVolume(GetPaintRemainRatio());
//    }

//    float GetPaintRemainRatio()
//    { return Mathf.Clamp01((paintVolumeMax - paintedVolume) / paintVolumeMax); }

//    public int GetPaintScore()
//    {
//        float ratio = GetPaintRemainRatio();

//        if (ratio >= paintVolumeCut1) return 3;
//        else if (ratio >= paintVolumeCut2) return 2;
//        else if (ratio > 0) return 1;
//        else return 0;
//    }
//}
