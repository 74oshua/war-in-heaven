using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityController))]
public class Spacecraft : MonoBehaviour
{
    public SpacecraftPart root_part;
    private Rigidbody _rb;
    public float main_throttle = 0.0f;

    private Vector3 _target_heading = Vector3.up;
    public Vector3 target_heading
    {
        get { return _target_heading; }
    }
    private Vector3 _target_up = Vector3.forward;
    public Vector3 target_up
    {
        get { return _target_up; }
    }
    private bool _lock_heading = false;
    public bool lock_heading
    {
        get { return _lock_heading; }
    }
    private List<FuelTank> _fuel_tanks;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.drag = 0;
        _rb.angularDrag = 0;
        _rb.centerOfMass = Vector3.zero;
        _rb.mass = 0;

        _fuel_tanks = new List<FuelTank>();

        foreach (SpacecraftPart part in root_part.getParts())
        {
            part.Init(this);
        }
        
        recalculateMass();
    }

    void FixedUpdate()
    {
        recalculateMass();
    }

    // recalculates mass and center of mass
    private void recalculateMass()
    {
        Vector3 com = Vector3.zero;
        float m = 0;
        foreach (SpacecraftPart part in root_part.getParts())
        {
            com += transform.InverseTransformPoint(part.transform.position) * part.mass;
            m += part.mass;
        }
        com /= m;

        _rb.mass = m;
        _rb.centerOfMass = com;
    }

    public void setHeading(Vector3 heading, bool stablize)
    {
        _target_heading = heading;
        if (_target_heading.magnitude == 0)
        {
            _target_heading = transform.up;
        }
        _target_up = transform.forward;
        _lock_heading = stablize;
    }

    public void setHeading(Vector3 heading, Vector3 up, bool stablize)
    {
        _target_heading = heading;
        if (_target_heading.magnitude == 0)
        {
            _target_heading = transform.up;
        }
        _target_up = up;
        if (_target_up.magnitude == 0)
        {
            _target_up = transform.forward;
        }
        _lock_heading = stablize;
    }

    public float useFuel(FuelType type, float amount)
    {
        FuelTank priority_tank = null;
        foreach (FuelTank tank in _fuel_tanks)
        {
            if (tank.fuel_type == type
                && (priority_tank == null
                || tank.priority > priority_tank.priority))
            {
                priority_tank = tank;
            }
        }

        // if the spacecraft doesn't have a tank with the specified fuel type
        if (priority_tank == null)
        {
            return 0;
        }
        
        float used = priority_tank.useFuel(amount);
        return used;
    }

    public void addFuelTank(FuelTank tank)
    {
        _fuel_tanks.Add(tank);
    }

    void Update()
    {
        // Debug.DrawLine(transform.position, transform.position + target_heading.normalized * 90, Color.red);
    }
}
