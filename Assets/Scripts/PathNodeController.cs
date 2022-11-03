using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeController : MonoBehaviour
{
    [SerializeField] Transform nextPathNode;
    [SerializeField] bool lastNode = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Transform GetNext()
    {
        return nextPathNode;
    }
    public bool CheckLastNode()
    {
        return lastNode;
    }
}
