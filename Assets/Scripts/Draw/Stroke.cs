using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//public class StrokeParticle
//{
//    Stroke stroke;
//    public Vector3 position;
//    public Vector3 forward;
//    public Vector3 right;
//    public Vector3 up;
//    //public Vector3 center;
//    public Vector2 uv1;
//    public Color color;
//    //public bool tag; //notDestoryed/destroyed
//    public bool isBoundary;
//    StrokeCollider collider;
//    int row;
//    int col;

//    public StrokeParticle(Stroke stroke, Vector3 position, Vector3 forward, Vector3 right, int row, int col)
//    {
//        this.stroke = stroke;
//        this.position = position;

//        this.forward = forward;
//        this.right = right;
//        this.up = Vector3.Normalize(Vector3.Cross(forward, right));

//        this.isBoundary = true;

//        this.row = row;
//        this.col = col;

//        //this.tag = true;
//        //this.color = new Color(255, 255, 255, 255);
//    }

//    public void SetCollider(Transform parent, Vector3 center, Quaternion rotation, Vector3 size)
//    {
//        collider = new GameObject().AddComponent<StrokeCollider>();
//        collider.SetCollider(stroke, parent, center, rotation, size, row, col);
//    }
//}

public class RippleControl
{
    Stroke stroke;

    public float rippleTimeInterval = 0.25f;
    float rippleTimer = 0f;

    public float rippleAmplitudeInitial = 0.5f;
    public float rippleThreshold = 0.001f;
    public float rippleDecay = 0.95f;

    public float rippleRadiusIncrement = 0.05f;

    int rippleNum = 10;
    int rippleCurr = 0;
    float[] rippleAmplitudes;
    float[] rippleRadiuses;

    public RippleControl(Stroke stroke)
    {
        this.stroke = stroke;

        rippleAmplitudes = new float[rippleNum];
        rippleRadiuses = new float[rippleNum];
        for (int i = 0; i < rippleNum; ++i) rippleAmplitudes[i] = 0f;
    }

    public void Add(float velocity, int row, int col)
    {
        if (rippleTimer > 0) return;
        rippleTimer = rippleTimeInterval;

        rippleCurr = (rippleCurr + 1) % rippleNum;
        rippleAmplitudes[rippleCurr] = rippleAmplitudeInitial;

        stroke.mr.material.SetFloat("_RippleU" + (rippleCurr + 1), stroke.particles[row][col].uv1.x);
        stroke.mr.material.SetFloat("_RippleV" + (rippleCurr + 1), stroke.particles[row][col].uv1.y);
        stroke.mr.material.SetFloat("_RippleAmplitude" + (rippleCurr + 1), rippleAmplitudes[rippleCurr]);

        //mr.material.SetVector("_Ripple1", new Vector4(particles[row][col].uv1.x, particles[row][col].uv1.y, amplitude, wavelength));
    }

    public void Update()
    {
        if (rippleTimer > 0) rippleTimer -= Time.deltaTime;

        for (int i = 0; i < rippleNum; ++i)
        {
            if (rippleAmplitudes[i] >= rippleThreshold)
            {
                rippleRadiuses[i] += rippleRadiusIncrement;
                rippleAmplitudes[i] *= rippleDecay;
            }
            else
            {
                rippleRadiuses[i] = 0;
                rippleAmplitudes[i] = 0;
            }
            stroke.mr.material.SetFloat("_RippleRadius" + (i + 1), rippleRadiuses[i]);
            stroke.mr.material.SetFloat("_RippleAmplitude" + (i + 1), rippleAmplitudes[i]);
        }
        //mr.material.SetFloat("_RippleAmplitude" + 1, rippleAmplitudeInitial);
        //mr.material.SetFloat("_RippleAmplitude" + 3, rippleAmplitudeInitial);
    }

}

public class Stroke : MonoBehaviour
{
    public MeshFilter mf;
    public MeshRenderer mr;
    public Mesh mesh;

    float colormixFactor = 0.3f;
    bool isDirty = false;

    public List<List<StrokeParticle>> particles;

    BrushType.Type brushType;

    public RippleControl rippleControl;

    public void SetStroke(Transform parent, BrushType.Type type)
    {
        gameObject.name = "Stroke";
        gameObject.tag = "Stroke";
        gameObject.layer = LayerMask.NameToLayer("Stroke");
        gameObject.transform.parent = parent;

        mf = gameObject.AddComponent<MeshFilter>();
        mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = new Material(BrushType.GetMaterial(type));
        mesh = new Mesh();
        mf.sharedMesh = mesh;

        gameObject.AddComponent<Rigidbody>().isKinematic = true;

        particles = new List<List<StrokeParticle>>();

        brushType = type;

        rippleControl = new RippleControl(this);
    }

