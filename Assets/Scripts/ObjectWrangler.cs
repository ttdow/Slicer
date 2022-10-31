using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWrangler : MonoBehaviour
{
    public Vector3 initialPos;

    public Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;

        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -2.0f)
        {
            transform.position = initialPos;

            body.velocity = Vector3.zero;
        }
    }
}
