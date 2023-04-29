using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spacecraft))]
public class ManualSpacecraftController : MonoBehaviour
{
    private Spacecraft _sc;
    private bool stablize = false;

    public float sensitivity = 10f;

    void Start()
    {
        _sc = GetComponent<Spacecraft>();
    }

    void Update()
    {
        Vector3 target_heading = _sc.transform.up;
        Vector3 target_up = _sc.transform.forward;
        if (BindingManager.GetKeyStateDown("ToggleStable"))
        {
            stablize = !stablize;
        }
        if (BindingManager.GetKeyState("RotUp"))
        {
            target_heading += _sc.transform.forward;
        }
        if (BindingManager.GetKeyState("RotDown"))
        {
            target_heading += -_sc.transform.forward;
        }
        if (BindingManager.GetKeyState("RotLeft"))
        {
            target_heading += -_sc.transform.right;
        }
        if (BindingManager.GetKeyState("RotRight"))
        {
            target_heading += _sc.transform.right;
        }
        if (BindingManager.GetKeyState("RollLeft"))
        {
            target_up += _sc.transform.right;
        }
        if (BindingManager.GetKeyState("RollRight"))
        {
            target_up += -_sc.transform.right;
        }
        if (BindingManager.GetKeyState("Burn"))
        {
            _sc.main_throttle = 1;
        }
        else
        {
            _sc.main_throttle = 0;
        }
        _sc.setHeading(target_heading, target_up, stablize);
    }
}
