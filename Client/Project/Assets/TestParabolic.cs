using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParabolic : MonoBehaviour
{
    private float t;
    private float g = 0.9f;

    public bool reset = false;

    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
            
        float yV = -g * t * 0.5f;
        float v = yV;
        this.transform.position += Vector3.up * v;

        if (reset)
        {
            this.transform.position = startPos;
            t = 0;
            reset = false;
        }
    }
}
