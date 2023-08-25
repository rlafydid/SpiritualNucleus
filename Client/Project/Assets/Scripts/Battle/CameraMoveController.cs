using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

public class CameraMoveController : LKEngine.Component
{
    public Entity target;
    Camera camera;

    Vector3 offset;

    float speed = 0.5f;

    public void TraceTarget(Entity target)
    {
        this.target = target;
        camera = Camera.main;
        offset = camera.transform.position - this.target.Position;
    }

    protected override void OnUpdate()
    {
        if(target == null)
            return;

        Vector3 newpos = target.Position + target.LocalRotation * offset;
        Vector3 forward = (newpos - camera.transform.position).normalized;
        var lookat = target.LocalRotation;
        float t = Time.deltaTime * speed;

        camera.transform.position = Vector3.Lerp(camera.transform.position, target.Position + offset, t);

        //camera.transform.position = Vector3.Lerp(camera.transform.position, target.Position + target.LocalRotation * offset, t);
        //camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, Quaternion.Euler(45, lookat.eulerAngles.y, lookat.eulerAngles.z), t);
    }


}
