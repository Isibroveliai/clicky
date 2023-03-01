using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public bool MouseClick {  set; get; }
    public bool MouseHold {  set; get; }
    private bool started;
    public Vector2 MousePositionScreen { get; private set; }
    public Vector2 MousePositionWorld { get; private set; }
   // private float clickStart;
   // private float delay = 0.3f;


    private void Awake()
    {
        MousePositionWorld = new Vector2();
        MousePositionScreen = new Vector2();
    }
    public void Click(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            started = true;
          //  clickStart = Time.time;
        }

        if (context.canceled)
        {
            MouseClick = true;
            
        }
        if (context.performed)
        {
            MouseHold = true;
        }
        else
        {
            MouseHold = false;
            //MouseClick = false;
        }
    }

    public void MoveMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MousePositionScreen = context.ReadValue<Vector2>();
            MousePositionWorld = Camera.main.ScreenToWorldPoint(MousePositionScreen);
        }
    }

    void Update()
    {
        //if(started)
        //{
        //    if(Time.time >= clickStart + delay)
        //    {
        //        MouseHold = true;
        //        started = false;
        //    }
        //}
        // Debug.Log(System.String.Format("click: {0} hold: {1}, mouse pos: {2}", MouseClick, MouseHold, MousePositionWorld));
        Debug.Log(MouseHold);
    }
    

}
