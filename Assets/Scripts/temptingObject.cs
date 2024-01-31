using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class temptingObject : MonoBehaviour
{
    // initializing variables for the player, highlight material, and a gameobject that will be the highlight
    public Material highlightMaterial;
    private GameObject highlight;
    private static Color rainbow = new Color(1, 0, 1, .5f);

    void Start()
    {
        // creating the highlight object that is basically this object with no scripts or collider with a transparent texture on it
        highlight = Instantiate(gameObject, transform, true); 
        Destroy(highlight.GetComponent<temptingObject>());
        Destroy(highlight.GetComponent<Collider>());
        highlight.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        highlight.GetComponent<Renderer>().material = highlightMaterial;
        //StartCoroutine(rainbowProgression());
        devisualizeTemptation();
    }

    void Update()
    {  
        
    }

    public void visualizeTemptation ()
    {
        highlight.gameObject.SetActive(true);
    }

    public void devisualizeTemptation()
    {
        highlight.gameObject.SetActive(false);
    }

    public IEnumerator rainbowProgression()
    {
        while (true)
        {
            if (rainbow.r == 1f && rainbow.g < 1f)
            {
                rainbow.b = Mathf.Clamp(rainbow.b - 0.01f, 0, 1);
                rainbow.g = Mathf.Clamp(rainbow.g + 0.01f, 0, 1);
            }
            if (rainbow.g == 1f && rainbow.b < 1f)
            {
                rainbow.r = Mathf.Clamp(rainbow.r - 0.01f, 0, 1);
                rainbow.b = Mathf.Clamp(rainbow.b + 0.01f, 0, 1);
            }
            if (rainbow.b == 1f && rainbow.r < 1f)
            {
                rainbow.g = Mathf.Clamp(rainbow.g - 0.01f, 0, 1);
                rainbow.r = Mathf.Clamp(rainbow.r + 0.01f, 0, 1);
            }
            highlightMaterial.SetColor("_Color", rainbow);
            yield return new WaitForSeconds(0.005f);
        }
    }
}
