using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class light : MonoBehaviour
{
    public GameManager gameManager;
    AudioSource a_source;
    GameObject bulb;

    bool _is_enabled = true;
    public bool is_enabled {
        get { return _is_enabled; }
        set {
            if (_is_enabled == false && value == true)
            {
                StartCoroutine(turnLightOn());
            }
            else if (_is_enabled == true && value == false) {
                bulb.SetActive(false);
                a_source.Stop();
                //StartCoroutine(turnLightOff());
            }
            _is_enabled = value;
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
        StartCoroutine(flickerRandomly());
    }

    // Update is called once per frame
    void Update()
    {
        if ((gameManager.time - 300) > -1 && (gameManager.time - 300) < 1) {
            is_enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            Debug.Log(!is_enabled);
            is_enabled = (!is_enabled);
        }
    }

    IEnumerator flickerRandomly() {
        
        float time_target = gameManager.time + Random.Range(1, 15);
        GameObject spot_light = this.transform.GetChild(1).GetChild(1).gameObject;
        float original_intensity = spot_light.GetComponent<Light>().intensity;
        while (true) {
            while (is_enabled)
            {
                while (gameManager.time < time_target)
                {
                    yield return null;
                }

                spot_light.GetComponent<Light>().intensity = original_intensity / (2 + Random.Range(-1, 1));
                yield return new WaitForSeconds(0.1f);
                spot_light.GetComponent<Light>().intensity = original_intensity + Random.Range(-2, 2);
                time_target = gameManager.time + Random.Range(1, 15);
                yield return null;
            }
            yield return null;
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
