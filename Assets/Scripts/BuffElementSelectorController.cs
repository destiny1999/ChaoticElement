using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffElementSelectorController : MonoBehaviour
{
    [SerializeField] GameObject moveTarget;
    [SerializeField] GameObject rotateTarget;
    [SerializeField] GameObject openTrigger;
    [SerializeField] GameObject closeTrigger;
    public bool stop = true;
    public float moveSpeed;
    public float rotateSpeed;
    bool open = false;

    float tempTimeScale = -1;

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeBuffElementsStatus();
        }

    }
    public void ChangeBuffElementsStatus()
    {
        tempTimeScale = Time.timeScale == 0 ? tempTimeScale : Time.timeScale;
        open = !open;
        Time.timeScale = open ? 0 : tempTimeScale;
        this.gameObject.SetActive(open);
    }
    public void TriggerStop()
    {
        stop = true;
    }
}
