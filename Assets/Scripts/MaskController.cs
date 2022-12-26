using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskController : MonoBehaviour
{
    [SerializeField] GameObject[] maskObject;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < maskObject.Length; i++)
        {
            maskObject[i].GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
