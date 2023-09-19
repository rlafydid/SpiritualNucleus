using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParabolic : MonoBehaviour
{
    private float t;
    private float g = 10f;

    public bool reset = false;

    private Vector3 startPos;

    private Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
            
        this.transform.position +=  velocity * deltaTime;

        velocity.y -= g * deltaTime;
        
        if (reset)
        {
            this.transform.position = startPos;
            t = 0;
            reset = false;
        }
    }
}
