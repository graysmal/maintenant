using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class timescript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int seconds = (int) GameManager.time;
        int minutes = (int) (GameManager.time / 60);
        seconds = seconds % 60;
        GetComponent<TextMeshProUGUI>().text = "TIME: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
