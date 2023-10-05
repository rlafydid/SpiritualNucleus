using System.Collections;
using System.Collections.Generic;
using Battle;
using LKEngine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMoveController : LKEngine.Component,DefaultInputActions.ICameraActions
{
    public Entity target;
    Camera camera;

    public Vector3 DefaultAngle { get; set; } = new Vector3(0, 60, 0);
    public float DefaultDistance { get; set; } = 10;

    Vector3 offset;

    float speed = 1f;
    private float _dragSpeed = 0.8f;

    protected override void OnStart()
    {
        base.OnStart();
    }

    public void TraceTarget(Entity target)
    {
        this.target = target;
        camera = Camera.main;
        
        // offset = camera.transform.position - this.target.Position;
        offset = Quaternion.Euler(DefaultAngle) * -this.target.Forward * DefaultDistance;
        
        HandleInputComponent.playerInput.Camera.SetCallbacks(this);
        StartDrag();
    }

    private bool _isDrag = false;
    private Vector3 _startDragPoint;
    private Vector3 _cameraStartOffset;
    
    
    protected override void OnUpdate()
    {
        if(target == null)
            return;

        Vector3 newpos = target.Position + target.LocalRotation * offset;
        Vector3 forward = (newpos - camera.transform.position).normalized;
        var lookat = target.LocalRotation;
        float t = Time.deltaTime * speed;

        camera.transform.position = Vector3.Lerp(camera.transform.position, target.Position + offset, t * 10);
        // camera.transform.LookAt(target.Position);

        Vector3 lookAtDir = (target.Position + Vector3.up * 2) - camera.transform.position ;
        
        camera.transform.rotation =
            Quaternion.Lerp(camera.transform.rotation, Quaternion.LookRotation(lookAtDir.normalized), t * 10);
        
        //camera.transform.position = Vector3.Lerp(camera.transform.position, target.Position + target.LocalRotation * offset, t);
        //camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, Quaternion.Euler(45, lookat.eulerAngles.y, lookat.eulerAngles.z), t);
        if(_isDrag)
            DragCamera();
    }

    private Vector3 _lastOffset;
    public void OnPress(InputAction.CallbackContext context)
    {
        Debug.Log($"OnPress {context.started} {context.performed} {context.phase} {context.canceled}");

        switch (context.phase)
        {
            case InputActionPhase.Started:
                // StartDrag();
                break;
            case InputActionPhase.Canceled:
                // CanceledDrag();
                break;
        }
    }

    void StartDrag()
    {
        _lastMousePos = Mouse.current.position.ReadValue();
        _cameraStartOffset = camera.transform.position;
        _isDrag = true;
        _lastOffset = offset;
        Cursor.visible = false;
    }

    void CanceledDrag()
    {
        _isDrag = false;
        Cursor.visible = true;
    }

    public void OnCallOutMouse(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                CanceledDrag();
                break;
            case InputActionPhase.Canceled:
                StartDrag();
                // CanceledDrag();
                break;
        }
    }

    
    private Vector3 _lastMousePos;
    void DragCamera()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        var dragOffset = mousePos - _lastMousePos;
        if (dragOffset.magnitude < 0.01f)
            return;
        float x = dragOffset.x / Screen.width;
        float y = dragOffset.y / Screen.height;

        Vector3 newPoint = Quaternion.AngleAxis(x * 360 * _dragSpeed, Vector3.up) * offset;
        
        float yAngleOffset = y * 360 * _dragSpeed;
        float angle = Vector3.SignedAngle(newPoint, Vector3.up, Vector3.Cross(newPoint, Vector3.up));
        Debug.Log($"angle{angle} offset{yAngleOffset}");
        angle = Mathf.Abs(angle);
        if (angle + yAngleOffset > 150)
            yAngleOffset = Mathf.Abs(angle) - 150;

        if (angle + yAngleOffset < 30)
            yAngleOffset = 30 - angle; 
        
        newPoint = Quaternion.AngleAxis(yAngleOffset, Vector3.Cross(Vector3.up, newPoint)) * newPoint;
        
                
        offset = newPoint;
        _lastMousePos = mousePos;
    }
}
