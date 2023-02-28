using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainInteractable : InteractableObject
{
   
    
    void Start()
    {
       
    }

    //BUG: score registers after clicking outside object, holding mouse button , hovering over and releasing button
    public override void OnInteract()
    {
        base.OnInteract();
        ClickerManager.Instance.Score++;
        Debug.Log(System.String.Format("CURRENT SCORE: {0}", ClickerManager.Instance.Score));
    }
}
