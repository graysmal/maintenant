using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Server : MonoBehaviour
{
    public GameManager gameManager;

    public AudioSource a_source;

    public GameObject plug;
    public GameObject switch_object;
    public TextMeshPro ip_text;
    public Light status_light;
    public GameObject cord;

    public float off_position_offset;

    public bool permanently_disabled;
    private bool _turnedOn;
    public bool turnedOn { 
        get { return  _turnedOn; }
        set {
            if (_turnedOn == false & value == true)
            {
                if (event_ongoing && current_event == ServerEvent.OFF) { 
                    event_ongoing = false;
                }
                if (permanently_disabled == false)
                {
                    switch_object.transform.localEulerAngles = Vector3.right * 30;
                    status_light.gameObject.SetActive(true);
                }
            }
            else if (_turnedOn == true && value == false) {
                switch_object.transform.localEulerAngles = Vector3.right * -30;
                status_light.gameObject.SetActive(false);
                if (event_ongoing && current_event == ServerEvent.YELLOW)
                {
                    event_ongoing = false;
                    status_light.color = Color.green;
                }
                else if (event_ongoing && current_event == ServerEvent.RED) {
                    disableServer();
                }
            }
            _turnedOn = value;
        }
    }
    public bool still;
    private bool _event_ongoing;
    public bool event_ongoing {
        get { return _event_ongoing; }
        set {
            // set to true
            if (_event_ongoing == false && value == true)
            {
                gameManager.servers.Remove(this.gameObject);
                //disable coroutines was here idk why
                status_light.gameObject.SetActive(false);
                gameManager.ongoing_event_servers.Add(this.gameObject);
            }
            // set to false
            else if (_event_ongoing == true && value == false) {
                gameManager.ongoing_event_servers.Remove(this.gameObject);
                if (!permanently_disabled) {
                    gameManager.servers.Add(this.gameObject);
                }
            }
            
            _event_ongoing = value;
        }
    }
    public enum ServerEvent { YELLOW, RED, OFF }
    public ServerEvent current_event;

    public string ip;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.servers.Add(this.gameObject);
        turnedOn = true;
        plug = transform.GetChild(0).gameObject;
        switch_object = transform.GetChild(1).gameObject;
        ip_text = transform.GetChild(2).gameObject.GetComponent<TextMeshPro>();
        status_light = transform.GetChild(3).gameObject.GetComponent<Light>();
        cord = transform.parent.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        a_source = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) {
            disableServer();
        }
        // could be more efficient if grabbed ip in coroutine
        if (ip.Equals(""))
        {
            grabRandomIP();
        }
        if (!ip_text.Equals(ip)) { 
            ip_text.text = ip;
        }

        if (_turnedOn) {
            gameManager.progress += gameManager.rate * Time.deltaTime;
        }
    }

    public void startEvent(int eventID) {
        switch (eventID) {
            // if server only needs to be turned on.
            case 0:
                event_ongoing = true;
                current_event = ServerEvent.OFF;
                turnedOn = false;
                a_source.clip = gameManager.off_sfx;
                a_source.Play();
                break;
            // if server only needs to be turned off and back on
            case 1:
                event_ongoing = true;
                current_event = ServerEvent.YELLOW;
                a_source.clip = gameManager.yel_sfx;
                a_source.Play();
                StartCoroutine(blinkLight(Color.yellow, gameManager.yellow_event_time));
                break;
            // if server needs to have ip entered on computer
            case 2:
                event_ongoing = true;
                current_event = ServerEvent.RED;
                a_source.clip = gameManager.red_sfx;
                a_source.Play();
                StartCoroutine(blinkLight(Color.red, gameManager.red_event_time));
                break;
        }
        
    }

    void disableServer() {
        permanently_disabled = true;
        event_ongoing = false;
        turnedOn = false;
        a_source.clip = gameManager.perm_off;
        a_source.Play();
        StopAllCoroutines();
        try
        {
            gameManager.servers.Remove(this.gameObject);
            gameManager.ongoing_event_servers.Remove(this.gameObject);
        }
        catch (Exception e) {
            UnityEngine.Debug.Log(e.StackTrace);
        }
        // if user lets a red server die, spawn three more red events
        if (current_event == ServerEvent.RED)
        {
            for (int i = 0; i < 2; i++)
            {
                if (gameManager.servers.Count > 0)
                {
                    gameManager.startRandomEvent(2);
                }
            }

        }
        transform.parent.position = transform.parent.position + transform.parent.up * off_position_offset;
        status_light.gameObject.SetActive(false);
    }

    void grabRandomIP() {
        string temp_ip = gameManager.ips.ElementAt(UnityEngine.Random.Range(0, gameManager.ips.Count));
        gameManager.ips.Remove(temp_ip);
        ip = temp_ip;
    }

    IEnumerator blinkLight(Color color, float time) {
        float starting_time = GameManager.time;
        status_light.color = color;
        while (event_ongoing) {
            if (GameManager.time >= (starting_time + time))
            {
                disableServer();
                yield break;
            }
            status_light.gameObject.SetActive(!status_light.gameObject.activeSelf);
            yield return new WaitForSeconds((-0.8f / time)*((GameManager.time - starting_time)) + 1f);
        }
    }
}
