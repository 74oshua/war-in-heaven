using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spacecraft))]
public class AutoSpacecraftController : MonoBehaviour
{
    private Spacecraft _sc;
    public Spacecraft target;

    void Start()
    {
        _sc = GetComponent<Spacecraft>();
    }

    void FixedUpdate()
    {
        _sc.setHeading(target.transform.position - transform.position, true);
    }
}
