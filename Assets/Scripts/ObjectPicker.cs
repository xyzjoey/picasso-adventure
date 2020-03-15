using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker : MonoBehaviour
{
    new Camera camera;

    RaycastHit hit;

    [HideInInspector] public bool hitSwitch;
    [HideInInspector] public bool hitStroke;
    [HideInInspector] public GameObject hittedObject;

    void Start()
    {
        camera = Camera.main;

        hitSwitch = false;
        hitStroke = false;
        hittedObject = null;
    }

    void Update()
    {
        hitSwitch = false;
        hitStroke = false;

        //detect hit
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 5))
        {
            if (hit.collider.CompareTag("Switch")) hitSwitch = true;
            //else if (hit.collider.CompareTag("Stroke")) hitStroke = true;
        }


        //set obj
        GameObject obj;

        if (hitStroke) obj = hit.collider.gameObject.transform.parent.gameObject;
        else if (hitSwitch) obj = hit.collider.gameObject;
        else obj = null;


        //set outline & hittedObject
        bool hitAny = hitSwitch || hitStroke;

        if (!(hitAny && System.Object.ReferenceEquals(hittedObject, obj)) && hittedObject != null)
        {
            hittedObject.GetComponent<Outline>().enabled = false;
            hittedObject = null;
        }
        if (hitAny && !System.Object.ReferenceEquals(hittedObject, obj))
        {
            if (obj.GetComponent<Outline>() == null)
            {
                var outline = obj.AddComponent<Outline>();
                SetOutline(outline);
            }
            else obj.GetComponent<Outline>().enabled = true;

            hittedObject = obj;
        }
    }

    void SetOutline(Outline outline)
    {
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 10f;
    }
}
