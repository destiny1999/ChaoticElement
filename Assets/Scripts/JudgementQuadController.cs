using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementQuadController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color positionNoColor;
    //[SerializeField] Color normalColor;
    GameObject preTarget = null;
    Renderer renderer;
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
                    if(preTarget != null && preTarget != hit.transform.gameObject)
                    {
                        preTarget.GetComponent<BuildingPositionController>().ChangeColor(false);
                    }
                    preTarget = hit.transform.gameObject;
                    preTarget.GetComponent<BuildingPositionController>().ChangeColor(true);
                }
            }
            else
            {
                if (preTarget != null)
                {
                    preTarget.GetComponent<BuildingPositionController>().ChangeColor(false);
                    preTarget = null;
                }
                renderer.enabled = true;
                //renderer.material.color = positionNoColor;
                
            }
        }

    }
}
