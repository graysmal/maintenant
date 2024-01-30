using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int serverCount;
    public List<string> ips = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        generateIPs(700);
    }
    // Update is called once per frame
    void Update()
    {

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
