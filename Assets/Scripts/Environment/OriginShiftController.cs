using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginShiftController : MonoBehaviour
{
    public static List<OriginShiftController> controllers = new List<OriginShiftController>();

    void Awake()
    {
        controllers.Add(this);
    }

    public static void shiftAll(Vector3 offset)
    {
        foreach (OriginShiftController controller in controllers)
        {
            controller.shift(offset);
        }
    }

    public virtual void shift(Vector3 offset)
    {
        transform.position -= offset;
    }
}
