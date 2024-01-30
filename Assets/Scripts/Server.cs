using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Server : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject plug;
    public GameObject switch_object;
    public TextMeshPro ip_text;
    public Light status_light;
    public GameObject cord;
    Collider server_collider;

    public bool turnedOn;
    public bool ready;

    public string ip;
    // Start is called before the first frame update
    void Start()
    {
        //gameManager.serverCount++;
        gameManager.servers.Add(this.gameObject);

        plug = transform.GetChild(0).gameObject;
        switch_object = transform.GetChild(1).gameObject;
        ip_text = transform.GetChild(2).gameObject.GetComponent<TextMeshPro>();
        status_light = transform.GetChild(3).gameObject.GetComponent<Light>();
        cord = transform.parent.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        server_collider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (ip.Equals(""))
        {
            grabRandomIP();
        }
        if (!ip_text.Equals(ip)) { 
            ip_text.text = ip;
        }
    }

    void grabRandomIP() {
        Debug.Log(gameManager.ips.Count);
        string temp_ip = gameManager.ips.ElementAt(UnityEngine.Random.Range(0, gameManager.ips.Count));
        gameManager.ips.Remove(temp_ip);
        ip = temp_ip;
    }
    
}
