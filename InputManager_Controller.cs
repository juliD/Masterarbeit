using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.ObjectModel;

using Valve.VR.InteractionSystem;

public class InputManager_Controller : MonoBehaviour
{

    public Hand hand1,hand2;
    Hand hand;// The hand object
    Vector2 last, vDown,vDrag;
    WIM_vive wim;
    public bool pressed, released;
    PositionManager pm;
    int oldstate = 0;
    public Vector3 pressDownPos;

    void Start()
    {
        // Get the hand componenet
        //hand = GetComponent<Hand>();
        wim = GameObject.Find("InteractionCenter").GetComponent<WIM_vive>();
        pm = GameObject.Find("InteractionCenter").GetComponent<PositionManager>();
        hand = hand1;
        released = true;
    }

    void Update()
    {
        if (oldstate != pm.state)
        {
            oldstate = pm.state;
            if (oldstate == 0)
            {
                hand = hand1;
            }
            else
            {
                hand = hand2;
            }
        }
        // === NULL REFERENCE === //
        if (hand!=null && hand.controller != null)
        {
            

            if (hand.controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) || hand.controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) || hand.controller.GetPress(SteamVR_Controller.ButtonMask.Grip))
            {
                if (!pressed && released)
                {
                    wim.SetPressDown();
                }
                if (released)
                {
                    pressed = true;
                }
                
            }
            else
            {
                pressed = false;
                
            }
            
            if(hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) || hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) || hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                released = true;
            }


                Vector2 touchpad = (hand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            if (touchpad.x!=0 &&touchpad.y!=0)
            {

                if (last.x == 0 && last.y == 0)
                {
                    vDown = touchpad;
                    vDrag = touchpad;
                }
                else
                {
                    vDown = vDrag;
                    vDrag = touchpad;
                }

                wim.ControllerScale(vDrag.y - vDown.y);

            }
            last = touchpad;
        }
        
        
    }
}