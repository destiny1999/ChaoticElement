using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPositionController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Color selectedColor;
    Color normalColor;
    Renderer renderer;
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
        normalColor = renderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseOver()
    {
        renderer.material.color = selectedColor;
    }
    private void OnMouseExit()
    {
        renderer.material.color = normalColor;
    }
}
