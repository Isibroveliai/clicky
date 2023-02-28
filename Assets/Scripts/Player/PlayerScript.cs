using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe not needed?
public class PlayerScript : MonoBehaviour
{
    Inputs inputs;
    void Start()
    {
        inputs = FindAnyObjectByType<Inputs>();
    }

    void FixedUpdate()
    {
        //CheckMouseHover();
    }

    //public void CheckMouseHover()
    //{
    //    Vector3 mouseToWorldPos = Camera.main.ScreenToWorldPoint(inputs.MousePosition);
    //    RaycastHit2D hit = Physics2D.Raycast(new Vector2(mouseToWorldPos.x, mouseToWorldPos.y), Vector2.zero, 0f);

    //    if(hit.collider != null )
    //    {   

    //        if(inputs.MouseClick)
    //        {
                
    //            inputs.SetMouseClickFalse();
    //        }
           
    //    }
    //}
}
