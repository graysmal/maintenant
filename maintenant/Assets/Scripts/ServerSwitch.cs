using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSwitch : MonoBehaviour
{
    public GameManager gameManager;
    GameObject server;
    // Start is called before the first frame update
    void Start()
    {
        server = transform.parent.gameObject;
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
        if (server.GetComponent<Server>().turnedOn)
        {
            server.GetComponent<Server>().a_source.PlayOneShot(gameManager.switch_off);
        }
        else {
            server.GetComponent<Server>().a_source.PlayOneShot(gameManager.switch_on);
        }
        server.GetComponent<Server>().turnedOn = !server.GetComponent<Server>().turnedOn;
    }
}
