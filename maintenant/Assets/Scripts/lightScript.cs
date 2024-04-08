using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static Unity.VisualScripting.Member;

public class lightScript : MonoBehaviour
{
    public static List<lightScript> lights = new List<lightScript>();
    public GameManager gameManager;
    AudioSource a_source;
    GameObject bulb;
    Coroutine current;

    Animator l_s_switch;

    bool light_turning_on;
    bool _is_enabled = false;
    public bool is_enabled {
        get { return _is_enabled; }
        set {
            if (_is_enabled == false && value == true)
            {
                current = StartCoroutine(turnLightOn());
                l_s_switch.Play("switch_on");
                AudioSource.PlayClipAtPoint(gameManager.power_down, l_s_switch.transform.position);
            }
            else if (_is_enabled == true && value == false) {
                if (light_turning_on)
                {
                    light_turning_on = false;
                    StopCoroutine(current);
                }
                bulb.SetActive(false);
                a_source.Stop();
                l_s_switch.Play("switch_off");
                AudioSource.PlayClipAtPoint(gameManager.power_up, l_s_switch.transform.position);
                //StartCoroutine(turnLightOff());
            }
            _is_enabled = value;
        }
    }

    bool isTurningOff = false;

    public AudioClip hum_on_clip;
    public AudioClip hum_loop_clip;
    // Start is called before the first frame update
    void Start()
    {
        lights.Add(this.transform.GetComponent<lightScript>());
        a_source = GetComponent<AudioSource>();
        bulb = this.transform.GetChild(1).gameObject;
        l_s_switch = gameManager.light_switch.GetComponent<Animator>();
        is_enabled = true;
        StartCoroutine(flickerRandomly());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTurningOff && (GameManager.time - 300) > -1 && (GameManager.time - 300) < 1) {
            is_enabled = false;
            isTurningOff = true;
            StartCoroutine(turnOffPeriodically());
            
        }
    }

    public static void publicToggle() {
        foreach (lightScript l in lights) {
            l.is_enabled = !l.is_enabled;
        }
    }

    IEnumerator turnOffPeriodically() {
        float time_target = GameManager.time + Random.Range(10, 30);
        while (isTurningOff)
        {
            while (is_enabled)
            {
                while (GameManager.time < time_target)
                {
                    yield return null;
                }
                foreach (lightScript obj in lights) { 
                    obj.is_enabled = false;
                }
                time_target = GameManager.time + Random.Range(10, 30);
                yield return null;
            }
            time_target = GameManager.time + Random.Range(10, 30);
            yield return null;
        }
    }
    IEnumerator flickerRandomly() {
        
        float time_target = GameManager.time + Random.Range(1, 15);
        GameObject spot_light = this.transform.GetChild(1).GetChild(1).gameObject;
        float original_intensity = spot_light.GetComponent<Light>().intensity;
        while (true) {
            while (is_enabled)
            {
                while (GameManager.time < time_target)
                {
                    yield return null;
                }

                spot_light.GetComponent<Light>().intensity = original_intensity / (2 + Random.Range(-1, 1));
                yield return new WaitForSeconds(0.1f);
                spot_light.GetComponent<Light>().intensity = original_intensity + Random.Range(-2, 2);
                time_target = GameManager.time + Random.Range(1, 15);
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator turnLightOn() { 
        a_source.clip = hum_on_clip;
        a_source.loop = false;
        a_source.Play();
        light_turning_on = true;
        float x = 0;
        while (a_source.isPlaying)
        {
            if (!light_turning_on) {
                yield break;
            }
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
        light_turning_on = false;
    }
}
