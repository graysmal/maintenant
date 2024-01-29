using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    public static int serverCount;
    public bool enabled;
    public bool ready;
    // Start is called before the first frame update
    void Start()
    {
        enabled = true;
        serverCount++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getServerCount()
    {
        return serverCount;
    }
}
