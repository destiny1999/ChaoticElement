using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float moveSpeed;
    [SerializeField] float esistTime;
    void Start()
    {
        StartCoroutine(MoveUpAndDisppear());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator MoveUpAndDisppear()
    {
        while(esistTime >0)
        {
            esistTime -= Time.deltaTime * 1;
            this.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
