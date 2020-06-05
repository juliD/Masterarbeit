using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;
using System.Linq;

public class WIM_vive : MonoBehaviour
{
    GameObject wim, sphere, wim_recen;
    Collider bounds;
    Color originalColor;
    public Material sphereMat, highlightMat, sphereHighlight, menuMat, ui;
    bool prev_inbox = false;
    public float time;
    Quaternion last_angle;
    public float distanceThreshold, timeThreshold, scaleDistThres,controllerScaleThres,scaleRotThreshold = 1.0f, armDistThresh=0.65f;
    Vector3 current_wim_pos;
    Vector3 prev_pos, starting_point, direction, scale, pickpos;
    //Text text;
    GameObject player, pickuppoint, startscreen, terrain;
    //public GameObject city;
    //public TextMesh modeDisplay;
    bool sphereController = true;
    float timer = 0, distance_overall, angle, distance_pickup;
    int in_task = -2;
    Image loadingBar;
    string filePath;
    TaskManager_vive tm;
    PositionManager pm;
    modeEnum old_mode = modeEnum.neutral;
    int old_task = 0;
    int old_menu = 0;
    public int menu = 0;
    GameObject scaleM, moveM, rotateM, exitM, t5, menue4, menue5, rotationHelper, rotationVis;
    public GameObject go;
    LineRenderer lineRenderer;
    bool ground_placed = false, init_once=false;
    InputManager_Controller ab;
    Quaternion last_scale_rot, current_wim_rot;
    Vector3 local, campos;
    float dist_arm=0, last_angle_scale, scale_factor, old_distance, diff;
    int temp_menu;
    public bool movementStop = false;
    StudyManager sm;
    PositionManager.controllerEnum oldContr;
    int old_subtask = 0;
    public GameObject edgeSphere;
    public Text menu_picked;
    public enum modeEnum
    {
        pickup,
        selection,
        neutral,
        translationRotation,
        scale,
        scaleRotate
    }
    public modeEnum mode;
   
    public void hideMenu()
    {
        lineRenderer.positionCount = 0;
        moveM.transform.parent.gameObject.SetActive(false);
    }
    public void nextSubtask1()
    {
        rotationVis.SetActive(false);
        time = 0;
        timer2 = 0;
        movementStop = false;

    }
    public void setMenuActive(int i)
    {
        
        if (i == 2)
        {
            menue5.SetActive(true);
        }
        if (i == 1)
        {
            menue4.SetActive(true);
        }


    }

    void Start()
    {
        sm = GameObject.Find("StudyManager").GetComponent<StudyManager>();
        tm = gameObject.GetComponent<TaskManager_vive>();
        pm = gameObject.GetComponent<PositionManager>();
        rotationHelper = GameObject.Find("rotationHelper");
        rotationVis = GameObject.Find("rotationVis");
        ab = gameObject.GetComponent<InputManager_Controller>();
        scale = Vector3.one;
        local = Vector3.zero;
        //Create Linerenderer for scaling
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 0;

        //Get all necessary Gameobjects
        bounds = GameObject.Find("terrain").GetComponent<MeshCollider>();
        
        pickuppoint = GameObject.Find("sphereMotion");
        wim = GameObject.Find("terrain");
        t5 = GameObject.Find("t5");
        t5.SetActive(false);
        menue4 = GameObject.Find("sphereMotion");
        menue5 = GameObject.Find("sphereMotion1");
        originalColor = GameObject.Find("move").GetComponent<Renderer>().material.color;
        menue4.SetActive(false);
        menue5.SetActive(false);

        terrain = GameObject.Find("terrain");
        //scene = GameObject.Find("City");
        sphere = GameObject.Find("Small Sphere");
        player = GameObject.Find("InteractionCenter");
        //text = GameObject.Find("Text").GetComponent<Text>();
        loadingBar = GameObject.Find("LoadingBar").GetComponent<Image>();
        
        startscreen = GameObject.Find("StartScreen");


        edgeSphere.SetActive(false);
        

        starting_point = pm.go.transform.position;
        //scene.SetActive(false);
        time = 0;
        prev_pos = pm.go.transform.position;
        current_wim_pos = wim.transform.position;
        pickuppoint.transform.position = wim.transform.position;
        current_wim_rot = wim.transform.rotation;
        pickpos = current_wim_pos;
        mode = modeEnum.neutral;

        loadingBar.transform.parent.gameObject.SetActive(true);

        scale = wim.transform.localScale;
        
        scaleM = menue4.transform.Find("Menu/scale").gameObject;
        moveM = menue4.transform.Find("Menu/move").gameObject;

        exitM = menue4.transform.Find("Menu/exit").gameObject;

        scaleM.transform.Find("m/n").GetComponent<Text>().material = ui;
        moveM.transform.Find("m/n").GetComponent<Text>().material = ui;

        exitM.transform.Find("m/n").GetComponent<Text>().material = ui;
        menu_picked.material = ui;
        menu_picked.transform.parent.GetComponent<Image>().material = ui;

        moveM.transform.parent.gameObject.SetActive(false);
        startscreen = menue4.transform.Find("StartScreen").gameObject;


        
        RotationMode();
        rotationVis.SetActive(false);
        oldContr = pm.contr;
        old_subtask = tm.subtask;
        terrain.SetActive(false);
    }
    public void setTimer(int x)
    {
        timer = x;
    }

    void change()
    {
        sphereController = !sphereController;
    }

