using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static int serverCount;
    public static string[] IPs = { };
    public bool turnedOn;
    public bool ready;
    // Start is called before the first frame update
    void Start()
    {
        turnedOn = true;
        serverCount++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void generateIPs(int amount)
    {
        string[] ips = new string[amount];
        string root = "192.168.";
        int differentiator = 0;
        for (int i = 1; i < amount; i++)
        {
            if (i % 255 == 0) {
                differentiator += 1;
            }
            if (i > 255) {
                string ip = root + differentiator + "." + (i % 255);
                Debug.Log(ip);
                ips[i] = ip;

            }
        }
        IPs = ips;
    }

    public int getServerCount()
    {
        return serverCount;
    }
}