    public bool Empty()
    { return particles.Count <= 1; }

    List<StrokeParticle> CreateStrokeParticles(List<Vector3> positions, List<Vector3> forwards, List<Vector3> rights)
    {
        List<StrokeParticle> particleRow = new List<StrokeParticle>();
        StrokeParticle particle;
        Color color = BrushType.GetColor(brushType);
        for (int i = 0; i < positions.Count; ++i)
        {
            particle = new GameObject().AddComponent<StrokeParticle>();
            particle.Initialize(this, positions[i], forwards[i], rights[i], particles.Count, i, color);
            particleRow.Add(particle);
        }
        return particleRow;
    }

    void ResetLastBoundary()
    {
        int i = particles.Count - 1;
        if (i <= 0) return;

        for (int j = 1; j < particles[i].Count - 1; ++j)
        {
            particles[i][j].isBoundary = false;
        }
    }

    void SetLastUV1()
    {
        int row = particles.Count;
        int col = particles.Any() ? particles[0].Count : 0;

        int i = row - 1;
        float u;
        float v = i == 0 ? 0f : particles[i - 1][0].uv1.y + Vector3.Distance(particles[i - 1][col / 2].position, particles[i][col / 2].position);

        for (int j = 0; j < particles[i].Count; ++j)
        {
            if (j == 0) u = 0f;
            else if (i == 0) u = particles[i][j - 1].uv1.x + Vector3.Distance(particles[i][j - 1].position, particles[i][j].position);
            else u = particles[i - 1][j].uv1.x;
            particles[i][j].uv1 = new Vector2(u, v);
        }
    }

