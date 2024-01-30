using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    PlayerController controller;

    bool looking_at_server;
    bool inspecting_server;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isPlayerLooking("server"))
        {
            looking_at_server = true;
        }
        else { looking_at_server = false; }

        if (!inspecting_server && looking_at_server && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(inspectServer());
        }
    }

    IEnumerator inspectServer() {
        GameObject server = controller.lookingAt();
        Server serverScript = server.GetComponent<Server>();

        // if server isn't ready to be interacted with again, cancel coroutine
        if (serverScript.still == false)
        {
            yield break;
        }
        // if server is ready, make not ready for interaction again
        else {
            server.GetComponent<Server>().still = false;
        }

        Cursor.lockState = CursorLockMode.None;
        controller.can_move = false;
        inspecting_server = true;

        serverScript.plug.SetActive(true);
        serverScript.switch_object.SetActive(true);
        serverScript.cord.SetActive(true);
        serverScript.ip_text.gameObject.SetActive(true);

        
        Vector3 original_position = server.transform.position;
        Quaternion original_rotation = server.transform.rotation;
        Vector3 destination = controller.cam.transform.position + (controller.cam.transform.forward * 1);
        StartCoroutine(rotateServer(server));

        // move to destination position until server is put back down
        while (!Input.GetKeyDown(KeyCode.Tab)) {
            destination += (controller.cam.transform.forward * Input.GetAxis("Mouse ScrollWheel"));
            server.transform.position = Vector3.Lerp(server.transform.position, destination, 6.26f * Time.deltaTime);
            yield return null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        controller.can_move = true;
        inspecting_server = false;

        // move server back to original position
        while (Vector3.Distance(server.transform.position, original_position) > 0.0001f) {
            server.transform.position = Vector3.Lerp(server.transform.position, original_position, 6.26f * Time.deltaTime);
            server.transform.rotation = Quaternion.Lerp(server.transform.rotation, original_rotation, 6.26f * Time.deltaTime);
            yield return null;
        }
        serverScript.plug.SetActive(false);
        serverScript.switch_object.SetActive(false);
        serverScript.cord.SetActive(false);
        serverScript.ip_text.gameObject.SetActive(false);
        serverScript.still = true;

    }
    IEnumerator rotateServer(GameObject server) {
        while (inspecting_server)
        {
            while (Input.GetKey(KeyCode.Mouse0))
            {
                server.transform.RotateAround(server.transform.position, controller.cam.transform.right, controller.ver_curs_inp * 3);
                server.transform.RotateAround(server.transform.position, controller.cam.transform.up, -controller.hor_curs_inp * 3);
                yield return null;
            }
            yield return null;
        }
        
    }
}
