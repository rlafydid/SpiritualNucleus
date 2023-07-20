using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public float MoveRange = 20f;
    // Start is called before the first frame update

    public float Speed = 2;

    bool isForwad = false;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        isForwad = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isForwad)
        {
            transform.position += Vector3.forward * Time.deltaTime * Speed;

            if(Vector3.Distance(transform.position, startPos) > MoveRange/2)
            {
                isForwad = false;
            }
        }
        else
        {
            transform.position += Vector3.back * Time.deltaTime * Speed;

            if (Vector3.Distance(transform.position, startPos) > MoveRange / 2)
            {
                isForwad = true;
            }
        }
    }
}
