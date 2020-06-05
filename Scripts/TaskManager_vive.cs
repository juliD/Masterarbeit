using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TaskManager_vive : MonoBehaviour
{
    string filePath;
    public int task = 0, subtask = 0;
    public GameObject[] menues;
    WIM_vive icenter;
    public GameObject wim;
    public GameObject player, targetPlacement, modelObjects, targetPlacementt5, modelObjectst5;
    GameObject sphere;
    public Material ui;
    float timeStamp = 0, time = 0;
    float time_since_start = 0;
    float accuracy = 0, rot_accuracy = 0, scale_accuracy = 0;
    public float accThreshold=0.1f, rotThreshold = 3f, scaleThreshold = 0.7f, timeThreshold = 5, accThreshold_task2=0.1f, rotThreshold_task2=10f;
    int participant = 1;
    public int collision_count = 0;
    public bool started = false;
    bool started_path = false, pause=false;
    int  points_count = 0;
    float distance_mean = 0;
    Color originalColor;
    Vector3[,] t1;
    List<Vector3> linePositions;
    //Text text;
    LineRenderer lr;
    MeshCollider meshCollider;
    int old_task=0;
    string[,] t4;
    Text startscreen;
    GameObject up, down, left, right, infront, back, move, scale, rotate;
    public int exited = 0, next=0;
    PositionManager pm;
    StudyManager sm;
    public bool placed;
    float[,,] acc;
    float[,] times;
    public int count = 0;
    public GameObject testPlacement, testObject;
    public float getTime()
    {
        return time_since_start;
    }
    public int getParticipant()
    {
        return participant;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        sm = GameObject.Find("StudyManager").GetComponent<StudyManager>();
        pm = gameObject.GetComponent<PositionManager>();

        startscreen = GameObject.Find("StartText").GetComponent<Text>();
        t4 = new string[,]{{ "m","1"},{ "r","2" },{ "s","3"},{ "r","1"},{ "s","2" },{ "m","3" } };
        linePositions = new List<Vector3>();
        sphere = GameObject.Find("Small Sphere");
        
        

        

        lr = sphere.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.SetColor("_Color", Color.green);
        lr.widthMultiplier = 0.01f;
        lr.positionCount = 0;

        //var b = GameObject.Find("targetBuildings");
        for (int i=0; i < modelObjects.transform.childCount; i++)
        {
            modelObjects.transform.GetChild(i).gameObject.SetActive(false);
            targetPlacement.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < targetPlacementt5.transform.childCount; i++)
        {
            targetPlacementt5.transform.GetChild(i).gameObject.SetActive(false);
            modelObjectst5.transform.GetChild(i).gameObject.SetActive(false);
        }


        //text = GameObject.Find("t").GetComponent<Text>();
        



        originalColor = targetPlacementt5.transform.GetChild(0).GetComponent<Renderer>().material.GetColor("_Color");

        while (File.Exists("Assets/Logs/participant" + participant + ".txt"))
        {
            participant++;
        }
        
        StreamWriter writer = new StreamWriter("Assets/Logs/participant"+participant+".txt", false);
        writer.WriteLine("participant,task,subtask,time_since_start,accuracy,accuracyrot,accuracyscale,overall_time,mode,is_end,exited,controller");
        writer.Close();
    
        icenter = gameObject.GetComponent<WIM_vive>();


        acc = new float[sm.t1_contr.GetLength(1), targetPlacementt5.transform.childCount*2, 3];
        times = new float[sm.t1_contr.GetLength(1), targetPlacementt5.transform.childCount*2];
    }

    void changeTask()
    {
        for (int i = 0; i < targetPlacement.transform.childCount; i++)
        {
            targetPlacement.transform.GetChild(i).gameObject.SetActive(false);
            modelObjects.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < targetPlacementt5.transform.childCount; i++)
        {
            targetPlacementt5.transform.GetChild(i).gameObject.SetActive(false);
            modelObjectst5.transform.GetChild(i).gameObject.SetActive(false);
        }
        time = 0;
        time_since_start = 0;
        subtask = 0;
        collision_count = 0;
        lr.positionCount = 0;
        linePositions = new List<Vector3>();
        old_task = task;
        started = false;
        started_path = false;
        
    }


    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    pause = !pause;
        //}


        //text.text = "";
        if (old_task != task)
        {

            changeTask();

            if (task==1 || task==2)
            {
                //acc = new float[sm.t1_contr.GetLength(1), targetPlacementt5.transform.childCount * 2, 3];
                //times = new float[sm.t1_contr.GetLength(1), targetPlacementt5.transform.childCount * 2];
                //Physics.autoSimulation = true;
                //player.GetComponent<PositionManager>().physicsOn = true;

                //wim.SetActive(true);
                wim.transform.localScale = Vector3.one;
                sphere.transform.localScale = Vector3.one;
                started = true;

            }
            else
            {

                wim.transform.localScale = Vector3.one;
                sphere.transform.localScale = Vector3.one;
            }
            
        }
       
       if (task == 1)
        {

            //GameObject.Find("model").SetActive(false);
            if (started & !pause)
            {
                if (!started_path)
                {
                    if(count==0)sm.NextController(1);
                    started_path = true;
                    targetPlacement.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject.SetActive(true);
                    modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject.SetActive(true);
                    //pm.contr = sm.rocks_contr[participant-1, subtask];
                }
                else
                {
                    
                    GameObject m = modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject;
                    GameObject t = targetPlacement.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject;
                    accuracy = Vector3.Distance(m.transform.position, t.transform.position);
                    rot_accuracy = Quaternion.Angle(m.transform.rotation, t.transform.rotation);
                    scale_accuracy = Vector3.Distance(m.transform.localScale, t.transform.localScale);
                    //text.text = "\nDistance: " + accuracy + "\n" +
                    //"Rotation: " + rot_accuracy + "\nScale" + scale_accuracy;

                    if (accuracy < accThreshold_task2 && rot_accuracy < rotThreshold_task2 && scale_accuracy < scaleThreshold)
                    {
                        
                        t.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                        time += Time.deltaTime;
                        if (time > timeThreshold)
                        {
                            icenter.movementStop = true;
                            t.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
                            
                            modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).localRotation = Quaternion.Euler(0,0,0);

                            modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).localPosition = Vector3.zero;
                            modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).transform.localScale = Vector3.one;
                            t.SetActive(false);
                            m.SetActive(false);
                            icenter.mode = WIM_vive.modeEnum.neutral;
                            icenter.time = 0;
                            icenter.updateProgress(icenter.timeThreshold);
                            write(1);
                            time = 0;

                            if (sm.currentController == PositionManager.controllerEnum.sphere)
                            {
                                acc[0, subtask, 0] = accuracy;
                                acc[0, subtask, 1] = rot_accuracy;
                                acc[0, subtask, 2] = scale_accuracy;

                                times[0, subtask] = time_since_start;
                                
                            }
                            if (sm.currentController == PositionManager.controllerEnum.controller)
                            {
                                acc[1, subtask, 0] = accuracy;
                                acc[1, subtask, 1] = rot_accuracy;
                                acc[1, subtask, 2] = scale_accuracy;
                                times[1, subtask] = time_since_start;
                               
                            }
                            if (sm.currentController == PositionManager.controllerEnum.controller_menu)
                            {
                                acc[2, subtask, 0] = accuracy;
                                acc[2, subtask, 1] = rot_accuracy;
                                acc[2, subtask, 2] = scale_accuracy;
                                times[2, subtask] = time_since_start;
                                
                            }
                            if (sm.currentController == PositionManager.controllerEnum.controller_arcball)
                            {
                                acc[3, subtask, 0] = accuracy;
                                acc[3, subtask, 1] = rot_accuracy;
                                acc[3, subtask, 2] = scale_accuracy;
                                times[3, subtask] = time_since_start;

                            }

                            subtask++;
                            icenter.setScale();
                            sphere.GetComponent<Renderer>().enabled = true;
                            time_since_start = 0;
                            
                            if (subtask < sm.subtask_order_task_rocks.Length)
                            {
                                modelObjects.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject.SetActive(true);
                                targetPlacement.transform.GetChild(sm.subtask_order_task_rocks[subtask]).gameObject.SetActive(true);
                                //pm.contr = sm.rocks_contr[participant - 1, subtask];
                                //pause = true;
                                icenter.nextSubtask1();

                                
                            }
                            else
                            {
                                count++;
                                Debug.Log("count: "+count);
                                if (count == 4)
                                {
                                    writeTaskPrecision(1);
                                    writeTaskTime(1);
                                    count = 0;
                                }
                                else
                                {
                                    sm.NextController(1);
                                }
                                
                                sm.task = 0;
                                subtask = 0;
                                accuracy = 0;
                                rot_accuracy = 0;
                                scale_accuracy = 0;
                                
                                started = false;
                                started_path = false;
                                
                                for (int i = 0; i < targetPlacement.transform.childCount; i++)
                                {
                                    targetPlacement.transform.GetChild(i).gameObject.SetActive(false);
                                    modelObjects.transform.GetChild(i).gameObject.SetActive(false);
                                }
                                icenter.time = 0;
                                icenter.updateProgress(icenter.timeThreshold);
                                icenter.movementStop = false;
                            }
                        }
                        else
                        {
                            write(0);
                        }
                    }
                    else
                    {
                        t.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
                        time = 0;
                        write(0);
                    }
                }

            }
            else
            {
                time_since_start = 0;
            }

        }

        else if (task == 2)
        {
            if (placed)
            {
                //Debug.Log("placed");
                //GameObject.Find("model").SetActive(false);
                if (started)
                {
                    if (!started_path)
                    {
                        if (count == 0) { sm.NextController(2); }
                        
                        for (int i = 0; i < targetPlacementt5.transform.childCount; i++)
                        {
                            targetPlacementt5.transform.GetChild(i).gameObject.SetActive(false);
                            modelObjectst5.transform.GetChild(i).gameObject.SetActive(false);
                        }
                        started_path = true;
                        modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject.SetActive(true);
                        targetPlacementt5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject.SetActive(true);
                        //pm.contr = sm.puzzle_contr[participant - 1, subtask];
                    }
                    else
                    {

                        GameObject m = modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject;
                        GameObject t = targetPlacementt5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject;
                        accuracy = Vector3.Distance(m.transform.position, t.transform.position);
                        rot_accuracy = Quaternion.Angle(m.transform.rotation, t.transform.rotation);
                        scale_accuracy = Vector3.Distance(m.transform.localScale, t.transform.localScale);
                        //text.text = "\nDistance: " + accuracy + "\n" +
                        //"Rotation: " + rot_accuracy + "\nScale" + scale_accuracy;

                        if (accuracy < accThreshold_task2 && rot_accuracy < rotThreshold_task2 && scale_accuracy < scaleThreshold)
                        {

                            t.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                            time += Time.deltaTime;
                            if (time > timeThreshold)
                            {
                                icenter.movementStop = true;
                                t.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
                                modelObjectst5.transform.GetChild(sm.subtask_order_task_rocks[subtask]).localRotation = Quaternion.Euler(0, 0, 0);

                                modelObjectst5.transform.GetChild(sm.subtask_order_task_rocks[subtask]).localPosition = Vector3.zero;
                                modelObjectst5.transform.GetChild(sm.subtask_order_task_rocks[subtask]).transform.localScale = Vector3.one;
                                m.transform.localRotation = Quaternion.identity;
                                m.transform.localPosition = Vector3.zero;
                                m.transform.localScale = Vector3.one;
                                t.SetActive(false);
                                m.SetActive(false);
                                icenter.mode = WIM_vive.modeEnum.neutral;
                                icenter.time = 0;
                                icenter.updateProgress(icenter.timeThreshold);
                                write(1);
                                time = 0;
                                

                                if (sm.currentController == PositionManager.controllerEnum.sphere)
                                {
                                    acc[0, subtask, 0] = accuracy;
                                    acc[0, subtask, 1] = rot_accuracy;
                                    acc[0, subtask, 2] = scale_accuracy;

                                    times[0, subtask] = time_since_start;
                                    Debug.Log("sphere "+accuracy + " "+ rot_accuracy+" "+ scale_accuracy+" "+time_since_start);
                                }
                                if (sm.currentController == PositionManager.controllerEnum.controller)
                                {
                                    acc[1, subtask, 0] = accuracy;
                                    acc[1, subtask, 1] = rot_accuracy;
                                    acc[1, subtask, 2] = scale_accuracy;
                                    times[1, subtask] = time_since_start;
                                    Debug.Log("c1 " + accuracy + " " + rot_accuracy + " " + scale_accuracy + " " + time_since_start);
                                }
                                if (sm.currentController == PositionManager.controllerEnum.controller_menu)
                                {
                                    acc[2, subtask, 0] = accuracy;
                                    acc[2, subtask, 1] = rot_accuracy;
                                    acc[2, subtask, 2] = scale_accuracy;
                                    times[2, subtask] = time_since_start;
                                    Debug.Log("c2 " + accuracy + " " + rot_accuracy + " " + scale_accuracy + " " + time_since_start);
                                }
                                if (sm.currentController == PositionManager.controllerEnum.controller_arcball)
                                {
                                    acc[3, subtask, 0] = accuracy;
                                    acc[3, subtask, 1] = rot_accuracy;
                                    acc[3, subtask, 2] = scale_accuracy;
                                    times[3, subtask] = time_since_start;
                                    Debug.Log("c3 " + accuracy + " " + rot_accuracy + " " + scale_accuracy + " " + time_since_start);
                                }


                                subtask++;
                                icenter.setScale();
                                icenter.nextSubtask1();
                                sphere.GetComponent<Renderer>().enabled = true;
                                icenter.hideMenu();
                                
                                if (subtask < sm.subtask_order_task_puzzle.Length)
                                {
                                    modelObjectst5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject.SetActive(true);
                                    targetPlacementt5.transform.GetChild(sm.subtask_order_task_puzzle[subtask]).gameObject.SetActive(true);
                                    //pm.contr = sm.puzzle_contr[participant - 1, subtask];
                                    //pause = true;
                                    time_since_start = 0;
                                    
                                }
                                else
                                {
                                    count++;
                                    if (count == 4)
                                    {
                                        writeTaskPrecision(2);
                                        writeTaskTime(2);
                                        count = 0;
                                    }
                                    else
                                    {
                                        sm.NextController(2);
                                    }
                                    sm.task = 0;
                                    subtask = 0;
                                    accuracy = 0;
                                    rot_accuracy = 0;
                                    scale_accuracy = 0;
                                    time_since_start = 0;
                                    started = false;
                                    started_path = false;

                                    for (int i = 0; i < targetPlacementt5.transform.childCount; i++)
                                    {
                                        targetPlacementt5.transform.GetChild(i).gameObject.SetActive(false);
                                        modelObjectst5.transform.GetChild(i).gameObject.SetActive(false);
                                    }
                                    icenter.time = 0;
                                    icenter.updateProgress(icenter.timeThreshold);
                                }
                            }
                            else
                            {
                                write(0);
                            }
                        }
                        else
                        {
                            t.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
                            time = 0;
                            write(0);
                        }
                    }






                }
                else
                {
                    time_since_start = 0;
                }

            }
        }
        if (task == 3)
        {
            
                testObject.SetActive(true);
                testPlacement.SetActive(true);

                accuracy = Vector3.Distance(testPlacement.transform.position, testObject.transform.position);
                rot_accuracy = Quaternion.Angle(testPlacement.transform.rotation, testObject.transform.rotation);
                scale_accuracy = Vector3.Distance(testPlacement.transform.localScale, testObject.transform.localScale);
                //text.text = "\nDistance: " + accuracy + "\n" +
                //"Rotation: " + rot_accuracy + "\nScale" + scale_accuracy;

                if (accuracy < accThreshold_task2 && rot_accuracy < rotThreshold_task2 && scale_accuracy < scaleThreshold)
                {

                    testPlacement.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                    time += Time.deltaTime;
                    if (time > timeThreshold)
                    {
                        icenter.movementStop = true;
                        testPlacement.GetComponent<Renderer>().material.SetColor("_Color", originalColor);

                        icenter.mode = WIM_vive.modeEnum.neutral;
                        icenter.time = 0;
                        icenter.updateProgress(icenter.timeThreshold);
                        write(1);
                        time = 0;

                        icenter.setScale();
                        sphere.GetComponent<Renderer>().enabled = true;
                        time_since_start = 0;
                        icenter.nextSubtask1();
                        
                        sm.task = 0;
                        subtask = 0;
                        accuracy = 0;
                        rot_accuracy = 0;
                        scale_accuracy = 0;

                        started = false;
                        started_path = false;

                        testPlacement.SetActive(false);
                        testObject.SetActive(false);
                                
                        icenter.time = 0;
                        icenter.updateProgress(icenter.timeThreshold);
                        icenter.movementStop = false;
                        

                    }
                    else
                    {
                        write(0);
                    }
                
                }
                else
                {
                    testPlacement.GetComponent<Renderer>().material.SetColor("_Color", originalColor);
                    time = 0;
                    write(0);
                }
            
        }

        time_since_start += Time.deltaTime;
        timeStamp += Time.deltaTime;

    }
    void write(int end)
    {
        //task,time_since_start,accuracy,overall_time
        StreamWriter writer = new StreamWriter("Assets/Logs/participant" + participant + ".txt", true);
        writer.WriteLine(participant+","+task + "," + subtask + "," + time_since_start + "," + accuracy + "," + rot_accuracy + "," + scale_accuracy + "," + timeStamp + "," + icenter.mode.ToString() + "," + end + ","+exited+","+pm.contr);
        
        writer.Close();
        
    }

    void writeTaskPrecision(int task)
    {
        StreamWriter writer = new StreamWriter("Assets/Logs/PrecT"+task+".txt", true);

        var res = "";
        for(int i=0; i<sm.t1_contr.GetLength(1); i++)
        {
            for(int j = 0; j < acc.GetLength(1); j++)
            {
                res = res + ","+acc[i, j, 0] + "," + acc[i, j, 1] + "," + acc[i, j, 2];
            }
            
        }


        writer.WriteLine(participant+res);

        writer.Close();
    }
    void writeTaskTime(int task)
    {
        StreamWriter writer = new StreamWriter("Assets/Logs/TimeT" + task + ".txt", true);

        var res = "";
        for (int i = 0; i < sm.t1_contr.GetLength(1); i++)
        {
            for (int j = 0; j < times.GetLength(1); j++)
            {
                res = res + "," + times[i, j];
            }
            
        }


        writer.WriteLine(participant+res);

        writer.Close();
    }

}
