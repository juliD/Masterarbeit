using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudyManager : MonoBehaviour
{
    TaskManager_vive tm;
    WIM_vive wim;
    PositionManager pm;
    public int participant;
    public int[] subtask_order_task_rocks;
    public int[] subtask_order_task_puzzle;
    public PositionManager.controllerEnum currentController;
    public PositionManager.controllerEnum[,] t1_contr, t2_contr;
    public int task =0;
    public bool switchController;
    public float calib;
    public GameObject canv, contrImage, sphereImage;
    public Text text;
    TTSUnityWin tt;
    // Start is called before the first frame update
    void Start()
    {
        tm = GameObject.Find("InteractionCenter").GetComponent<TaskManager_vive>();
        pm = GameObject.Find("InteractionCenter").GetComponent<PositionManager>();
        wim = GameObject.Find("InteractionCenter").GetComponent<WIM_vive>();
        tt = gameObject.GetComponent<TTSUnityWin>();
        participant = tm.getParticipant();
        subtask_order_task_rocks = new int[] {
             0, 1, 2, 3, 0, 1, 2, 3,

        };
        subtask_order_task_puzzle = new int[] {
             0, 1, 2, 3, 0, 1, 2, 3

        };
        //subtask_order_task_rocks = new int[] {
        //     0

        //};
        //subtask_order_task_puzzle = new int[] {
        //     0

        //};
        canv.SetActive(false);
        t1_contr = new PositionManager.controllerEnum[,]
        {
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
        };
        t2_contr = new PositionManager.controllerEnum[,]
        {
            
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball},
            {PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller},
            {PositionManager.controllerEnum.controller, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.sphere},
            {PositionManager.controllerEnum.sphere, PositionManager.controllerEnum.controller_menu,PositionManager.controllerEnum.controller_arcball, PositionManager.controllerEnum.controller},
        };
    }
    IEnumerator switchContr(int task)
    {
        //this.task = 0;
        canv.SetActive(true);
        sphereImage.SetActive(false);
        contrImage.SetActive(false);
        if (currentController == PositionManager.controllerEnum.sphere)
        {
            sphereImage.SetActive(true);
            text.text = "Sphere";
        }
        else
        {
            contrImage.SetActive(true);
            if(currentController == PositionManager.controllerEnum.controller)
            {
                text.text = "Controller without Buttons";
            }
            else if(currentController == PositionManager.controllerEnum.controller_menu)
            {
                text.text = "Controller with a Button";
            }
            else
            {
                text.text = "Controller with Buttons and Touchpad";
            }
        }
        tt.controllerChange(currentController);
        yield return new WaitForSeconds(5);
        sphereImage.SetActive(false);
        contrImage.SetActive(false);
        canv.SetActive(false);
        this.task = task;
    }
    public void NextController(int task)
    {
        Debug.Log(task);
        Debug.Log(participant);
        if (task == 1)
        {
            currentController = t1_contr[tm.getParticipant() - 1, tm.count];
        }
        if (task == 2)
        {
            currentController = t2_contr[tm.getParticipant() - 1, tm.count];
        }
        StartCoroutine(switchContr(task));
        
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentController = PositionManager.controllerEnum.controller_arcball;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            currentController = PositionManager.controllerEnum.controller;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentController = PositionManager.controllerEnum.sphere;
        }
        if (Input.GetKey("1"))
        {
            task = 1;
        }

        if (Input.GetKey("2"))
        {
            task = 2;
        }

        if (Input.GetKey("0"))
        {
            task = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switchController=true;
        }
        pm.contr = currentController;
        tm.task = task;
    }
}
