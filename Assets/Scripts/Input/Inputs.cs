using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public bool MouseClick { private set; get; }
    public bool MouseHold { private set; get; }
    public Vector2 MousePosition { get; private set; }
    
    private void OnEnable()
    {
        
    }
    private void Awake()
    {
        
        MousePosition = new Vector2();
    }
    public void Click(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if(context.canceled)
        {
            MouseClick = true;
        }
        else
        {
            MouseClick = false;
        }

        if (context.performed)
        {
            MouseHold = true;
        }
        else
        {
            MouseHold = false;
        }
    }
    public void Space(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }
    public void MoveMouse(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if(context.performed)
        {
            MousePosition = context.ReadValue<Vector2>();
        }
    }

    //private void Update()
    //{
    //    Debug.Log(System.String.Format("Click: {0} Hold: {1}, Mouse Pos: {2}", MouseClick, MouseHold, MousePosition));
    //}
}
