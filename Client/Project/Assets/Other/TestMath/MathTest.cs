using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    public Transform p1Trans;
    public Transform p2Trans;

    public Transform p3Trans;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Haha"))
        {
            Line();
        }
    }

    void Line()
    {
        Vector3 p1 = p1Trans.position.To2D();
        Vector3 p2 = p2Trans.position.To2D();

        Vector3 p = p3Trans.position.To2D();

        Vector3 directory = p2 - p1;

        float k = (p2.y - p1.y) / (p2.x - p1.x);
        float t = directory.y - k * directory.x;

        float a = k;
        float b = -1;
        float c = t;

        float distance = Mathf.Abs(a * p.x + b * p.y + c) / (Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2)));

        Debug.Log(distance);
    }
}
