using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKCamera : LKEngine.Component
{
    Camera _camera;
    public UnityEngine.Camera Camera { get => _camera; }

    public void SwitchCamera(UnityEngine.Camera cam)
    {
        _camera = cam;
    }

    public void ChangeComponent(Component comp)
    {
        for(int i = 0; i < children.Count; i++)
        {
            children[i].Destroy();
        }
    }

}
