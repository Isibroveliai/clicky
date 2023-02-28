using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for any mouse-interactable object
/// 
/// TODO:
/// add functionality as needed
/// </summary>
public class InteractableObject : MonoBehaviour
{
    protected BoxCollider2D boxCollider;
    protected Inputs inputs;
   
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
       // inputs = (Inputs)FindObjectOfType(typeof(Inputs));
        inputs = (Inputs)FindAnyObjectByType(typeof(Inputs));
    }
  
    void Update()
    {
        CheckMouse();
    }
    
    private void CheckMouse()
    {
        if (boxCollider.bounds.Contains(inputs.MousePositionWorld))
        {
            OnHover();
            if(inputs.MouseClick)
            {
                inputs.MouseClick = false;
                OnInteract();

            }
            
        }
        else
        {
            inputs.MouseClick = false;
        }
       
    }
    public virtual void OnInteract()
    {
        Debug.Log(System.String.Format("Interact with: {0}", this.name));
    }
    public virtual void OnHover()
    {
        Debug.Log(System.String.Format("Hover Over: {0}", this.name));
    }
}
