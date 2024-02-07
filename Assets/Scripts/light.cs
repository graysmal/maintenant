using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class light : MonoBehaviour
{
    AudioSource a_source;
    GameObject bulb;

    bool _enabled = true;
    public bool enabled {
        get { return _enabled; }
        set {
            if (_enabled == false && value == true)
            {
                StartCoroutine(turnLightOn());
            }
            else if (_enabled == true && value == false) {
                bulb.SetActive(false);
                a_source.Stop();
                //StartCoroutine(turnLightOff());
            }
            _enabled = value;
        }
    }

    public AudioClip hum_on_clip;
    public AudioClip hum_loop_clip;
    // Start is called before the first frame update
    void Start()
    {
        a_source = GetComponent<AudioSource>();
        bulb = this.transform.GetChild(1).gameObject;
        StartCoroutine(turnLightOn());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            Debug.Log(!enabled);
            enabled = (!enabled);
        }
    }

    IEnumerator turnLightOn() { 
        a_source.clip = hum_on_clip;
        a_source.loop = false;
        a_source.Play();
        float x = 0;
        while (a_source.isPlaying)
        {
            if (x < .2f)
            {
                x += 0.05f;
                bulb.SetActive(!bulb.activeSelf);
                yield return new WaitForSeconds(0.08f + x);
            }
            else {
                bulb.SetActive(true);
            }
            
            yield return null;
        }
        bulb.SetActive(true);
        a_source.clip = hum_loop_clip;
        a_source.loop = true;
        a_source.Play();
    }
}
