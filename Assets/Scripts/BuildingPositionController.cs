using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPositionController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color selectedColor;
    [SerializeField] Color notBuildColor;
    Color normalColor;
    Renderer renderer;
    [SerializeField] bool canUse = true;
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
        normalColor = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool JudgeCanBeBuilded()
    {
        return canUse;
    }
    public void ChangeColor(bool selected)
    {
        renderer.material.color = selected ? selectedColor : normalColor;
    }
    public void SetUseSituation(bool use)
    {
        renderer.material.color = normalColor;
        canUse = !use;
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("buildingJudgePlane"))
        {
            if (canUse)
            {
                renderer.material.color = selectedColor;
            }
            else
            {
                renderer.material.color = notBuildColor;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("buildingJudgePlane"))
        {
            renderer.material.color = normalColor;
        }
    }*/
}
