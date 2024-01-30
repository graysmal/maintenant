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
    public float reach; //3

    // input
    float hor_inp;
    float ver_inp;
    public float hor_curs_inp;
    public float ver_curs_inp;

    // player states
    public bool can_move;
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
    Collider player_collider;
    Rigidbody rb;
    public GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        // lock cursor to center of screen
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        player_collider = gameObject.GetComponent<CapsuleCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        cam = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = frame_rate;
        updateXYInput();
        if (can_move)
        {
            calculateMovement();
        }
        else {
            rb.velocity = Vector3.zero;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(cam.transform.position, cam.transform.position + cam.transform.forward * 3);
    }

    void updateXYInput()
    {
        hor_inp = Input.GetAxisRaw("Horizontal");
        ver_inp = Input.GetAxisRaw("Vertical");
        hor_curs_inp = Input.GetAxisRaw("Mouse X") * mouse_sens;
        ver_curs_inp = Input.GetAxisRaw("Mouse Y") * mouse_sens;
    }

    void updatePlayerStates()
    {
        // if there is collision below player, grounded is true

        grounded = Physics.Raycast(transform.position, -Vector3.up, player_collider.bounds.extents.y + 0.1f);

        // if player is crouching, check headroom for being able to uncrouch
        if (crouching)
        {
            has_headroom = (!Physics.CheckSphere(
                        transform.position + new Vector3(0, player_collider.bounds.extents.y + 0.055f + default_player_scale.y / 2, 0),
                        default_player_scale.y / 2));
        }
        // if player isn't crouching, check if there is any headroom at all above player
        else
        {
            has_headroom = (!Physics.CheckCapsule(
                            transform.position + new Vector3(0, player_collider.bounds.extents.y + 0.055f, 0) + transform.forward * .35f,
                            transform.position + new Vector3(0, player_collider.bounds.extents.y + 0.055f, 0) - transform.forward * .35f,
                            0.05f) ||
                        !Physics.CheckCapsule(
                            transform.position + new Vector3(0, player_collider.bounds.extents.y + 0.055f, 0) + transform.right * .35f,
                            transform.position + new Vector3(0, player_collider.bounds.extents.y + 0.055f, 0) - transform.right * .35f,
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
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, sprinting_cam_fov, Time.deltaTime * 6.56f);
        }
        else
        {
            cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, default_cam_fov, Time.deltaTime * 6.56f);
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
            transform.localScale = Vector3.Lerp(transform.localScale, crouching_player_scale, Time.deltaTime * 6.56f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, crouching_cam_pos, Time.deltaTime * 6.56f);
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
            transform.localScale = Vector3.Lerp(transform.localScale, default_player_scale, Time.deltaTime * 6.56f);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, default_cam_pos, Time.deltaTime * 6.56f);
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
        rb.AddForce(vector * Time.deltaTime * 120, ForceMode.Force);


        if (Input.GetKeyDown(KeyCode.Space) && grounded && !crouching)
        {
            rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }

        gameObject.transform.Rotate(new Vector3(0, hor_curs_inp, 0), Space.World);
        updateCamera();

    }

    void updateCamera() {
        Vector3 cameraRotation = new Vector3(
                -ver_curs_inp,
                0,
                0
                );
        cameraRotation = cam.transform.localEulerAngles + cameraRotation;
        cameraRotation = new Vector3(
            Mathf.Clamp(toNegativeDegrees(cameraRotation.x), -90, 90),
            cameraRotation.y,
            cameraRotation.z
            );
        cam.transform.localEulerAngles = cameraRotation;
    }

    // reused self-made code from Junior year project Super Michael Ball
    // unity likes to use degrees in the range 0 to 360. this function will try and return a range of -180 to 180.
    // there is a flaw in unity however that makes the x axis, and ONLY the x axis not cooperate past 90 degrees. it is very weird.
    // if it MUST be fixed, then we can start storing the x axis rotation in this script rather than using requesting unitys.
    float toNegativeDegrees(float degree)
    {
        degree /= 360;
        if (degree > 0.5) { degree -= 1; }
        degree *= 360;
        return degree;
    }

    public GameObject lookingAt() // returns true if the player is close enough to and is looking at the given object
    {
        RaycastHit hit; // if i'm close, & a raycast from the player hit the the object to look at, return true
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit) && hit.distance < reach)
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    public bool isPlayerLooking(String tag) // returns true if the player is close enough to and is looking at the given object
    {
        RaycastHit hit; // if i'm close, & a raycast from the player hit the the object to look at, return true
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit) && hit.transform.tag == tag && hit.distance < reach)
        {
            return true;
        }
        return false;
    }
}