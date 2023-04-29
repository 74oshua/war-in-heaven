using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalElements
{
    public Vector3 ecc_v = new Vector3();
    public float ecc
    {
        get { return ecc_v.magnitude; }
    }
    public float sma;
    public float inc;
    public float long_asc;
    public float arg_peri;
    public float true_anom;
}

public static class Orbit
{
    public static OrbitalElements getElements(StateVectors body, StateVectors reference)
    {
        OrbitalElements o = new OrbitalElements();

        float g = GravityController.G * (body.mass + reference.mass);
        Vector3 r = body.position - reference.position;
        Vector3 v = body.velocity - reference.velocity;
        Vector3 h = Vector3.Cross(r, v);
        float spec_oe = v.sqrMagnitude / 2 - g / r.magnitude;

        o.ecc_v = Vector3.Cross(v, h) / g - r.normalized;
        o.sma = -g / (2 * spec_oe);

        o.inc = Mathf.Acos(h.normalized.z);
        Vector3 n = Vector3.Cross(Vector3.forward, h);
        o.long_asc = Mathf.Acos(n.normalized.x);
        if (n.y < 0)
        {
            o.long_asc = 2 * Mathf.PI - o.long_asc;
        }

        o.arg_peri = Mathf.Acos(Vector3.Dot(n, o.ecc_v) / (n.magnitude * o.ecc));
        o.true_anom = Mathf.Acos(Vector3.Dot(o.ecc_v, r) / (o.ecc * r.magnitude));
        if (Vector3.Dot(o.ecc_v, r) < 0)
        {
            o.true_anom = 2 * Mathf.PI - o.true_anom;
        }

        return o;
    }

    public static float getCircularOrbitalSpeed(float mass, float radius)
    {
        return Mathf.Sqrt(GravityController.G * mass / radius);
    }
}
