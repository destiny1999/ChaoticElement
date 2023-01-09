using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPowerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetColor(Color color, bool canChange)
    {
        this.GetComponent<Collider2D>().enabled = canChange;
        this.GetComponent<SpriteRenderer>().color = color;
        StartCoroutine(ChangeColorAlpha());
    }
    IEnumerator ChangeColorAlpha()
    {
        while(this.GetComponent<SpriteRenderer>().color.a < 1)
        {
            Color color = this.GetComponent<SpriteRenderer>().color;
            color.a += Time.deltaTime * 0.5f;
            this.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
    }
    private void OnMouseDown()
    {
        print("click");
    }
}
