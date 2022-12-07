using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPMonitorController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject hpValue;
    [SerializeField] GameObject hpMonitor;
    [SerializeField] float testValue = -1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpMonitor.transform.rotation = Quaternion.identity;
        if (Input.GetKeyDown(KeyCode.Space) && testValue != -1)
        {
            ChangeHpShowValue(testValue);
        }
        
    }
    /// <summary>
    /// min 0 max 1
    /// </summary>
    /// <param name="remainPercent"></param>
    public void ChangeHpShowValue(float remainPercent)
    {
        Vector3 scale = hpValue.transform.localScale;
        Vector3 position = hpValue.transform.localPosition;
        scale.x = remainPercent;
        position.x = -(1 - remainPercent) / 2;
        hpValue.transform.localPosition = position;
        hpValue.transform.localScale = scale;
    }
}
