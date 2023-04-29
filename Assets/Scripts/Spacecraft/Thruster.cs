using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : SpacecraftPart
{
    // standard gravity in m/s^2
    public static float g0 = 9.80669f;

    // public float thrust = 1;
    public bool main_drive = false;

    public FuelType fuel_type = FuelType.HYDROGEN;
    public float isp = 190f;
    public float mass_flow_rate = 0.7f;

    private float _throttle = 0;
    public float throttle
    {
        get { return _throttle; }
    }

    private float _fuel_used;
    public float fuel_used
    {
        get { return _fuel_used; }
    }

    private Vector3 _prev_angular_velocity;

    void FixedUpdate()
    {
        if (!main_drive)
        {
            Vector3 thruster_torque = Vector3.Cross(transform.position - _rb.worldCenterOfMass, transform.up * isp * g0 * mass_flow_rate);
            Vector3 target_rotation_axis = Vector3.Cross(_sc.transform.up, _sc.target_heading.normalized) + Vector3.Cross(_sc.transform.forward, _sc.target_up);

            if (_rb.angularVelocity.magnitude > 0.001f && (Vector3.Angle(_sc.transform.up, _sc.target_heading) / 360) > 0.001f)
            {
                target_rotation_axis -= _rb.angularVelocity / Mathf.Sqrt(Vector3.Angle(_sc.transform.up, _sc.target_heading) / 180) * (Mathf.PI / 2);
            }

            _throttle = Mathf.Clamp(Vector3.Dot(thruster_torque, target_rotation_axis) / thruster_torque.magnitude, 0, 1);

            // Vector3 target_rotation_axis = Vector3.Cross(_sc.transform.up, _sc.target_heading.normalized) / (Mathf.PI) + Vector3.Cross(_sc.transform.forward, _sc.target_up.normalized) / (Mathf.PI);
            // // if (_sc.lock_heading)
            // // {
            // //     // target_rotation_axis -= _rb.angularVelocity / Mathf.Sqrt(Mathf.Clamp(Vector3.Angle(_sc.transform.up, _sc.target_heading) / 180, 0, 1));
            // //     // if (Vector3.Angle(_sc.transform.up, _sc.target_heading) < 5)
            // //     // {
            // //     //     target_rotation_axis = -_rb.angularVelocity;
            // //     // }
            // //     if (_rb.angularVelocity.magnitude > 0)
            // //     {
            // //         target_rotation_axis -= _rb.angularVelocity / (Vector3.Angle(_sc.transform.up, _sc.target_heading) * Mathf.Deg2Rad) / (2f);
            // //     }
            // // }
            // Vector3 target_thrust_axis = Vector3.Cross(target_rotation_axis, (transform.position - _rb.position + _rb.centerOfMass).normalized);
            // // Debug.DrawLine(transform.position, transform.position + _sc.target_heading.normalized * 100, Color.red);
            // // Debug.DrawLine(transform.position, transform.position + target_rotation_axis * 100, Color.blue);
            // // Debug.DrawLine(transform.position, transform.position + target_thrust_axis * 100, Color.green);
            // float alignment = Mathf.Clamp(Vector3.Dot(transform.up, target_thrust_axis), 0, 1);
            // if (!float.IsNaN(alignment))
            // {
            //     // _throttle = (Mathf.Sqrt(Vector3.Angle(_sc.transform.up, _sc.target_heading) / 180f) + (_rb.angularVelocity.magnitude / (Mathf.PI * 2)) * 100f) * alignment;
            //     _throttle = alignment;
            // }
            // else
            // {
            //     _throttle = 0;
            // }

            _throttle = Mathf.Clamp(_throttle, 0, 1);
        }
        else
        {
            _throttle = _sc.main_throttle;
        }


        // consume (mass_flow_rate * Time.fixedDeltaTime * _throttle) kg of fuel
        _fuel_used = _sc.useFuel(fuel_type, mass_flow_rate * Time.fixedDeltaTime * _throttle);
        
        _rb.AddForceAtPosition(transform.up * isp * g0 * (_fuel_used / Time.fixedDeltaTime), transform.position, ForceMode.Force);
    }
}
