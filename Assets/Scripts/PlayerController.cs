using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public int frame_rate;
    public float mouse_sens;
    public enum crouchModes {TOGGLE, HOLD};
    public crouchModes crouch_mode = crouchModes.TOGGLE;

    public float movement_speed; // 800
    public float default_drag; // 3
    public float jump_force; // 405

    // factors influencing movement_speed
    public float sprint_modifier; // 1.4
    public float jump_modifier; // 0.07
    public float crouch_modifier; //0.5

    public float stamina; // 100

    // input
    float hor_inp;
    float ver_inp;
    float hor_curs_inp;
    float ver_curs_inp;

    // player states
    public bool grounded;
    public bool has_headroom;
    public bool moving;
    public bool sprinting;
    public bool crouch_toggle;
    public bool crouching;

    // other
    Vector3 default_player_scale = Vector3.one;
    Vector3 default_cam_pos = new Vector3(0, 0.6f, 0);
    Vector3 crouching_player_scale = new Vector3(1, 0.5f, 1);
    Vector3 crouching_cam_pos = new Vector3(0, 0.2f, 0);
    float default_cam_fov = 60;
    float sprinting_cam_fov = 65;

    public Vector3 velocity;
    Collider collider;
    Rigidbody rb;
    GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        // lock cursor to center of screen
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        collider = gameObject.GetComponent<CapsuleCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        cam = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = frame_rate;
        updateXYInput();
        calculateMovement();

    }

    void updateXYInput()
    {
        hor_inp = Input.GetAxisRaw("Horizontal");
        ver_inp = Input.GetAxisRaw("Vertical");
        hor_curs_inp = Input.GetAxisRaw("Mouse X");
        ver_curs_inp = Input.GetAxisRaw("Mouse Y");
    }

    void updatePlayerStates()
    {
        // if there is collision below player, grounded is true

        // old capsule-based grounded collision check
       /* grounded = (Physics.CheckCapsule(
                        transform.position - new Vector3(0, collider.bounds.extents.y + 0.055f, 0) + transform.forward * .35f,
                        transform.position - new Vector3(0, collider.bounds.extents.y + 0.055f, 0) - transform.forward * .35f,
                        0.05f) ||
                    Physics.CheckCapsule(
                        transform.position - new Vector3(0, collider.bounds.extents.y + 0.055f, 0) + transform.right * .35f,
                        transform.position - new Vector3(0, collider.bounds.extents.y + 0.055f, 0) - transform.right * .35f,
                        0.05f));*/

        grounded = Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + 0.1f);

        // if player is crouching, check headroom for being able to uncrouch
        if (crouching)
        {
            has_headroom = (!Physics.CheckSphere(
                        transform.position + new Vector3(0, collider.bounds.extents.y + 0.055f + default_player_scale.y / 2, 0),
                        default_player_scale.y / 2));
        }
        // if player isn't crouching, check if there is any headroom at all above player
        else
        {
            has_headroom = (!Physics.CheckCapsule(
                            transform.position + new Vector3(0, collider.bounds.extents.y + 0.055f, 0) + transform.forward * .35f,
                            transform.position + new Vector3(0, collider.bounds.extents.y + 0.055f, 0) - transform.forward * .35f,
                            0.05f) ||
                        !Physics.CheckCapsule(
                            transform.position + new Vector3(0, collider.bounds.extents.y + 0.055f, 0) + transform.right * .35f,
                            transform.position + new Vector3(0, collider.bounds.extents.y + 0.055f, 0) - transform.right * .35f,
                            0.05f));
        }

        // if velocity is any magnitude greater than 0, moving is true
        if (rb.velocity.magnitude > 0.1f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        // if left shift is held and player isn't crouching, sprinting is true
        sprinting = (Input.GetKey(KeyCode.LeftShift) && (!crouching));

        // if sprinting is true and player is moving, change camera fov
        if (sprinting && moving)
        {
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, sprinting_cam_fov, 0.05f);
        }
        else
        {
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, default_cam_fov, 0.05f);
        }

        // set crouch toggle

        if (crouch_mode == crouchModes.TOGGLE) {
            if (Input.GetKeyDown(KeyCode.LeftControl) && !crouch_toggle)
            {
                crouch_toggle = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) && crouch_toggle)
            {
                crouch_toggle = false;
            }
        }
        else if (crouch_mode == crouchModes.HOLD) {
            crouch_toggle = Input.GetKey(KeyCode.LeftControl);
        }

        // if left shift is just now pressed in order to sprint, disable crouch
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            crouch_toggle = false;
        }

        // crouch
        if (crouch_toggle && !crouching)
        {
            crouching = true;
            sprinting = false;
            StartCoroutine(crouch());
        }
        // if crouch toggle is off, uncrouch when has_headroom is true
        else if (!crouch_toggle && crouching && has_headroom)
        {
            crouching = false;
        }





    }

    IEnumerator crouch()
    {
        crouching = true;
        // "source engine"-type jump
        if (!grounded)
        {
            transform.position += Vector3.up * 0.2f;
        }
        // crouching
        while (crouching)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, crouching_player_scale, 0.05f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, crouching_cam_pos, 0.05f);
            yield return null;
        }

        // uncrouching
        while (Vector3.Distance(transform.localScale, default_player_scale) > 0.05f || Vector3.Distance(cam.transform.localPosition, default_cam_pos) > 0.05f)
        {
            // end animation if crouching again
            if (crouching)
            {
                yield break;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, default_player_scale, 0.05f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, default_cam_pos, 0.05f);
            yield return null;
        }

        yield break;
    }

    void calculateMovement()
    {
        updatePlayerStates();


        // create flat movement vector with a magnitude of one based on player input
        Vector3 vector;
        vector = gameObject.transform.forward * ver_inp + gameObject.transform.right * hor_inp;
        vector.Normalize();

        // scale movement speed with according factors
        vector *= (movement_speed);

        if (sprinting)
        {
            vector *= sprint_modifier;
        }

        if (crouching)
        {

            vector *= crouch_modifier;
        }

        if (grounded)
        {
            rb.drag = default_drag;
        }
        else
        {
            rb.drag = 0;
            vector *= jump_modifier;
        }

        // DBUG display speed 
        //Debug.Log("V: " + rb.velocity + " || " + rb.velocity.magnitude);
        rb.AddForce(vector, ForceMode.Force);


        if (Input.GetKeyDown(KeyCode.Space) && grounded && !crouching)
        {
            rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }

        gameObject.transform.Rotate(new Vector3(0, hor_curs_inp * mouse_sens, 0), Space.World);
        cam.transform.Rotate(new Vector3(-ver_curs_inp * mouse_sens, 0, 0), Space.Self);
    }

}