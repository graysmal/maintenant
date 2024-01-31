using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<temptingObject>().visualizeTemptation();
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<temptingObject>().devisualizeTemptation();
    }

    private void OnMouseDown()
    {
        this.gameObject.transform.parent.GetComponent<Server>().turnedOn = !this.gameObject.transform.parent.GetComponent<Server>().turnedOn;
    }
}
