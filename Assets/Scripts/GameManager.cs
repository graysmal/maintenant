using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Server server;
    // Start is called before the first frame update
    void Start()
    {
        server = new Server();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(server.getServerCount());
    }
}
