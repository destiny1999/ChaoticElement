using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSomething : MonoBehaviour
{
    public List<Status> statuList;
    public Status status;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (statuList.Contains(status))
            {
                print("contain");
            }
            else
            {
                print("no");
            }
        }
    }
}
