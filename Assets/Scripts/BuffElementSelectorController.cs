using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffElementSelectorController : MonoBehaviour
{
    [SerializeField] GameObject moveTarget;
    [SerializeField] GameObject rotateTarget;
    public bool stop = true;
    public float moveSpeed;
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(ChangeBuffElementsStatus(true));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(ChangeBuffElementsStatus(false));
        }
    }
    IEnumerator ChangeBuffElementsStatus(bool open)
    {
        Vector3 rotateDirection = open ? Vector3.down : Vector3.up;
        Vector3 moveDirection = open ? Vector3.right : Vector3.left;
        //float targetPos = open ? 0 : -1545f;
        stop = false;
        while(!stop)
        {
            //print(Mathf.Abs(moveTarget.GetComponent<RectTransform>().anchoredPosition.x));
            moveTarget.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            rotateTarget.transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime);
            yield return null;
        }
        /*
        moveTarget.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(targetPos,
                        moveTarget.GetComponent<RectTransform>().anchoredPosition.y);*/
    }
    public void TriggerStop()
    {
        stop = true;
    }
}