    List<Vector3> getVertexList()
    {
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < particles.Count; ++i) {
            for (int j = 0; j < particles[i].Count; ++j)  {
                vertices.Add(particles[i][j].position);
            }
        }
        return vertices;
    }

    List<int> getTriangleList()
    {
        List<int> triangles = new List<int>();

        int row = particles.Count;
        int col = particles.Any() ? particles[0].Count : 0;
        int ind0, ind1, ind2, ind3;

        for (int i = 0; i < row - 1; ++i)
        {
            for (int j = 0; j < col - 1; ++j)
            {
                ind0 = col * i + j;
                ind1 = col * (i + 1) + j;
                ind2 = ind1 + 1;
                ind3 = ind0 + 1;
                triangles.Add(ind0); triangles.Add(ind1); triangles.Add(ind2);
                triangles.Add(ind2); triangles.Add(ind3); triangles.Add(ind0);
            }
        }
        return triangles;
    }

    List<Vector2> getUV1List() //actual distance from 0
    {
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < particles.Count; ++i) {
            for (int j = 0; j < particles[i].Count; ++j) {
                uvs.Add(particles[i][j].uv1);
            }
        }
        return uvs;
    }

    List<Vector2> getUV2List() // 0 to 1
    {
        List<Vector2> uvs = new List<Vector2>();
        Vector2 uv = new Vector2(0f,0f);

        for (int i = 0; i < particles.Count; ++i) {
            uv.y = ((float)i) / (particles.Count - 1);
            for (int j = 0; j < particles[i].Count; ++j) {
                uv.x = ((float)j) / (particles[i].Count - 1);
                uvs.Add(uv);
            }
        }

        return uvs;
    }

    List<Vector2> getUV3List() // 0 to 1
    {
        List<Vector2> uvs = new List<Vector2>();
        Vector2 uv = new Vector2(0f, 0f);

        for (int i = 0; i < particles.Count; ++i)
        {
            uv.y = ((float)i) / (particles.Count - 1);
            for (int j = 0; j < particles[i].Count; ++j)
            {
                uv.x = ((float)j) / (particles[i].Count - 1);
                uvs.Add(uv);
            }
        }

        return uvs;
    }
    List<Vector2> getUV4List() // 0 to 1
    {
        List<Vector2> uvs = new List<Vector2>();
        Vector2 uv = new Vector2(0f, 0f);

        for (int i = 0; i < particles.Count; ++i)
        {
            uv.y = ((float)i) / (particles.Count - 1);
            for (int j = 0; j < particles[i].Count; ++j)
            {
                uv.x = ((float)j) / (particles[i].Count - 1);
                uvs.Add(uv);
            }
        }

        return uvs;
    }

    List<Color> getColorList()
    {
        
        List<Color> colors = new List<Color>();

        for (int i = 0; i < particles.Count; ++i) {
            for (int j = 0; j < particles[i].Count; ++j)  {
                colors.Add(particles[i][j].color);
            }
        }
        return colors;
    }

    List<Vector3> getNormalList()
    {
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < particles.Count; ++i)
            for (int j = 0; j < particles[i].Count; ++j)
                normals.Add(particles[i][j].up);

        return normals;
    }

    List<Vector4> getTangentList()
    {
        List<Vector4> tangents = new List<Vector4>();
        Vector4 tangent = new Vector4();

        for (int i = 0; i < particles.Count; ++i)
        {
            for (int j = 0; j < particles[i].Count; ++j)
            {
                tangent = particles[i][j].right;
                tangent.w = 1;
                tangents.Add(tangent);
            }
        }
        return tangents;
    }

    public void DrawStroke(List<Vector3> positions, List<Vector3> forwards, List<Vector3> rights)
    {
        if (particles.Count > 1) ResetLastBoundary();
        particles.Add(CreateStrokeParticles(positions, forwards, rights));
        SetLastUV1();
        if (particles.Count > 1)
        {
            AddColliders();
            SetMesh();
        }
    }

    public void EndStroke()
    {
        MaterialControl.setRenderingMode(mr.material, MaterialControl.BlendMode.Opaque);
        if (brushType == BrushType.Type.Rigid) gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Destroy()
    {
        mr.enabled = false;
        Object.Destroy(this.gameObject);//
    }

    void SetMesh()
    {
        mesh.Clear();
        mesh.SetVertices(getVertexList());
        mesh.SetTriangles(getTriangleList(), 0);
        mesh.SetUVs(0, getUV1List());
        mesh.SetUVs(1, getUV2List());
        mesh.SetUVs(2, getUV3List());
        mesh.SetUVs(3, getUV4List());
        mesh.SetColors(getColorList());
        ////mesh.Optimize(); // MeshUtility.Optimize?
        //mesh.SetNormals(getNormalList());
        mesh.RecalculateNormals();
        ////mesh.RecalculateBounds();?
        mesh.SetTangents(getTangentList());
    }

    void AddColliders()
    {
        int i = particles.Count - 2;
        if (i < 0) return;

        Vector3 forward, right;
        Vector3 center;
        Quaternion rotation;
        Vector3 size;

        for (int j = 0; j < particles[i].Count - 1; ++j)
        {
            forward = Vector3.Lerp(particles[i + 1][j].position, particles[i + 1][j + 1].position, 0.5f) - Vector3.Lerp(particles[i][j].position, particles[i][j + 1].position, 0.5f);
            right = Vector3.Lerp(particles[i][j + 1].position, particles[i + 1][j + 1].position, 0.5f) - Vector3.Lerp(particles[i][j].position, particles[i + 1][j].position, 0.5f);

            center = (particles[i][j].position + particles[i + 1][j].position + particles[i][j + 1].position + particles[i + 1][j + 1].position) / 4;
            rotation = Quaternion.LookRotation(forward, Vector3.Cross(forward, right));
            size = new Vector3(Mathf.Abs(right.magnitude), GetDrawMove().thickness, Mathf.Abs(forward.magnitude));

            particles[i][j].SetCollider(center, rotation, size);
        }
    }

    public void MixColor(int row, int col, Color color)
    {
        particles[row][col].color = Color.Lerp(particles[row][col].color, color, colormixFactor);
        if (row + 1 < particles.Count) particles[row + 1][col].color = Color.Lerp(particles[row + 1][col].color, color, colormixFactor);
        if (row + 1 < particles.Count && col + 1 < particles[row + 1].Count) particles[row + 1][col + 1].color = Color.Lerp(particles[row + 1][col + 1].color, color, colormixFactor);
        if (col + 1 < particles[row].Count) particles[row][col + 1].color = Color.Lerp(particles[row][col + 1].color, color, colormixFactor);

        isDirty = true;
    }

    void UpdateColor()
    { if (isDirty) mesh.SetColors(getColorList()); }

    DrawMove GetDrawMove()
    { return gameObject.GetComponentInParent<DrawMove>(); }

    void Update()
    {
        rippleControl.Update();
        UpdateColor();
    }
}
