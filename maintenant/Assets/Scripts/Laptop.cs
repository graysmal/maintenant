using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using static Server;

public class Laptop : MonoBehaviour
{
    public GameManager gameManager;
    public Transform cam_destination;

    public AudioClip[] keys;
    public AudioClip key_enter;

    public AudioClip open;
    public AudioClip close;

    public AudioSource a_source;
    public TextMeshPro laptop_text_obj;

    bool blinking = false;

    Color default_color = new Color(145, 255, 131);

    string laptop_past_content = "";
    string laptop_current_line = "> ";

    string laptop_current_input = "";

    // Start is called before the first frame update
    void Start()
    {
        cam_destination = transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1);
        a_source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator listenLaptopInput() {
        a_source.PlayOneShot(open);
        laptop_text_obj.text = laptop_past_content + laptop_current_line;
        StartCoroutine(blinkText(default_color));
        yield return new WaitForSeconds(0.2f);
        while (true) {
            foreach (Char character in Input.inputString)
            {
                if (character == '\b')
                {
                    a_source.PlayOneShot(keys.ElementAt(UnityEngine.Random.Range(0, keys.Length)));
                    if (laptop_current_input.Length != 0)
                    {
                        laptop_current_input = laptop_current_input.Substring(0, laptop_current_input.Length - 1);
                    }
                    
                }
                else if (character == '\n' || character == '\r')
                {
                    a_source.PlayOneShot(key_enter);
                    if (laptop_current_input.Equals(""))
                    {
                        StartCoroutine(blinkText(Color.red));
                    }
                    else
                    {

                        string status_code = "";
                        // find out if server needed its code entered.
                        foreach (GameObject server_obj in gameManager.ongoing_event_servers)
                        {
                            Server server_script = server_obj.GetComponent<Server>();
                            if (laptop_current_input.Equals(server_script.ip))
                            {
                                status_code = "server " + laptop_current_input + "'s data has been reset.";
                                if (server_script.current_event == ServerEvent.RED)
                                {
                                    server_script.event_ongoing = false;
                                    server_script.status_light.color = Color.green;
                                    server_script.status_light.gameObject.SetActive(true);
                                }
                                break;
                            }
                        }
                        if (status_code.Equals(""))
                        {
                            status_code = "server " + laptop_current_input + " does not need to be reset.";
                            StartCoroutine(blinkText(Color.red));
                        }

                        laptop_past_content += laptop_current_input + "\n" + status_code + "\n";
                        int count_of_lines = laptop_past_content.Split("\n").Length - 1;
                        if (count_of_lines > 7)
                        {
                            laptop_past_content = laptop_past_content.Substring(laptop_past_content.IndexOf("\n") + 1, laptop_past_content.Length - (laptop_past_content.IndexOf("\n") + 1));
                            laptop_past_content = laptop_past_content.Substring(laptop_past_content.IndexOf("\n") + 1, laptop_past_content.Length - (laptop_past_content.IndexOf("\n") + 1));
                        }
                        laptop_current_input = "";
                    }
                }
                else { 
                    if (laptop_current_input.Length < 15)
                    {
                        a_source.PlayOneShot(keys.ElementAt(UnityEngine.Random.Range(0, keys.Length)));
                        laptop_current_input += character;
                    }
                    else {
                        StartCoroutine(blinkText(Color.red));
                    }
                }
                laptop_current_line = "> " + laptop_current_input;
                laptop_text_obj.text = laptop_past_content + laptop_current_line;

            }
            
            yield return null;
        }
    }

    public IEnumerator blinkText(Color color) {
        if (blinking)
        {
            yield break;
        }
        else {
            blinking = true;
            float lasting_for = 1;
            float time = 0;
            Color original_color = laptop_text_obj.color;
            laptop_text_obj.color = color;
            laptop_text_obj.gameObject.SetActive(false);
            while (time < lasting_for)
            {
                yield return new WaitForSeconds(0.2f);
                time += 0.2f;
                laptop_text_obj.gameObject.SetActive(!laptop_text_obj.gameObject.activeSelf);
            }
            laptop_text_obj.color = original_color;
            blinking = false;
            laptop_text_obj.gameObject.SetActive(true);
            yield break;
        }
    }
}
