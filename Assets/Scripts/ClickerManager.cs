using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager Instance { get; private set; }
    public int Score { get; set; }
 
    Inputs inputs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        inputs = GetComponent<Inputs>();
        Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCurrency() => Score++;

}
