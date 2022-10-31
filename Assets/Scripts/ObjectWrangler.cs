using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Small script to prevent objects from falling through the world
public class ObjectWrangler : MonoBehaviour
{
    public Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -2.0f)
        {
            transform.position = new Vector3(Random.Range(-9.0f, 9.0f), 2.0f, Random.Range(-9.0f, 9.0f));

            body.velocity = Vector3.zero;
        }
    }
}