    void resetAll()
    {
        //scene.SetActive(false);
        player.transform.position = Vector3.zero;
        tm.task = 0;
        

        //switch off colors
        //GameObject.Find("wim_recen").GetComponent<TextureManager>().resetColor();
        menu = 0;
        time = 0;
        updateProgress(timeThreshold);

        sphere.transform.localScale = Vector3.one;
        wim.transform.localScale = Vector3.one;
        terrain.transform.localScale = Vector3.one;
    }

    public void start_task()
    {

        wim.SetActive(false);
        //scene.SetActive(false);
        in_task = -1;
        startscreen.SetActive(true);

    }
    //public void placeGround()
    //{
    //    ground_placed = false;
    //    init_once = false;
        
    //    in_task = -1;
    //    startscreen.SetActive(true);
    //}

    public void nextSubtask()
    {

        startscreen.SetActive(true);
        in_task = -1;
        mode = modeEnum.neutral;
        timer = -2;
        

    }
    void Update()
    {
        if (old_subtask != tm.subtask)
        {
            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[tm.subtask]).localRotation = Quaternion.Euler(0, 0, 0); 
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).localRotation = Quaternion.Euler(0, 0, 0);
            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[old_task]).localRotation = Quaternion.Euler(0, 0, 0);
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[old_task]).localRotation = Quaternion.Euler(0, 0, 0);

            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[tm.subtask]).localPosition = Vector3.zero;
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).localPosition = Vector3.zero;
            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[old_task]).localPosition = Vector3.zero;
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[old_task]).localPosition = Vector3.zero;

            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[tm.subtask]).localScale = Vector3.one;
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).localScale = Vector3.one;
            tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[old_task]).localScale = Vector3.one;
            tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[old_task]).localScale = Vector3.one;

            



            old_subtask = tm.subtask;
        }


        //text.text = "";
        if (oldContr != pm.contr)
        {
            rotationVis.SetActive(false);
            oldContr = pm.contr;
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }

        if (old_menu != menu)
        {
            old_menu = menu;
            temp_menu = menu;
            if (menu == 1)
            {
                pickuppoint.SetActive(false);
                menue4.transform.position = pickuppoint.transform.position;
                pickuppoint = menue4;
                pickuppoint.SetActive(true);

                scaleM = menue4.transform.Find("Menu/scale").gameObject;
                moveM = menue4.transform.Find("Menu/move").gameObject;

                exitM = menue4.transform.Find("Menu/exit").gameObject;

                scaleM.transform.Find("m/n").GetComponent<Text>().material = ui;
                moveM.transform.Find("m/n").GetComponent<Text>().material = ui;

                exitM.transform.Find("m/n").GetComponent<Text>().material = ui;


                moveM.transform.parent.gameObject.SetActive(false);
                startscreen = menue4.transform.Find("StartScreen").gameObject;
                startscreen.SetActive(false);
            }
            else if (menu == 2)
            {
                pickuppoint.SetActive(false);
                menue5.transform.position = pickuppoint.transform.position;
                pickuppoint = menue5;
                pickuppoint.SetActive(true);
            }
        }


        if (Input.GetKey("6"))
        {
            menu = 1;

        }

        if (Input.GetKey("7"))
        {
            menu = 2;

        }


        //text.text = "task: " + tm.task +"\nsubtask: "+tm.subtask;
        var tx = current_wim_pos;
        tx.y -= 2;
        //text.text = "\nPos: " + pm.getPosition(tx) + "\nPos2: " + pm.getPosition();
        
        if (old_task != tm.task)
        {
            tm.testObject.transform.localPosition = new Vector3(0, 0.9f, -0.3f);
            tm.testObject.transform.localScale = Vector3.one;
            tm.testObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            menu = temp_menu;
                if (tm.task == 1)
                {
                    sphere.transform.localScale = Vector3.one;
                    wim = tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[tm.subtask]).gameObject;
                    terrain.SetActive(false);
                    //city.SetActive(false);
                    t5.SetActive(false);
                    pickuppoint.transform.position = wim.transform.position;
                    current_wim_pos = wim.transform.position;
                    current_wim_rot = wim.transform.rotation;

            }
                else if (tm.task == 2)
                {
                    sphere.transform.localScale = Vector3.one;
                    wim = tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).gameObject;
                    t5.SetActive(false);
                    //city.SetActive(false);
                    terrain.SetActive(false);
                    pickuppoint.transform.position = wim.transform.position;
                current_wim_pos = wim.transform.position;
                current_wim_rot = wim.transform.rotation;
            }
            if (tm.task == 3)
            {
                wim = tm.testObject;
                terrain.SetActive(false);
                pickuppoint.transform.position = wim.transform.position;
                current_wim_pos = wim.transform.position;
                current_wim_rot = wim.transform.rotation;
            }
                else
                {
                    sphere.transform.localScale = Vector3.one;

                    //terrain.SetActive(true);
                    wim = terrain;
                    //city.SetActive(false);
                    t5.SetActive(false);
                    pickuppoint.transform.position = wim.transform.position;
                current_wim_pos = wim.transform.position;
                current_wim_rot = wim.transform.rotation;
            }
                //start_task();
                old_task = tm.task;
            //}
            scale = wim.transform.localScale;

        }

        if (in_task == -1)
        {

            var dist = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
            if (dist < 0.2)
            {
                pickuppoint.SetActive(true);
            }
            else
            {
                pickuppoint.SetActive(false);
            }
            if ((pickuppoint.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || dist < 0.05))
            {

                in_task++;
                if (tm.task != 4)
                {
                    startscreen.SetActive(false);
                }
                else
                {
                    tm.next = 1;
                }

                wim.SetActive(true);
                if (tm.task == 1)
                {
                    //model.SetActive(true);
                }
                if (tm.task != 5)
                {
                    tm.started = true;
                }
                
            }
        }
        else if (pm.contr != PositionManager.controllerEnum.controller_arcball)
        {
            sphere.SetActive(true);
            if (menu == 0)
            {
                
                MovementMode3();
                
                
            }
            else
            {
                MovementMode4();
            }

        }
        else 
        {
            sphere.SetActive(false);
            pickuppoint.SetActive(false);
            if (ab.pressed)
            {
                if (!movementStop)
                {
                    ClutchRotationMode();
                    ClutchtransRotWim();
                }
                else
                {
                    wim.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    wim.transform.localPosition = Vector3.zero;
                    wim.transform.localScale = Vector3.one;
                }
                
            }
            else
            {
                pm.go.transform.GetChild(0).rotation = current_wim_rot;
                
                
            }


        }
        if (tm.task == 2)
        {
            if (pm.contr == PositionManager.controllerEnum.sphere)
            {
                t5.transform.position = new Vector3(0.317f, 0.774f+sm.calib, -0.4705f);
            }
            else
            {
                t5.transform.position = new Vector3(0.317f, 0.804f, -0.4705f);
            }

            //if (tm.placed)
            //{
            //    t5.SetActive(false);
            //    wim = tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).gameObject;
            //    pickuppoint.transform.position = wim.transform.position;
            //    menu = 0;
            //}
            //else
            //{
            //    t5.SetActive(true);
            //    pickuppoint.transform.position = wim.transform.position;

            //    menu = 1;
            //    wim = t5;
            //}
            //current_wim_pos = wim.transform.position;
            //current_wim_rot = wim.transform.rotation;
            t5.SetActive(false);
            wim = tm.modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[tm.subtask]).gameObject;
            pickuppoint.transform.position = wim.transform.position;
            current_wim_pos = wim.transform.position;
            current_wim_rot = wim.transform.rotation;
            tm.placed = true;
            tm.started = true;


        }
        if (tm.task == 1)
        {
            wim = tm.modelObjects.transform.GetChild(sm.subtask_order_task_rocks[tm.subtask]).gameObject;
            pickuppoint.transform.position = wim.transform.position;
            current_wim_pos = wim.transform.position;
            current_wim_rot = wim.transform.rotation;
        }

        







        prev_pos = pm.go.transform.position;
        last_scale_rot = pm.go.transform.rotation;
        rotationHelper.transform.position = pm.go.transform.position;
        rotationHelper.transform.rotation =  pm.go.transform.rotation;
        
    }

    void rotationToTranslation(Quaternion from, Quaternion to)
    {
        
        var x = Quaternion.Inverse(go.transform.rotation) * from;
        var y = Quaternion.Inverse(go.transform.rotation) * to;
        Vector3 floorPoint = new Vector3(0, -1, 0);
        Quaternion quadDiff = x * Quaternion.Inverse(y);

        Vector3 floorPointMult = quadDiff * floorPoint;

        //rotationVis.transform.GetChild(0).Rotate(rotationVis.transform.GetChild(0).right,floorPointMult.z);
        
        scale_factor = -floorPointMult.z;
        //var sumZ += floorPointMult.x;

    }


    public void ControllerScale(float dist)
    {
        if (pm.contr == PositionManager.controllerEnum.controller_arcball)
        {
            scale += Vector3.one * dist*scale.x *controllerScaleThres;
            if (scale.x < 1) scale = Vector3.one;
            wim.transform.localScale = scale;
        }
        
    }
    public void ClutchRotationMode()
    {
        
        wim.transform.rotation = pm.go.transform.GetChild(0).rotation;
        current_wim_rot = pm.go.transform.GetChild(0).rotation; 


    }

    public void RotationMode()
    {

        wim.transform.rotation = pm.go.transform.GetChild(0).rotation;
        
        
        //if (tm.task == 2)
        //{

        //}
        //else wim.transform.rotation = Quaternion.Euler(0, pm.go.transform.rotation.eulerAngles.y, 0);


    }



    public void updateProgress(float timeThres)
    {
        loadingBar.fillAmount = time / timeThres;
        //loadingBar.transform.parent.LookAt(Camera.main.transform.position, Vector3.up);
    }


    float timer2 = 0;

    void MovementMode5()
    {
        Debug.Log("Movement5");
        var lookat = go.transform.position;
        lookat.y = pickuppoint.transform.position.y;

        pickuppoint.transform.LookAt(lookat);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
        //text.text = "\nscale= " + wim.transform.localScale.x;
        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {

            if (wim.transform.localScale.x < 1.5)
            {
                mode = modeEnum.scale;
                pickuppoint.SetActive(false);
                pm.go.transform.GetChild(0).rotation = current_wim_rot;
                starting_point = current_wim_pos;
                sphere.GetComponent<Renderer>().material = sphereHighlight;
            }
            


        }
        if (mode == modeEnum.neutral)
        {
            pickup();
            var dist = 0.0;
            if (wim.transform.childCount > 0)
            {
                dist = Vector3.Distance(wim.transform.GetChild(0).GetComponent<MeshCollider>().bounds.ClosestPoint(pm.go.transform.position), pm.go.transform.position);
            }
            else
            {
                dist = Vector3.Distance(wim.GetComponent<MeshCollider>().bounds.ClosestPoint(pm.go.transform.position), pm.go.transform.position);
            }

            if (wim.transform.localScale.x > 1 && dist < 0.05 && timer2 > 2)
            {

                mode = modeEnum.pickup;
            }
        }
        if (mode == modeEnum.scale)
        {

            scaleWithRot();

        }
        

        last_angle = pm.go.transform.rotation;




    }
    void MovementMode4()
    {
        //Debug.Log("Movement4");
        var lookat = go.transform.position;
        lookat.y = pickuppoint.transform.position.y;

        pickuppoint.transform.LookAt(lookat);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
        //text.text = "\nscale= " + wim.transform.localScale.x;
        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {
            
            if (wim.transform.localScale.x < 1.5)
            {
                mode = modeEnum.translationRotation;
                pickuppoint.SetActive(false);
                pm.go.transform.GetChild(0).rotation = current_wim_rot;
                starting_point = current_wim_pos;
                sphere.GetComponent<Renderer>().material = sphereHighlight;
            }
            else
            {
                //if (tm.task != 5)
                //{
                //    lineRenderer.positionCount = 2;
               
                //    starting_point = current_wim_pos;
                
                //    pickuppoint.SetActive(false);
                //    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                //    mode = modeEnum.scaleRotate;
                //}
                
            }


        }
        if (mode == modeEnum.neutral)
        {
            pickup();
            var dist = 0.0;
            if (wim.transform.childCount > 0)
            {
                dist = Vector3.Distance(wim.transform.GetChild(0).GetComponent<MeshCollider>().bounds.ClosestPoint(pm.go.transform.position), pm.go.transform.position);
            }
            else
            {
                dist = Vector3.Distance(wim.GetComponent<MeshCollider>().bounds.ClosestPoint(pm.go.transform.position), pm.go.transform.position);
            }
            
            if (wim.transform.localScale.x>1 && dist<0.05 && timer2>2)
            {

                mode = modeEnum.pickup;
            }
        }
        if (mode == modeEnum.scaleRotate)
        {

            scaleWimInit();

        }
        if (mode == modeEnum.translationRotation)
        {
            transRotWim();

        }

        last_angle = pm.go.transform.rotation;




    }
    void MovementMode3()
    {
        
        pickuppoint.transform.LookAt(Camera.main.transform.position);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);

        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {

            //scene.SetActive(false);
            moveM.transform.parent.gameObject.SetActive(true);
            moveM.transform.parent.LookAt(Camera.main.transform.position, Vector3.up);
            timer += Time.deltaTime;
            float d_m = Vector3.Distance(moveM.transform.position, pm.go.transform.position);
            float d_s = Vector3.Distance(scaleM.transform.position, pm.go.transform.position);

            float d_e = Vector3.Distance(exitM.transform.position, pm.go.transform.position);
            float[] arr = { d_m, d_s, d_e };
            int shortest = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < arr[shortest]) { shortest = i; }
            }


            if (shortest == 0)
            {

                moveM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;
                exitM.GetComponent<Renderer>().material.color = originalColor;

                if (moveM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_m < 0.05 )
                {
                    ab.pressed = false;
                    mode = modeEnum.translationRotation;
                    moveM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                    menu_picked.transform.parent.gameObject.SetActive(true);
                    menu_picked.text = "move & rotate";
                }
            }
            else if (shortest == 1)
            {
                scaleM.GetComponent<Renderer>().material = highlightMat;
                moveM.GetComponent<Renderer>().material.color = originalColor;

                exitM.GetComponent<Renderer>().material.color = originalColor;
                if (scaleM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_s < 0.05 )
                {
                    ab.pressed = false;
                    mode = modeEnum.scale;
                    scaleM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    starting_point = pickuppoint.transform.position;
                    lineRenderer.positionCount = 2;
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                    menu_picked.transform.parent.gameObject.SetActive(true);
                    menu_picked.text = "scale";
                }
            }

            else if (shortest == 2)
            {
                exitM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;

                moveM.GetComponent<Renderer>().material.color = originalColor;
                if (exitM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_e < 0.05 )
                {
                    ab.pressed = false;
                    mode = modeEnum.neutral;
                    exitM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    tm.exited++;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

        }
        if (mode == modeEnum.neutral)
        {
            pickup();
        }
        if (mode == modeEnum.scaleRotate)
        {
            rotateWim();
        }
        if (mode == modeEnum.scale)
        {
            if(pm.contr == PositionManager.controllerEnum.controller_menu)
            {
                contrDistScale();
            }
            else scaleWithRot();
        }
        if (mode == modeEnum.translationRotation)
        {
            if (pm.contr == PositionManager.controllerEnum.controller_menu)
            {
                
                if (ab.pressed)
                {
                    ClutchRotationMode();
                    ClutchtransRotWim();
                }
                else
                {
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }

                if (distanceThreshold > distance_overall && Mathf.Abs(angle) < 2)
                {

                    time += Time.deltaTime;
                }
                else
                {
                    time = 0;
                }
                updateProgress(timeThreshold);

                //exit pickup mode
                if (time > timeThreshold)
                {
                    menu_picked.text = "";
                    time = 0;
                    updateProgress(timeThreshold);
                    mode = modeEnum.neutral;
                    timer2 = 0;
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                    pickuppoint.transform.position = current_wim_pos;

                }

            }
            else transRotWim();
        }

        last_angle = pm.go.transform.rotation;




    }
    void MovementMode2()
    {
        //Debug.Log("Movement2");
        pickuppoint.transform.LookAt(Camera.main.transform.position);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
        dist_arm = 0.0f;
        var point = Vector3.zero;
        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {
            local = pm.go.transform.InverseTransformPoint(current_wim_pos);
            //scene.SetActive(false);
            moveM.transform.parent.gameObject.SetActive(true);
            moveM.transform.parent.LookAt(Camera.main.transform.position, Vector3.up);
            timer += Time.deltaTime;
            float d_m = Vector3.Distance(moveM.transform.position, pm.go.transform.position);
            float d_s = Vector3.Distance(scaleM.transform.position, pm.go.transform.position);

            float d_e = Vector3.Distance(exitM.transform.position, pm.go.transform.position);
            float[] arr = { d_m, d_s, d_e };
            int shortest = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < arr[shortest]) { shortest = i; }
            }


            if (shortest == 0)
            {

                moveM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;
                exitM.GetComponent<Renderer>().material.color = originalColor;

                if (moveM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_m < 0.05)
                {
                    mode = modeEnum.translationRotation;
                    moveM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }
            else if (shortest == 1)
            {
                scaleM.GetComponent<Renderer>().material = highlightMat;
                moveM.GetComponent<Renderer>().material.color = originalColor;

                exitM.GetComponent<Renderer>().material.color = originalColor;
                if (scaleM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_s < 0.05)
                {
                    diff = Vector3.Distance(pm.go.transform.position, current_wim_pos)-old_distance;
                    campos = go.transform.position;
                    mode = modeEnum.scale;
                    scaleM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    pickuppoint.transform.position = pm.go.transform.position;
                    starting_point = pickuppoint.transform.position;
                    lineRenderer.positionCount = 2;
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

            else if (shortest == 2)
            {
                exitM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;

                moveM.GetComponent<Renderer>().material.color = originalColor;
                if (exitM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_e < 0.05)
                {
                    mode = modeEnum.neutral;
                    exitM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    tm.exited++;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

        }
        if (mode == modeEnum.neutral)
        {
            
            if (wim.transform.childCount > 0)
            {
                point = wim.transform.GetChild(0).GetComponent<Renderer>().bounds.ClosestPoint(pm.go.transform.position);
                dist_arm = Vector3.Distance(point, current_wim_pos);
            }
            else
            {
                point = wim.transform.GetComponent<Renderer>().bounds.ClosestPoint(pm.go.transform.position);
                dist_arm = Vector3.Distance(point, current_wim_pos);
            }
            if (dist_arm > armDistThresh)
            {
                pickuppoint.transform.position = point;
                distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
            }else
            {
                pickuppoint.transform.position = current_wim_pos;
            }
            starting_point = pickuppoint.transform.position;
            pickup();
        }
        if (mode == modeEnum.scaleRotate)
        {
            rotateWim();
        }
        if (mode == modeEnum.scale)
        {
            scaleWimInit();
        }
        if (mode == modeEnum.translationRotation)
        {
            transRotWimDistance(dist_arm);
        }

        last_angle = pm.go.transform.rotation;

    }
    void MovementMode1()
    {
        //Debug.Log("Movement1");
        pickuppoint.transform.LookAt(Camera.main.transform.position);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
        dist_arm = 0.0f;
        var point = Vector3.zero;
        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {
            local = pm.go.transform.InverseTransformPoint(current_wim_pos);
            //scene.SetActive(false);
            moveM.transform.parent.gameObject.SetActive(true);
            moveM.transform.parent.LookAt(Camera.main.transform.position, Vector3.up);
            timer += Time.deltaTime;
            float d_m = Vector3.Distance(moveM.transform.position, pm.go.transform.position);
            float d_s = Vector3.Distance(scaleM.transform.position, pm.go.transform.position);

            float d_e = Vector3.Distance(exitM.transform.position, pm.go.transform.position);
            float[] arr = { d_m, d_s, d_e };
            int shortest = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < arr[shortest]) { shortest = i; }
            }


            if (shortest == 0)
            {

                moveM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;
                exitM.GetComponent<Renderer>().material.color = originalColor;

                if (moveM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_m < 0.05)
                {
                    mode = modeEnum.translationRotation;
                    moveM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }
            else if (shortest == 1)
            {
                scaleM.GetComponent<Renderer>().material = highlightMat;
                moveM.GetComponent<Renderer>().material.color = originalColor;

                exitM.GetComponent<Renderer>().material.color = originalColor;
                if (scaleM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_s < 0.05)
                {
                    diff = Vector3.Distance(pm.go.transform.position, current_wim_pos) - old_distance;
                    campos = go.transform.position;
                    mode = modeEnum.scale;
                    scaleM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    pickuppoint.transform.position = pm.go.transform.position;
                    starting_point = pickuppoint.transform.position;
                    lineRenderer.positionCount = 2;
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

            else if (shortest == 2)
            {
                exitM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;

                moveM.GetComponent<Renderer>().material.color = originalColor;
                if (exitM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_e < 0.05)
                {
                    mode = modeEnum.neutral;
                    exitM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    tm.exited++;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

        }
        if (mode == modeEnum.neutral)
        {

            if (wim.transform.childCount > 0)
            {
                point = wim.transform.GetChild(0).GetComponent<Renderer>().bounds.ClosestPoint(pm.go.transform.position);
                dist_arm = Vector3.Distance(point, current_wim_pos);
            }
            else
            {
                point = wim.transform.GetComponent<Renderer>().bounds.ClosestPoint(pm.go.transform.position);
                dist_arm = Vector3.Distance(point, current_wim_pos);
            }
            if (dist_arm > armDistThresh)
            {
                pickuppoint.transform.position = point;
                distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
            }
            else
            {
                pickuppoint.transform.position = current_wim_pos;
            }
            starting_point = pickuppoint.transform.position;
            pickup();
        }
        if (mode == modeEnum.scaleRotate)
        {
            rotateWim();
        }
        if (mode == modeEnum.scale)
        {
            scaleWimInit();
        }
        if (mode == modeEnum.translationRotation)
        {
            transRotWimDistance(dist_arm);
        }

        last_angle = pm.go.transform.rotation;

    }
    void MovementMode()
    {

        //Debug.Log("Movement");
        pickuppoint.transform.LookAt(Camera.main.transform.position);
        var local_s = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);
        var local_p = pickuppoint.transform.GetChild(0).transform.localPosition;


        var distancex = Mathf.Abs(local_s.x - local_p.x);
        var distancey = Mathf.Abs(local_s.y - local_p.y);
        var distancez = Mathf.Abs(local_s.z - local_p.z);
        distance_overall = Vector3.Distance(pm.go.transform.position, prev_pos);
        angle = Quaternion.Angle(last_angle, pm.go.transform.rotation);

        //var distance_to_start = Vector3.Distance(pm.go.transform.position, starting_point);
        var distance = Mathf.Abs(pm.go.transform.position.y - current_wim_pos.y);


        distance_pickup = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);

        //if already in pickup mode
        if (mode == modeEnum.pickup)
        {

            //scene.SetActive(false);
            moveM.transform.parent.gameObject.SetActive(true);
            moveM.transform.parent.LookAt(Camera.main.transform.position, Vector3.up);
            timer += Time.deltaTime;
            float d_m = Vector3.Distance(moveM.transform.position, pm.go.transform.position);
            float d_s = Vector3.Distance(scaleM.transform.position, pm.go.transform.position);

            float d_e = Vector3.Distance(exitM.transform.position, pm.go.transform.position);
            float[] arr = { d_m, d_s, d_e };
            int shortest = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < arr[shortest]) { shortest = i; }
            }


            if (shortest == 0)
            {

                moveM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;
                exitM.GetComponent<Renderer>().material.color = originalColor;

                if (moveM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_m < 0.05)
                {
                    mode = modeEnum.translationRotation;
                    moveM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }
            else if (shortest == 1)
            {
                scaleM.GetComponent<Renderer>().material = highlightMat;
                moveM.GetComponent<Renderer>().material.color = originalColor;

                exitM.GetComponent<Renderer>().material.color = originalColor;
                if (scaleM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_s < 0.05)
                {
                    mode = modeEnum.scale;
                    scaleM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    starting_point = pickuppoint.transform.position;
                    lineRenderer.positionCount = 2;
                    timer = 0;
                    sphere.GetComponent<Renderer>().material = sphereHighlight;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

            else if (shortest == 2)
            {
                exitM.GetComponent<Renderer>().material = highlightMat;
                scaleM.GetComponent<Renderer>().material.color = originalColor;

                moveM.GetComponent<Renderer>().material.color = originalColor;
                if (exitM.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || d_e < 0.05)
                {
                    mode = modeEnum.neutral;
                    exitM.GetComponent<Renderer>().material.color = originalColor;
                    moveM.transform.parent.gameObject.SetActive(false);
                    timer = 0;
                    tm.exited++;
                    pickuppoint.SetActive(false);
                    pm.go.transform.GetChild(0).rotation = current_wim_rot;
                }
            }

        }
        if (mode == modeEnum.neutral)
        {
            pickup();
        }
        if (mode == modeEnum.scaleRotate)
        {
            rotateWim();
        }
        if (mode == modeEnum.scale)
        {
            scaleWim();
        }
        if (mode == modeEnum.translationRotation)
        {
            transRotWim();
        }

        last_angle = pm.go.transform.rotation;

    }
    Vector3 pressDown, enterScalingPos;
    float last_distance_wim;
    bool in_scaling;
    public void SetPressDown()
    {
        pressDown = pm.go.transform.position;
        last_distance_wim = Vector3.Distance(pm.go.transform.position, current_wim_pos);
    }
    void contrDistScale()
    {
        Debug.Log("pressing: " + ab.pressed);
        Debug.Log("dist: " + distance_overall);
        
        if (ab.pressed)
        {
            
            var dist_wim = Vector3.Distance(pm.go.transform.position, current_wim_pos);
            var dist_press = Vector3.Distance(current_wim_pos, pressDown);


            wim.transform.localScale = scale* dist_wim/dist_press * scaleDistThres;
            
            if (wim.transform.localScale.x < 1) wim.transform.localScale = Vector3.one;


        }
        else
        {
            scale = wim.transform.localScale;
        }
         
        last_distance_wim = distance_overall;
        if (distanceThreshold*0.5 > distance_overall )
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {
            menu_picked.text = "";
            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            lineRenderer.positionCount = 0;
            timer2 = 0;
            scale = wim.transform.localScale;
            pickuppoint.transform.position = current_wim_pos;

        }
    }
    void scaleWithRot()
    {
        
        rotationToTranslation(rotationHelper.transform.rotation,pm.go.transform.rotation);
        scale += Vector3.one*scale_factor*scale.x*scaleRotThreshold;
        if (scale.x < 1) scale = Vector3.one;
        wim.transform.localScale = scale;


        rotationVis.SetActive(true);
        rotationVis.transform.position = pm.go.transform.position;
        rotationVis.transform.rotation = go.transform.rotation * Quaternion.Euler(0, 0, 90);

        //Debug.Log("Sphere: " + pm.go.transform.position);
        //Debug.Log("helper: " + rotationHelper.transform.position);

        //check if exiting scaling mode
        if (Mathf.Abs(angle) < 2)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            lineRenderer.positionCount = 0;
            timer2 = 0;
            ground_placed = true;
            
            pickuppoint.transform.position = current_wim_pos;
            
            tm.next = 1;
            scale = wim.transform.localScale;
            rotationVis.SetActive(false);
        }
    }
    void transRotWim()
    {

        //Debug.Log("Sphere: "+pm.go.transform.position);
        
        //var temp = pm.go.transform.position;
        if (tm.task == 5 && tm.placed == false)
        {

        }
        else
        {
            RotationMode();
        }

        //if (tm.task!=5)
        //{
        //    //temp.y -= 0.5f;
        //    RotationMode();
        //}
        //else
        //{

        //    sphere.GetComponent<Renderer>().enabled = false;

        //    RotationMode();

        //    var point = pm.go.transform.position;
        //    point.y = current_wim_pos.y;
        //    var b = terrain.transform.GetChild(0).GetComponent<MeshCollider>().bounds;

        //    if (tm.task == 5)
        //    {
        //        b = t5.transform.GetChild(0).GetComponent<MeshCollider>().bounds;
        //    }
        //    var p1 = b.center + b.extents;
        //    var p2 = b.center - b.extents;


        //    if (p1.x > point.x && p1.z > point.z && p2.x < point.x && p2.z < point.z)
        //    {

        //        temp.y = pm.getPosition();
        //        pickpos = temp;
        //        text.text = "getPosition = " + temp.y;

        //        var t = temp;
        //        t.y -= 2;
        //        if (tm.task == 5)
        //        {
        //            var bottom = wim.transform.GetChild(0).GetComponent<BoxCollider>().bounds.ClosestPoint(t);
        //            var distance_bottom = Mathf.Abs(bottom.y - current_wim_pos.y);
        //            //text.text += "bottom: " + bottom.x + ", " + bottom.y + ", " + bottom.z;
        //            //text.text += "\ndistance_bottom: " + distance_bottom;
        //            //text.text += "\ntemp: " + temp.y;

        //            temp.y += distance_bottom / 2;
        //        }
        //        else
        //        {
        //            var bottom = wim.GetComponent<MeshCollider>().bounds.ClosestPoint(t);
        //            var distance_bottom = Mathf.Abs(bottom.y - current_wim_pos.y);
        //            //text.text += "bottom: " + bottom.x + ", " + bottom.y + ", " + bottom.z;
        //            //text.text += "\ndistance_bottom: " + distance_bottom;
        //            //text.text += "\ntemp: " + temp.y;

        //            temp.y += distance_bottom;
        //        }



        //    }
        //    else
        //    {
        //        text.text = "outside";
        //    }
        //}
        current_wim_pos = pm.go.transform.position;
        current_wim_rot = pm.go.transform.GetChild(0).rotation;
            wim.transform.position = current_wim_pos;
            //Debug.Log("Wim " + current_wim_pos);
        //if (tm.task == 2)
        //{
        //    var t_pos1 = wim.transform.position;
        //    t_pos1.y = pm.getPosition();
        //    pickuppoint.transform.position = t_pos1;
        //}
        //else
        //{
        //pickuppoint.transform.position = wim.transform.position;
        //}



        //check if exiting pickup mode
        if (distanceThreshold > distance_overall && Mathf.Abs(angle) < 2)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            if (tm.task == 2)
            {
                tm.placed = true;
                tm.started = true;
                if (pm.contr == PositionManager.controllerEnum.sphere)
                {
                    current_wim_pos.y += 0.006f;
                    wim.transform.position = current_wim_pos;
                }
                else
                {
                    current_wim_pos.y += 0.005f;
                    wim.transform.position = current_wim_pos;
                }
            }
            timer2 = 0;
            sphere.GetComponent<Renderer>().enabled = true;
            tm.next = 1;
            pickuppoint.transform.position = current_wim_pos;

        }
    }
    void ClutchtransRotWim()
    {


        if (!movementStop)
        {
            current_wim_pos = pm.go.transform.position;
            current_wim_rot = pm.go.transform.GetChild(0).rotation;
            wim.transform.position = current_wim_pos;

            pickuppoint.transform.position = current_wim_pos;
        }

       
        

    }
    void transRotWimDistance(float dist)
    {
        
        

        RotationMode();
        var d = Vector3.Distance(pickuppoint.transform.position, current_wim_pos);
        if (d<0.2)
        {
            current_wim_pos = pm.go.transform.position;
            current_wim_rot = pm.go.transform.GetChild(0).rotation;
            wim.transform.position = current_wim_pos;
            pickuppoint.transform.position = current_wim_pos;
        }
        else
        {
            current_wim_pos = pm.go.transform.TransformPoint(local);
            wim.transform.position = current_wim_pos;
            current_wim_rot = pm.go.transform.GetChild(0).rotation;
        }
        

        //pickuppoint.transform.position = wim.transform.position;


        //check if exiting pickup mode
        if (distanceThreshold > distance_overall && angle < 0.5)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            if (menu == 4)
            {
                mode = modeEnum.scaleRotate;
                lineRenderer.positionCount = 2;
                starting_point = current_wim_pos;
                sphere.GetComponent<Renderer>().material = sphereMat;
            }
            else mode = modeEnum.neutral;
            timer2 = 0;
            sphere.GetComponent<Renderer>().enabled = true;
            tm.next = 1;

        }
    }
    void scaleWimInit()
    {
        var lookPos = current_wim_pos - pickuppoint.transform.position;
        lookPos.y = 0;
        pickuppoint.transform.rotation = Quaternion.LookRotation(lookPos);

        var local_wim = pickuppoint.transform.InverseTransformPoint(current_wim_pos);
        var local_sphere = pickuppoint.transform.InverseTransformPoint(pm.go.transform.position);


        //var dist = Mathf.Abs(local_s.z- local_wim.z);
        var dist = Vector3.Distance(pm.go.transform.position,current_wim_pos);
        var dist_pick = Vector3.Distance(pm.go.transform.position, pickuppoint.transform.position);
        var dist_prev = Vector3.Distance(prev_pos, current_wim_pos);
        var distToLast = Vector3.Distance(prev_pos, pm.go.transform.position);

        var v1 = pm.go.transform.position - prev_pos;
        v1.y = 0;
        var v2 = current_wim_pos - campos;
        v2.y = 0;
        var angle = Vector3.Angle(v1, v2);
        var dist_cam = Vector3.Distance(pm.go.transform.position, campos);
        var dist_cam_wim = Vector3.Distance(current_wim_pos, campos);

        if (angle < 90)
        {
            //if(dist_cam>dist_cam_wim) scale -= Vector3.one * distToLast * scaleDistThres*2;
            scale -= Vector3.one * distToLast * scaleDistThres;
        }
        else
        {
            
            scale += Vector3.one * distToLast * scaleDistThres;
        }


        var temp = Vector3.one * old_distance + scale;
        
        if (temp.x < 1) temp = Vector3.one;
        wim.transform.localScale = temp;
        //if (wim.transform.localScale.x < 1.)
        //{
        //    wim.transform.localScale = Vector3.one;
        //}

        

        pickuppoint.transform.position = pm.go.transform.position;

        lineRenderer.SetPosition(0, pickuppoint.transform.position);

        lineRenderer.SetPosition(1, current_wim_pos);
        if (mode == modeEnum.scaleRotate)
        {
            RotationMode();
        }

        //check if exiting scaling mode
        if (distanceThreshold > distance_overall)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            lineRenderer.positionCount = 0;
            timer2 = 0;
            ground_placed = true;
            
            pickuppoint.transform.position = current_wim_pos;
            
            tm.next = 1;
            old_distance = dist;
        }
    }
    void scaleWim()
    {


        
        if (pm.go.transform.position.y - starting_point.y > 0)
        {
            wim.transform.localScale = scale + Vector3.one * Mathf.Abs(pm.go.transform.position.y - starting_point.y) * 10;
        }
        else
        {
            wim.transform.localScale = scale - Vector3.one * Mathf.Abs(pm.go.transform.position.y - starting_point.y) * 10;
        }

        pickuppoint.transform.position = pm.go.transform.position;

        lineRenderer.SetPosition(0, pickuppoint.transform.position);

        lineRenderer.SetPosition(1, starting_point);

        if (mode == modeEnum.scaleRotate)
        {
            RotationMode();
        }

        //check if exiting scaling mode
        if (distanceThreshold > distance_overall)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            lineRenderer.positionCount = 0;
            timer2 = 0;
            ground_placed = true;
            
            pickuppoint.transform.position = current_wim_pos;
            
            tm.next = 1;
            scale = wim.transform.localScale;
        }
    }
    void pickup()
    {
        menu_picked.text = "";
        menu_picked.transform.parent.gameObject.SetActive(false);
        moveM.transform.parent.gameObject.SetActive(false);
        if (distance_pickup < 0.3)
        {
            pickuppoint.SetActive(true);
        }
        else
        {
            pickuppoint.SetActive(false);
        }
        //if close to pickup point
        sphere.GetComponent<Renderer>().material = sphereMat;

        timer2 += Time.deltaTime;

        //Debug.Log(distance_pickup);
        //Debug.Log("Timer "+timer2);
        if ((pickuppoint.GetComponent<SphereCollider>().bounds.Contains(pm.go.transform.position) || distance_pickup < 0.05) && timer2 > 0.1)
        {
            ab.pressed = false;
            mode = modeEnum.pickup;

        }
    }
    void rotateWim()
    {
        RotationMode();
        if (distanceThreshold > distance_overall)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {
            menu_picked.text = "";
            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            lineRenderer.positionCount = 0;
            timer2 = 0;
            
            pickuppoint.transform.position = current_wim_pos;
            
            tm.next = 1;
        }
    }
    void translateWim()
    {
        //move and rotate WIM with sphere
        var temp = pm.go.transform.position;


        if (tm.task != 2)
        {
            //temp.y -= 0.5f;

            //RotationMode();
        }
        else
        {

            sphere.GetComponent<Renderer>().enabled = false;

            //RotationMode();

            var point = pm.go.transform.position;
            point.y = current_wim_pos.y;
            var b = terrain.GetComponent<MeshCollider>().bounds;
            var p1 = b.center + b.extents;
            var p2 = b.center - b.extents;
            if (p1.x > point.x && p1.z > point.z && p2.x < point.x && p2.z < point.z)
            {

                temp.y = pm.getPosition();
                pickpos = temp;
                //text.text += "getPosition = " + temp.y;

                var t = temp;
                t.y -= 2;
                var bottom = wim.GetComponent<MeshCollider>().bounds.ClosestPoint(t);
                var distance_bottom = Mathf.Abs(bottom.y - current_wim_pos.y);
                //text.text += "bottom: " + bottom.x + ", " + bottom.y + ", " + bottom.z;
                //text.text += "\ndistance_bottom: " + distance_bottom;
                //text.text += "\ntemp: " + temp.y;

                temp.y += distance_bottom / 2;

            }

        }

        wim.transform.position = temp;

        
        pickuppoint.transform.position = current_wim_pos;
        


        //check if exiting pickup mode
        if (distanceThreshold > distance_overall)
        {

            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        updateProgress(timeThreshold);

        //exit pickup mode
        if (time > timeThreshold)
        {

            time = 0;
            updateProgress(timeThreshold);
            mode = modeEnum.neutral;
            timer2 = 0;
            sphere.GetComponent<Renderer>().enabled = true;
            tm.next = 1;
        }
    }


}
