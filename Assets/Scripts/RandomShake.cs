using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomShake : MonoBehaviour
{
    Vector3 orgPosition;
    public float power = 0.001f;
    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        orgPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + (Mathf.Sin(t * 0.5f) * power),
            transform.position.z
        );

        if((UnityEngine.Random.Range(0, 10) == 0)) {
            t += 0.1f;
        }
    }
}
