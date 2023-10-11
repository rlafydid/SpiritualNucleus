using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendTree : MonoBehaviour
{
    public float val;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Animator>().SetFloat("Velocity X", val);

    }
}
