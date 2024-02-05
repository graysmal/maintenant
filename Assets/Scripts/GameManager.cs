using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public int serverCount;
    public List<GameObject> servers = new List<GameObject>();
    public List<GameObject> ongoing_event_servers = new List<GameObject>();
    public List<string> ips = new List<string>();
    public float time = 0;
    public float frequency = 5; // seconds
    float next_time = 0;

    public float yellow_event_time;
    public float red_event_time;

    public float progress;
    public float progress_to_end;

    // Start is called before the first frame update
    void Start()
    {
        next_time = frequency;
        generateIPs(700);
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > next_time) {
            next_time += frequency;
            startRandomEvent();

        }
    }

    public void startRandomEvent()
    {
        getAvailableServer();
        Server selected_server = getAvailableServer().GetComponent<Server>();
        // off event weight out of 100
        float off_w = 50;
        // yellow event weight out of 100
        float yel_w = 30;
        // red event weight out of 100
        float red_w = 20;
        float rand = Random.Range(1, 100);
        if (rand)

        selected_server.startEvent(Random.Range(0, 3));
    }

    public void startRandomEvent(int event_id)
    {
        getAvailableServer();
        Server selected_server = getAvailableServer().GetComponent<Server>();
        selected_server.startEvent(event_id);

    }

    public GameObject getAvailableServer() {
        GameObject server = servers.ElementAt(Random.Range(0, servers.Count));
        Server server_script = server.GetComponent<Server>();
        if (server_script.event_ongoing == true || server_script.still == false) {
            server = getAvailableServer();
        }
        return server;
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
