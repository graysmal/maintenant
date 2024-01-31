using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Server : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject plug;
    public GameObject switch_object;
    public TextMeshPro ip_text;
    public Light status_light;
    public GameObject cord;

    public bool permanently_disabled;
    private bool _turnedOn;
    public bool turnedOn { 
        get { return  _turnedOn; }
        set {
            if (_turnedOn == false & value == true)
            {
                if (permanently_disabled == false)
                {
                    if (status_light.color == Color.yellow) { 
                        
                    }
                    switch_object.transform.localEulerAngles = Vector3.right * 30;
                    status_light.gameObject.SetActive(true);
                }
                else {
                    Debug.Log("Server " + ip + " tried to turn on, it failed.");
                }
            }
            else if (_turnedOn == true && value == false) {
                switch_object.transform.localEulerAngles = Vector3.right * -30;
                status_light.gameObject.SetActive(false);
            }
            _turnedOn = value;
        }
    }
    public bool still;
    public bool event_ongoing;
    public enum ServerEvent { YELLOW, RED }
    public ServerEvent current_event;

    public string ip;

    public float rate;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        //gameManager.serverCount++;
        gameManager.servers.Add(this.gameObject);
        turnedOn = true;
        plug = transform.GetChild(0).gameObject;
        switch_object = transform.GetChild(1).gameObject;
        ip_text = transform.GetChild(2).gameObject.GetComponent<TextMeshPro>();
        status_light = transform.GetChild(3).gameObject.GetComponent<Light>();
        cord = transform.parent.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        //server_collider = GetComponent<Collider>();

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
        if (_turnedOn) {
            gameManager.progress += rate * Time.deltaTime;
        }
    }

    public void startEvent(int eventID) {
        switch (eventID) {
            // if server only needs to be turned on.
            case 0:
                turnedOn = false;
                break;
            // if server only needs to be turned off and back on
            case 1:
                event_ongoing = true;
                current_event = ServerEvent.YELLOW;
                StartCoroutine(blinkLight(Color.yellow, 12));
                break;
            // if server needs to have ip entered on computer
            case 2:
                event_ongoing = true;
                current_event = ServerEvent.RED;
                StartCoroutine(blinkLight(Color.red, 12));
                break;
        }
    }

    void grabRandomIP() {
        Debug.Log(gameManager.ips.Count);
        string temp_ip = gameManager.ips.ElementAt(UnityEngine.Random.Range(0, gameManager.ips.Count));
        gameManager.ips.Remove(temp_ip);
        ip = temp_ip;
    }

    IEnumerator blinkLight(Color color, float time) {
        float starting_time = timer;
        //time = time + starting_time;
        status_light.color = color;
        while (event_ongoing) {
            if ((timer + starting_time) >= time)
            {
                permanently_disabled = true;
                turnedOn = false;
                yield break;
            }
            Debug.Log(timer);
            status_light.gameObject.SetActive(!status_light.gameObject.activeSelf);
            yield return new WaitForSeconds((-0.8f / time)*((timer - starting_time)) + 1f);
        }
    }
}
