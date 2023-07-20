using System.Collections;
using System.Collections.Generic;
using EzySlice;
using UnityEngine;

public class SliceTest : MonoBehaviour
{
    public GameObject sourceGo;
    public GameObject slicerGO;
    public Material sectionMat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SlicedHull hull = sourceGo.Slice(slicerGO.transform.position, slicerGO.transform.up);
            GameObject upper = hull.CreateUpperHull(sourceGo, sectionMat);
            GameObject lower = hull.CreateLowerHull(sourceGo, sectionMat);
            sourceGo.SetActive(false);
        }
    }
}
