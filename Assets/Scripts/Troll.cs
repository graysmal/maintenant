using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll : MonoBehaviour
{
    public Vector3 starting_position;
    public Vector3 destination_position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator moveTroll() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;

        while (Vector3.Distance(transform.position, destination_position) > 0.05f)
        {
            Debug.Log(Vector3.Distance(transform.position, destination_position));
            transform.position = Vector3.Lerp(transform.position, destination_position, 0.56f * Time.deltaTime);
            yield return null;
        }
        yield break;
    }

    public void hideTroll() { 
        StopAllCoroutines();
        transform.position = starting_position;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }
}
