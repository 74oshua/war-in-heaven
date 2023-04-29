using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Thruster))]
public class ThrusterPlume : MonoBehaviour
{
    public GameObject plumeObject;

    private Thruster _thruster;
    private MeshRenderer _plumeRenderer;

    void Start()
    {
        _thruster = GetComponent<Thruster>();
        _plumeRenderer = plumeObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        _plumeRenderer.material.SetFloat("_Throttle", (_thruster.fuel_used / Time.fixedDeltaTime) / _thruster.mass_flow_rate);
    }
}
