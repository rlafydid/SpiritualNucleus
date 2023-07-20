using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public GameObject PathTarget;
    public GameObject Target;
    public float AttackRange = 1;
    public float Speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = PathTarget.transform.position;

        if(Vector3.Distance(transform.position, Target.transform.position) <= AttackRange)
        {
            return;
        }

        transform.position += (targetPos - transform.position).normalized * Time.deltaTime * Speed;
    }

    private void OnDrawGizmos()
    {
        Vector3 targetPos = Target.transform.position;
        Vector3 pathEndPos = PathTarget.transform.position;
        Gizmos.DrawWireSphere(targetPos, AttackRange);
        Gizmos.DrawLine(pathEndPos, transform.position);

        Vector3 dir1 = transform.position - targetPos;
        Vector3 dir2 = pathEndPos - targetPos;

        Debug.Log(Vector3.Angle(dir1, dir2));
    }
}
