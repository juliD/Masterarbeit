using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Globalization;

public class RotationInteraction : MonoBehaviour {

    public GameObject small;
    public GameObject big;
    public int scene_id = 1;
    public float offset = 20;
    private int old_id;
    private float calibration; 

    private float time_stamp;
    private bool calibrated;
    string calibration_path = "Assets/Resources/calibration.txt";
    public bool with_small;
    public Vector3 rotation_calib;
    public Quaternion calib_rot;
    public bool trial_phase = true;

    public float Time_Stamp()
    {
        return time_stamp;

    }
    public float get_calibration()
    {
        return calibration;
    }

	// Use this for initialization
	void Start () {
        old_id = scene_id;
        change_scene();
        small.transform.rotation = calib_rot;
        

        time_stamp = 0f;
        calibration = 0f;
        calibrated = false;

    }
	
	// Update is called once per frame
	void Update () {
        //calib_rot = Quaternion.Euler(rotation_calib);

        if (!calibrated)
        {
            
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(calibration_path);
            string calib = reader.ReadToEnd();
            if(!calib.Equals(""))
            {
                calibration = float.Parse(calib, CultureInfo.InvariantCulture.NumberFormat);
                float temp = 0;
                if (scene_id > 2)
                {
                    temp = offset;
                }
                big.transform.position = new Vector3(0, calibration, temp);
                calibrated = true;
            }
            reader.Close();


        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Saving Calibration");
            calibration = big.transform.position.y;
            calibrated = true;
            Debug.Log(calibration);
            StreamWriter writer = new StreamWriter("Assets/Resources/calibration.txt", false);
            writer.WriteLine(calibration);
            writer.Close();
        }



        if (old_id != scene_id)
        {
            change_scene();
        }

        switch (scene_id)
        {
            case 1:
                //position();
                rotate_inv();
                break;
            case 2:
                //position();
                rotate();
                break;
            case 3:
                rotate_inv();
                break;
            case 4:
                rotate();
                break;
            default:
                Debug.LogError("Invalid scene ID");
                break;
        }
        time_stamp += Time.deltaTime;  

         
    }
    void position()
    {
        big.transform.position = small.transform.position;
    }

    //inverse rotation
    void rotate_inv()
    {

        Quaternion calib;
        if (scene_id == 3)
        {
            calib = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            calib = Quaternion.Euler(-90, 0, 0);
        }

        big.transform.rotation = new Quaternion(small.transform.rotation.x,
            small.transform.rotation.y,
            small.transform.rotation.z * -1,
            small.transform.rotation.w * -1)*calib;
    }

    //normal rotation
    void rotate()
    {
        big.transform.rotation = small.transform.rotation;
    }

    //change materials and position when scene ID changes 
    void change_scene()
    {
        
        Material front = (Material)Resources.Load("frontCull_inv", typeof(Material));
        Material back = (Material)Resources.Load("backCull", typeof(Material));
        Vector3 pos = new Vector3(0, 0, offset);
        pos.y = calibration;
        

        switch (scene_id)
        {



            case 1:
                small.GetComponent<Renderer>().material = back;
                big.GetComponent<Renderer>().material = front;
                big.transform.localScale = new Vector3(1f, 1f, 1f);
                big.transform.position = new Vector3(0,calibration,0);
                small.transform.rotation = Quaternion.Euler(0, 0, 0);

                break;
            case 2:
                small.GetComponent<Renderer>().material = front;
                big.GetComponent<Renderer>().material = front;
                big.transform.localScale = new Vector3(1f, 1f, 1f);
                big.transform.position = new Vector3(0, calibration, 0);
                small.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 3:
                small.GetComponent<Renderer>().material = front;
                big.GetComponent<Renderer>().material = back;
                big.transform.localScale = new Vector3(2f,2f,2f);
                big.transform.position = pos;
                small.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 4:
                small.GetComponent<Renderer>().material = back;
                big.GetComponent<Renderer>().material = back;
                big.transform.localScale = new Vector3(2f, 2f, 2f);
                big.transform.position = pos;
                small.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            default:
                break;
        }
        old_id = scene_id;
    }
}
