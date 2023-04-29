using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyOriginShiftController : OriginShiftController
{
    public override void shift(Vector3 offset)
    {
        base.shift(offset);

        GetComponent<Rigidbody>().position -= offset;
    }
}
