using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public int serverCount;

    public AudioClip off_sfx;
    public AudioClip yel_sfx;
    public AudioClip red_sfx;
    public AudioClip perm_off;
    public AudioClip pick_up_sfx;
    public AudioClip put_down_sfx;
    public AudioClip switch_on;
    public AudioClip switch_off;

    public List<GameObject> servers = new List<GameObject>();
    // fix this code w
    public GameObject test;
    public float testVar;

    public GameObject progress_bar;
    Image progress_bar_fill;
    TextMeshProUGUI eta_text;
    TextMeshProUGUI time_text;
    public float volume;
    public float time_scale;

    public List<GameObject> ongoing_event_servers = new List<GameObject>();
    public List<string> ips = new List<string>();
    public float time = 0;
    public float frequency; // seconds
    float next_time = 0;

    public float yellow_event_time;
    public float red_event_time;

    public float rate;
    int seconds;
    int minutes;
    string eta;
    int seconds_remaining;
    int minutes_remaining;
    public float progress;
    public float progress_to_end;

    // Start is called before the first frame update
    void Start()
    {
        next_time = frequency;
        generateIPs(700);
        StartCoroutine(renderCam());
        progress_bar_fill = progress_bar.transform.GetChild(0).GetComponent<Image>();
        eta_text = progress_bar.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        time_text = progress_bar.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        AudioListener.volume = volume;
        Time.timeScale = time_scale;
        if (frequency > 0.5f) {
            frequency = (-(1f / 150f) * time) + 3f;
        }
        

        if (time > next_time) {
            next_time += frequency;
            startRandomEvent();

        }
        seconds_remaining = (int) ((progress_to_end - progress) / (rate * (servers.Count + ongoing_event_servers.Count)));
        if (seconds_remaining < 0)
        {
            seconds_remaining = 0;
        }
        minutes_remaining = seconds_remaining / 60;
        seconds_remaining %= 60;
        eta = "ETA: " + minutes_remaining + ":" + seconds_remaining.ToString("D2");
        eta_text.text = eta;
        progress_bar_fill.fillAmount = progress / progress_to_end;

        seconds = (int) time;
        minutes = seconds / 60;
        seconds %= 60;
        time_text.text = "TIME: " + minutes + ":" + seconds.ToString("D2");
    }

    IEnumerator renderCam() {
        while (true) {
            test.GetComponent<Camera>().Render();
            yield return new WaitForSeconds(testVar);
        }

    }

    public void startRandomEvent()
    {
        getAvailableServer();
        Server selected_server = getAvailableServer().GetComponent<Server>();
        // off event weight out of 100
        float off_w = 50;
        // yellow event weight out of 100
        float yel_w = ((-15f/300f) * time) + 50;
        // red event weight out of 100
        float red_w = ((15f/300f) * time);
        float chance_total = off_w + yel_w + red_w;
        float rand = UnityEngine.Random.Range(1, chance_total);
        if (rand < off_w)
        {
            selected_server.startEvent(0);
        }
        else if (rand < (off_w + yel_w)) {
            selected_server.startEvent(1);
        }
        else
        {
            selected_server.startEvent(2);
        }

    }

    public void startRandomEvent(int event_id)
    {
        getAvailableServer();
        Server selected_server = getAvailableServer().GetComponent<Server>();
        selected_server.startEvent(event_id);

    }

    public GameObject getAvailableServer() {
        if (servers.Count == 0)
        {
            return null;
        }
        else {
            GameObject server = servers.ElementAt(UnityEngine.Random.Range(0, servers.Count));
            Server server_script = server.GetComponent<Server>();
            if (server_script.event_ongoing == true || server_script.still == false)
            {
                if (servers.Count <= 1)
                {
                    return null;
                }
                else {
                    server = getAvailableServer();
                }
                
            }
            return server;
        }
    }

    public void generateIPs(int amount)
    {
        string root = "192.168.";
        int differentiator = 0;
        for (int i = 1; i < amount + 1; i++)
        {
            if (i % 255 == 0)
            {
                differentiator += 1;
            }
            string ip = root + differentiator + "." + (i % 255);
            ips.Add(ip);
        }
    }

}
