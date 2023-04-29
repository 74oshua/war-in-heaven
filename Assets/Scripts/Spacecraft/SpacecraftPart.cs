using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftPart : MonoBehaviour
{
    // mass of part in kg
    public float mass = 1;

    // list of child parts attached to this
    public List<SpacecraftPart> attached = new List<SpacecraftPart>();

    // parent spacecraft
    protected Spacecraft _sc;

    // _sc rigidbody component
    protected Rigidbody _rb;

    virtual public void Init(Spacecraft sc)
    {
        _sc = sc;
        _rb = sc.GetComponent<Rigidbody>();
    }

    // recursively gets all child parts
    public List<SpacecraftPart> getParts()
    {
        List<SpacecraftPart> parts = new List<SpacecraftPart>();
        parts.Add(this);
        foreach (SpacecraftPart part in attached)
        {
            parts.AddRange(part.getParts());
        }
        return parts;
    }
}
