using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PlayerInteractions : MonoBehaviour
{
    public GameManager gameManager;
    PlayerController controller;

    AudioSource a_source;
    bool looking_at_server;
    bool inspecting_server;
    bool on_laptop;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        a_source = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update() { 
    
        if (controller.isPlayerLooking("server"))
        {
            looking_at_server = true;
        }
        else { looking_at_server = false; }

        if (!inspecting_server && looking_at_server && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(inspectServer());
        }

        if (controller.isPlayerLooking("laptop") && Input.GetKeyDown(KeyCode.E) && !on_laptop && !controller.crouching) { 
            StartCoroutine(interactWithLaptop());
        }

        if (controller.isPlayerLooking("lightswitch") && Input.GetKeyDown(KeyCode.E)) {
            lightScript.publicToggle();
        }
    }

    IEnumerator inspectServer() {
        GameObject server = controller.lookingAt();
        Server serverScript = server.GetComponent<Server>();

        // if server isn't ready to be interacted with again, cancel coroutine
        if (serverScript.still == false || serverScript.permanently_disabled == true)
        {
            yield break;
        }
        // if server is ready, make not ready for interaction again
        else {
            server.GetComponent<Server>().still = false;
        }

        Cursor.lockState = CursorLockMode.None;
        controller.can_move = false;
        a_source.PlayOneShot(gameManager.pick_up_sfx);
        inspecting_server = true;

        serverScript.plug.SetActive(true);
        serverScript.switch_object.SetActive(true);
        serverScript.cord.SetActive(true);
        serverScript.ip_text.gameObject.SetActive(true);

        
        Vector3 original_position = server.transform.parent.position;
        Quaternion original_rotation = server.transform.rotation;
        Vector3 destination = controller.cam.transform.position + (controller.cam.transform.forward * 1);
        StartCoroutine(rotateServer(server));

        // move to destination position until server is put back down
        while (!Input.GetKeyDown(KeyCode.Tab) && !serverScript.permanently_disabled) {
            destination += (controller.cam.transform.forward * Input.GetAxis("Mouse ScrollWheel"));
            server.transform.position = Vector3.Lerp(server.transform.position, destination, 6.26f * Time.deltaTime);
            yield return null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        controller.can_move = true;
        a_source.PlayOneShot(gameManager.put_down_sfx);
        inspecting_server = false;


        // move server back to original position
        while (Vector3.Distance(server.transform.position, server.transform.parent.position) > 0.0001f) {
            server.transform.position = Vector3.Lerp(server.transform.position, server.transform.parent.position, 12.26f * Time.deltaTime);
            server.transform.rotation = Quaternion.Lerp(server.transform.rotation, original_rotation, 12.26f * Time.deltaTime);
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

    IEnumerator interactWithLaptop() {
        GameObject laptop = controller.lookingAt();
        Laptop laptop_script = laptop.transform.parent.GetComponent<Laptop>();

        on_laptop = true;
        Cursor.lockState = CursorLockMode.None;
        controller.can_move = false;
        laptop.transform.parent.GetComponent<Animator>().Play("laptop_open");
        controller.transform.position = new Vector3(8.17f, 1, -3.45f);
        controller.transform.localEulerAngles = new Vector3(0, 100, 0);
        Coroutine laptop_listen = StartCoroutine(laptop_script.listenLaptopInput());
        while (!Input.GetKeyDown(KeyCode.Tab)) {
            controller.cam.transform.position = Vector3.Lerp(controller.cam.transform.position, laptop_script.cam_destination.position, Time.deltaTime * 6.56f);
            controller.cam.transform.rotation = Quaternion.Lerp(controller.cam.transform.rotation, laptop_script.cam_destination.rotation, Time.deltaTime * 6.56f);
            yield return null;
        }

        
        Cursor.lockState = CursorLockMode.Locked;
        controller.can_move = true;
        laptop.transform.parent.GetComponent<Animator>().Play("laptop_close");
        laptop_script.a_source.PlayOneShot(laptop_script.close);
        StopCoroutine(laptop_listen);
        Vector3 orig_cam_position = controller.transform.position + new Vector3(0, 0.6f, 0);
        Vector3 orig_cam_rotation = new Vector3(controller.cam.transform.localEulerAngles.x, 0, 0);
        while ((Vector3.Distance(controller.cam.transform.position, orig_cam_position) > 0.0001f) || Vector3.Distance(controller.cam.transform.localEulerAngles, orig_cam_rotation) > 0.0001f) {
            orig_cam_position = controller.transform.position + new Vector3(0, 0.6f, 0);
            orig_cam_rotation = new Vector3(controller.cam.transform.localEulerAngles.x, 0, 0);
            controller.cam.transform.position = Vector3.Lerp(controller.cam.transform.position, orig_cam_position, Time.deltaTime * 12.56f);
            controller.cam.transform.localEulerAngles = Vector3.Lerp(controller.cam.transform.localEulerAngles, orig_cam_rotation, Time.deltaTime * 12.56f);
            yield return null;
        }
        on_laptop = false;
        yield return null;
    }
}
