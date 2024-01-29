using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeParent : MonoBehaviour
{

    public GameObject top;
    public GameObject top_parent;
    public GameObject bottom;
    public GameObject bottom_parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(top_parent.transform.position + " | " + top.transform.position + " || " + bottom_parent.transform.position + " | " + bottom.transform.position);
        top.transform.position = top_parent.transform.position;
        bottom.transform.position = bottom_parent.transform.position;
        
    }
}
