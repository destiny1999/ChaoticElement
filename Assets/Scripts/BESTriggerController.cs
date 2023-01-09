using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BESTriggerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject BES;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BES.GetComponent<BuffElementSelectorController>().TriggerStop();
    }
}
