using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //public int serverCount;
    public List<GameObject> servers = new List<GameObject>();
    public List<string> ips = new List<string>();
    public float time = 0;
    public float frequency = 5; // seconds
    float next_time = 0;

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
            Debug.Log("removed one");
            GameObject selected_server = servers.ElementAt(Random.Range(0, servers.Count));
            
            servers.ElementAt(Random.Range(0, servers.Count)).GetComponent<Server>().status_light.color = Color.green;
            servers.ElementAt(Random.Range(0, servers.Count)).GetComponent<Server>().status_light.gameObject.SetActive(true);

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
