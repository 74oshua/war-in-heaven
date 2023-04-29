using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public GameObject origin_object;
    public float maxOriginDistance = 1000f;

    private static GameManager _manager = null;
    public static GameManager manager
    {
        get { return _manager; }
    }

    void Awake()
    {
        _manager = this;
    }

    void Start()
    {
        // spacecraft
        BindingManager.AddBinding("RotUp", KeyCode.S);
        BindingManager.AddBinding("RotDown", KeyCode.W);
        BindingManager.AddBinding("RotLeft", KeyCode.D);
        BindingManager.AddBinding("RotRight", KeyCode.A);
        BindingManager.AddBinding("RollLeft", KeyCode.Q);
        BindingManager.AddBinding("RollRight", KeyCode.E);
        BindingManager.AddBinding("Burn", KeyCode.Space);
        BindingManager.AddBinding("ToggleStable", KeyCode.T);
        
        // camera
        BindingManager.AddBinding("RotCamHold", KeyCode.Mouse1);
        BindingManager.AddAxis("RotCamX", "Mouse X");
        BindingManager.AddAxis("RotCamY", "Mouse Y");
        BindingManager.AddAxis("Zoom", "Mouse ScrollWheel");
        BindingManager.AddBinding("ChangeView", KeyCode.V);
    }

    void FixedUpdate()
    {
        // run gravity sim
        // GravityController.attractBodies(Time.fixedDeltaTime);
        // GravityController.updateState();
    }

    void LateUpdate()
    {
        if (origin_object != null && origin_object.transform.position.magnitude > maxOriginDistance)
        {
            shiftOrigin(origin_object.transform.position);
        }
    }

    private void shiftOrigin(Vector3 offset)
    {
        OriginShiftController.shiftAll(offset);
    }
}
