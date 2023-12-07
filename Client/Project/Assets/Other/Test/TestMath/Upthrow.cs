using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upthrow : MonoBehaviour
{
    public float f;
    public float angle;
    public Transform center;
    float v;
    float t = 0;
    float g = 10;
    float v0;
    bool update = false;

    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    Vector3 pos;
    float lastH = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float radian = angle * Mathf.Deg2Rad;
            v0 = Mathf.Sin(radian) * f;
            v = Mathf.Cos(radian) * f;
            t = 0;
            transform.position = new Vector3(0, 0, 0);
            lastH = 0;
            update = true;
            dir = (transform.position - center.position).normalized;
        }
        if (!update)
            return;

        t += Time.deltaTime;
        float h = v0 * t - 0.5f * g * t * t;
        float deltaH = h - lastH;
        lastH = h;
        transform.position += Vector3.up * deltaH + dir * v * Time.deltaTime;

        if (h < 0)
            update = false;
    }

}
