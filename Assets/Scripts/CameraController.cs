using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject focus_target;

    public float sensitivityX = 20f;
    public float sensitivityY = 20f;
    public float sensitivityZoom = 50f;
    public float max_zoom = 1000f;
    public float min_zoom = 100f;

    private float _radius = 40f;
    private Vector3 _relative_position = Vector3.down;

    void Update()
    {
        _radius -= BindingManager.GetAxisValue("Zoom") * sensitivityZoom;
        _radius = Mathf.Clamp(_radius, min_zoom, max_zoom);

        float deltaX = 0;
        float deltaY = 0;
        if (BindingManager.GetKeyState("RotCamHold"))
        {
            deltaX = BindingManager.GetAxisValue("RotCamX");
            deltaY = -BindingManager.GetAxisValue("RotCamY");
        }

        _relative_position = Quaternion.Euler(Vector3.Cross(_relative_position, Vector3.forward) * deltaY * sensitivityY) * _relative_position;
        _relative_position = Quaternion.Euler(Vector3.forward * deltaX * sensitivityX) * _relative_position;
        
        transform.position = focus_target.transform.TransformPoint(_relative_position.normalized * _radius);
        transform.LookAt(focus_target.transform.position, focus_target.transform.forward);
    }
}
