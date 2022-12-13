using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementQuadController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color positionNoColor;
    //[SerializeField] Color normalColor;
    GameObject currentTarget = null;
    Renderer renderer;
    bool positionOK = false;
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
        renderer.material.color = positionNoColor;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            renderer.enabled = false;
            //renderer.material.color = normalColor;
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.transform.CompareTag("buildingPosition"))
            {
                if(hit.transform.GetComponent<BuildingPositionController>().JudgeCanBeBuilded())
                {
                    if(currentTarget != null && currentTarget != hit.transform.gameObject)
                    {
                        currentTarget.GetComponent<BuildingPositionController>().ChangeColor(false);
                    }
                    currentTarget = hit.transform.gameObject;
                    positionOK = true;
                    if (transform.GetComponentInParent<BuildingController>().AllPositionOK())
                    {
                        currentTarget.GetComponent<BuildingPositionController>().ChangeColor(true);
                    }
                    
                }
                else
                {
                    if (currentTarget != null)
                    {
                        currentTarget.GetComponent<BuildingPositionController>().ChangeColor(false);
                        currentTarget = null;
                    }
                    positionOK = false;
                    renderer.enabled = true;
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    currentTarget.GetComponent<BuildingPositionController>().ChangeColor(false);
                    currentTarget = null;
                }
                renderer.enabled = true;
                positionOK = false;
                //renderer.material.color = positionNoColor;
                
            }
        }

    }
    public bool CheckPositionStatus()
    {
        return positionOK;
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

}
