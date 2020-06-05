using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{

    public bool physicsOn = false;
    GameObject sphere, tracker, controller, controllerVis;
    public float tabletop_y;
    TaskManager_vive tm;
    public GameObject go, canvas;

    public StudyManager sm;
    public int state = 0;


    protected virtual void Start()
    {

        
        sphere = GameObject.Find("Small Sphere");
        tracker = GameObject.Find("Hand2");
        controller = GameObject.Find("Hand1");
        controllerVis = GameObject.Find("viveC");
        tm = gameObject.GetComponent<TaskManager_vive>();
        sm = GameObject.Find("StudyManager").GetComponent<StudyManager>();
        go = tracker;
    }



    public enum controllerEnum
    {
        sphere,
        controller,
        controller_menu,
        controller_arcball
    }
    public controllerEnum contr = controllerEnum.sphere;

    public bool is_on_table()
    {
        
        var diff = Mathf.Abs(tabletop_y - sphere.transform.position.y);

        if (diff < 0.2)
        {
            return true;
        }
        return false;
    }

    public float getPosition()
    {
        return getPosition(tracker.transform.position);
    }

    public float getPosition(Vector3 top)
    {
        float pos = top.y;
        if (physicsOn)
        {
            var x = top;
            var x1 = top;
            x1.y = x1.y - 2;
            x.y = x.y + 4;
            RaycastHit hit;
            if (Physics.Linecast(x, top, out hit, layerMask: ~(1 << sphere.layer)))
            {
                pos = hit.point.y;
            }
            else if(Physics.Linecast(  top, x1, out hit, layerMask: ~(1 << sphere.layer))&&tm.task!=2)
            {
                pos = hit.point.y;
                
            }
            else if (Physics.Linecast(x1, top, out hit, layerMask: ~(1 << sphere.layer)) && tm.task != 2)
            {
                pos = hit.point.y;

            }

        }
        return pos;
    }
    // Update is called once per frame
    void Update()
    {
        if (sm.switchController)
        {
            sm.switchController = false;
            if (state == 0)
            {
                state = 1;
                controller = GameObject.Find("Hand2");
                tracker = GameObject.Find("Hand1");
            }
            else
            {
                state = 0;
                tracker = GameObject.Find("Hand2");
                controller = GameObject.Find("Hand1");
            }
        }

        
        
        if (contr == controllerEnum.sphere)
        {
            go = tracker;
            controllerVis.SetActive(false);
            sphere.GetComponent<Renderer>().enabled=true;
        }
        else
        {
            go = controller;
            controllerVis.SetActive(true);
            sphere.GetComponent<Renderer>().enabled = false;
        }
        //if (tm.task == 3 && tm.started)
        //{
        //    physicsOn = true;
        //    var temp = go.transform.position;
        //    temp.y = wim.transform.position.y + 0.065f * sphere.transform.localScale.x;
            
            
        //    sphere.transform.position = temp;
        //}
        //else
        //{
            sphere.transform.position = go.transform.position;
            controllerVis.transform.position = go.transform.position;
            var temp = go.transform.position;
        temp.y += 0.1f;
        canvas.transform.position = temp;
        canvas.transform.LookAt(Camera.main.transform.position);

        //}

        sphere.transform.rotation = go.transform.rotation;
        controllerVis.transform.rotation = go.transform.rotation *Quaternion.Euler(90,0,0);




    }
}
