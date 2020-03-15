using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrokesControl : MonoBehaviour
{
    PlayerControl playerControl;
    ObjectPicker picker;

    DrawMove drawMove;

    UIBrushType uiBrushType;

    public float paintVolumeMax = 100;
    public float paintVolumeCut1 = 0.5f;
    public float paintVolumeCut2 = 0.2f;
    public float paintedVolume;

    List<Stroke> strokes;
    BrushType.Type currType;

    int curr; //stroke index
    bool isDrawing;

    void Start()
    {
        playerControl = GameObject.FindWithTag("Player").GetComponent<PlayerControl>();
        picker = GameObject.Find("ObjectPicker").GetComponent<ObjectPicker>();
        uiBrushType = GameObject.Find("UICanvas").GetComponent<UIBrushType>();

        isDrawing = false;
        strokes = new List<Stroke>();
        drawMove = GetComponent<DrawMove>();

        currType = BrushType.Type.Normal;

        paintedVolume = 0;
        uiBrushType.Initialize(paintVolumeMax, paintVolumeCut1, paintVolumeCut2, currType);
    }

    void Update()
    {
        if (StageControl.state != StageControl.GameState.Play) return;

        SetBrushType();
        SetBrushColor();
        SetDepth();
        Draw();
    }

    Stroke CreateStroke()
    {
        GameObject gameobject = new GameObject();
        Stroke stroke = gameobject.AddComponent<Stroke>();
        stroke.SetStroke(transform, currType);
        return stroke;
    }

    void Draw()
    {
        drawMove.UpdateLastMove();
        drawMove.UpdateDirection();
        drawMove.UpdateAimTransform();

        if (Input.GetMouseButtonDown(0) && !drawMove.IfObstacle()) //draw start
        {
            isDrawing = true;
            curr = StartDraw();
        }
        else if (isDrawing)
        {
            if (Input.GetMouseButtonUp(0)) //draw end
            {
                isDrawing = false;
                EndDraw(curr);
            }
            else if (Input.GetMouseButton(0) && drawMove.IfMove()) //draw drag
            {
                DragDraw(curr);
            }
        }
        else
            drawMove.UpdatePrevMove();

        if (Input.GetMouseButton(1) && picker.hitStroke) //draw cancel
        {
            if (isDrawing && System.Object.ReferenceEquals(strokes[strokes.Count - 1].gameObject, picker.hittedObject))
                isDrawing = false;
            CancelDraw(picker.hittedObject);
        }
    }

    int StartDraw()
    {
        drawMove.UpdatePrevMove();

        int last = strokes.Count;
        strokes.Add(CreateStroke());
        strokes[last].DrawStroke(drawMove.GetDrawPoints(), drawMove.GetDirForwards(), drawMove.GetDirRights());
        return last;
    }
    void DragDraw(int i)
    {
        if (drawMove.IfObstacle()) return;

        strokes[curr].DrawStroke(drawMove.GetDrawPoints(), drawMove.GetDirForwards(), drawMove.GetDirRights());
        UpdatePaintVolume();
        drawMove.UpdatePrevMove();
    }
    void EndDraw(int i)
    {
        if (strokes[i].Empty()) CancelDraw(i);
        else strokes[i].EndStroke();
    }

    void CancelDraw(int i)
    {
        playerControl.SetIsGrounded(false);
        strokes[i].Destroy();
        strokes.RemoveAt(i);
    }
    void CancelDraw(GameObject strokeObj)
    {
        playerControl.SetIsGrounded(false);

        for (int i = 0; i < strokes.Count; ++i)
        {
            if (System.Object.ReferenceEquals(strokes[i].gameObject, strokeObj))
            {
                strokes[i].Destroy();
                strokes.RemoveAt(i);
                return;
            }
        }
    }

    void SetDepth()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            drawMove.SetDepth(true);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            drawMove.SetDepth(false);
    }

    void SetBrushType()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        currType = BrushType.SetType(currType);
        uiBrushType.UpdateBrushType(currType);
    }

    void SetBrushColor()
    {
        if (Input.GetKeyDown("1")) BrushType.SetColor(currType, Color.red);
        else if (Input.GetKeyDown("2")) BrushType.SetColor(currType, Color.green);
        else if (Input.GetKeyDown("3")) BrushType.SetColor(currType, Color.blue);
        else if(Input.GetKeyDown("4")) BrushType.SetColor(currType, Color.magenta);
        else if (Input.GetKeyDown("5")) BrushType.SetColor(currType, Color.yellow);
        else if (Input.GetKeyDown("6")) BrushType.SetColor(currType, Color.cyan);
        else if(Input.GetKeyDown("7")) BrushType.SetColor(currType, Color.white);
        else if (Input.GetKeyDown("8")) BrushType.SetColor(currType, Color.gray);
        else if (Input.GetKeyDown("9")) BrushType.SetColor(currType, Color.black);

        uiBrushType.UpdateColor(currType);
    }

    void UpdatePaintVolume()
    {
        paintedVolume += drawMove.GetDistance();
        uiBrushType.UpdatePaintVolume(GetPaintRemainRatio());
    }

    float GetPaintRemainRatio()
    { return Mathf.Clamp01((paintVolumeMax - paintedVolume) / paintVolumeMax); }

    public int GetPaintScore()
    {
        float ratio = GetPaintRemainRatio();

        if (ratio >= paintVolumeCut1) return 3;
        else if (ratio >= paintVolumeCut2) return 2;
        else if (ratio > 0) return 1;
        else return 0;
    }
}
