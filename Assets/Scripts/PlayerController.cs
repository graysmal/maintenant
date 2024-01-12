using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    float horInput;
    float verInput;
    float horMouseInput;
    float verMouseInput;

    GameObject camera;
    //Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        camera = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        updateInput();
        updateMovement();
        
    }

    void updateInput() {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");
        horMouseInput = Input.GetAxis("Mouse X");
        verMouseInput = Input.GetAxis("Mouse Y");
    }

    void updateMovement() {
        Vector3 accelerationVector = new Vector3(
            horInput, 
            0,
            verInput);
        transform.Translate(accelerationVector * 0.1f, Space.World);

        transform.Rotate(new Vector3(0, horMouseInput, 0), Space.World);
        transform.Rotate(new Vector3(-verMouseInput, 0, 0), Space.Self);
    }
}
