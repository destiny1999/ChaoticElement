using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropElementController : MonoBehaviour
{
    [SerializeField] GameAttribute attribute;
    Vector3 targetPosition;
    bool used = false;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = GameObject.Find("MagicPet").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.Find("Fire").LookAt(-targetPosition);
        this.transform.position = 
            Vector3.MoveTowards(this.transform.position, targetPosition, 3 * Time.deltaTime);
    }
    public GameAttribute GetAttribute()
    {
        return attribute;
    }
    public void SetUsedStatus()
    {
        used = true;
    }
    public bool GetUsedStatus()
    {
        return used;
    }
}
