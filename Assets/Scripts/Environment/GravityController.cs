using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class StateVectors
{
    // used for null values
    public static StateVectors zero
    {
        get {return new StateVectors(Vector3.zero, Vector3.zero, 0);}
    }
    public Vector3 position;
    public Vector3 velocity;
    public float mass;

    public StateVectors(Vector3 p, Vector3 v, float m)
    {
        position = p;
        velocity = v;
        mass = m;
    }

    public StateVectors(StateVectors s)
    {
        position = s.position;
        velocity = s.velocity;
        mass = s.mass;
    }

    // returns a list with this as it's only element, useful for certain list operations (Concat, Except, etc.)
    public List<StateVectors> getList()
    {
        List<StateVectors> ret = new List<StateVectors>();
        ret.Add(this);
        return ret;
    }

    public bool isZero()
    {
        return (position == Vector3.zero && velocity == Vector3.zero && mass == 0);
    }
}

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    public static float G = 1000;
    public static List<GravityController> all_bodies = new List<GravityController>();
    public bool attractor = true;
    public Vector3 initial_velocity = Vector3.zero;
    public StateVectors state
    {
        get { return new StateVectors(_rb.position, _true_velocity, _rb.mass); }
    }
    public bool debug = false;
    public int path_steps = 0;
    public float path_delta = 0;
    public GravityController reference;
    
    private Rigidbody _rb;
    private StateVectors _prev_state = StateVectors.zero;
    private Vector3 _true_velocity = Vector3.zero;
    private Vector3 _prev_false_velocity = Vector3.zero;
    private List<StateVectors> _future_path = new List<StateVectors>();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0;
        _rb.angularDrag = 0;
        _rb.useGravity = false;
        _rb.velocity = initial_velocity;

        _true_velocity = initial_velocity;
        _prev_false_velocity = initial_velocity;
        _prev_state = state;
        
        all_bodies.Add(this);
    }

    void Update()
    {
        if (_future_path.Count != 0)
        {
            drawPath(_future_path);
        }
    }

    void FixedUpdate()
    {
        // _true_velocity += _rb.velocity - _prev_false_velocity;
    }

    public static void updateState()
    {
        foreach (GravityController body in all_bodies)
        {
            // set current state to previous state
            body._prev_state = new StateVectors(body.state);
        }
    }

    public static void attractBodies(float timestep)
    {
        foreach (GravityController body in all_bodies)
        {
            // update true state with acceleration that has occured between updates
            body._true_velocity += body._rb.velocity - body._prev_false_velocity;
            attract(body, timestep);
            if (body.debug)
            {
                List<StateVectors> other = new List<StateVectors>();
                List<StateVectors> other_prev = new List<StateVectors>();
                foreach (GravityController b in all_bodies)
                {
                    if (!b.attractor || b == body)
                    {
                        continue;
                    }
                    other.Add(b.state);
                    other_prev.Add(b._prev_state);
                }
                body._future_path = calcFuturePath(body.state, body._prev_state, other, other_prev, body.path_delta, body.path_steps);

                OrbitalElements elements = Orbit.getElements(body.state, body.reference.state);
                Debug.Log("ECC: \t\t" + elements.ecc);
                Debug.Log("SMA: \t\t" + elements.sma);
                Debug.Log("INC: \t\t" + elements.inc);
                Debug.Log("LONG ASC: \t" + elements.long_asc * Mathf.Rad2Deg);
                Debug.Log("ARG PERI: \t" + elements.arg_peri * Mathf.Rad2Deg);
                Debug.Log("TRUE ANOM: \t" + elements.true_anom * Mathf.Rad2Deg);
                Debug.Log("---------------------------------");
            }
        }
    }

    public static Vector3 calcTotalGravity(StateVectors s, List<StateVectors> o)
    {
        // calculate total gravitational force on an object
        Vector3 grav_force = Vector3.zero;
        foreach (StateVectors b in o)
        {
            // law of gravitation
            Vector3 rel_pos = b.position - s.position;
            grav_force += rel_pos.normalized * G * b.mass / rel_pos.sqrMagnitude;
        }
        return grav_force;
    }
    
    static void attract(GravityController body, float timestep)
    {
        // exclude this body
        List<StateVectors> other = new List<StateVectors>();
        foreach (GravityController b in all_bodies)
        {
            if (!b.attractor || b == body)
            {
                continue;
            }
            other.Add(b.state);
        }

        // calculate position and velocity next timestep
        StateVectors next = integrate(body.state, other, timestep);

        // set physics velocity such that the body will arrive at next_position next timestep
        body._rb.velocity = (next.position - body._rb.position) / timestep;
        body._prev_false_velocity = body._rb.velocity;

        // save the body's true state
        body._true_velocity = next.velocity;
    }

    public static List<StateVectors> calcFuturePath(StateVectors s, StateVectors sp, List<StateVectors> o, List<StateVectors> op, float timestep, int epochs)
    {
        List<StateVectors> prev_o = new List<StateVectors>();
        List<StateVectors> current_o = new List<StateVectors>();
        List<StateVectors> next_o = new List<StateVectors>(o);
        StateVectors prev_s = StateVectors.zero;
        StateVectors current_s = StateVectors.zero;
        StateVectors next_s = s;
        List<StateVectors> s_path = new List<StateVectors>();

        for (int i = 0; i < epochs; i++)
        {
            prev_s = current_s;
            current_s = next_s;
            prev_o = current_o;
            current_o = new List<StateVectors>(next_o);
            next_o.Clear();

            // if (prev_s.isZero())
            // {
            //     next_s = integrate_euler(current_s, current_o, timestep);
            //     for (int j = 0; j < current_o.Count; j++)
            //     {
            //         PhysicsState a = current_o[j];

            //         // all current states except a
            //         List<PhysicsState> _o = new List<PhysicsState>(current_o.Concat(new List<PhysicsState>(current_s.getList())).Except(a.getList()));
            //         next_o.Add(integrate_euler(a, _o, timestep));
            //     }
            //     continue;
            // }

            // get s on next timestep
            next_s = integrate(current_s, current_o, timestep);

            // get every element of o on next timestep
            for (int j = 0; j < current_o.Count; j++)
            {
                StateVectors a = current_o[j];
                // PhysicsState ap = prev_o[j];

                // all current states except a
                List<StateVectors> _o = new List<StateVectors>(current_o.Concat(new List<StateVectors>(current_s.getList())).Except(a.getList()));
                next_o.Add(integrate(a, _o, timestep));
            }

            s_path.Add(next_s);
        }

        return s_path;
    }

    public static void drawPath(List<StateVectors> path)
    {
        StateVectors prev = StateVectors.zero;
        foreach (StateVectors s in path)
        {
            if (!prev.isZero())
            {
                Debug.DrawLine(prev.position, s.position, Color.red);
            }
            prev = s;
        }
    }
    
    // runge kutta 4 integrator (used for simulation)
    static StateVectors integrate(StateVectors s, List<StateVectors> o, float timestep)
    {
        Vector3 k1p = s.velocity;
        Vector3 k1v = calcTotalGravity(s, o);

        Vector3 k2p = s.velocity + k1v * (timestep / 2);
        StateVectors a_halfstep_v = new StateVectors(s.position + k1p * (timestep / 2), Vector2.zero, s.mass);
        Vector3 k2v = calcTotalGravity(a_halfstep_v, o);

        Vector3 k3p = s.velocity + k2v * (timestep / 2);
        StateVectors a_halfstep2_v = new StateVectors(s.position + k2p * (timestep / 2), Vector2.zero, s.mass);
        Vector3 k3v = calcTotalGravity(a_halfstep2_v, o);

        Vector3 k4p = s.velocity + k3v * timestep;
        StateVectors a_finalstep_v = new StateVectors(s.position + k3p * timestep, Vector2.zero, s.mass);
        Vector3 k4v = calcTotalGravity(a_finalstep_v, o);

        Vector3 position = s.position + (k1p + k2p * 2 + k3p * 2 + k4p) / 6 * timestep;
        Vector3 velocity = s.velocity + (k1v + k2v * 2 + k3v * 2 + k4v) / 6 * timestep;
        StateVectors next_state = new StateVectors(position, velocity, s.mass);

        return next_state;
    }

    // verlet integrator (used for prediction)
    static StateVectors integrate_verlet(StateVectors s, StateVectors p, List<StateVectors> o, float timestep)
    {
        Vector3 acc = calcTotalGravity(s, o);
        StateVectors next_state = new StateVectors(2 * s.position - p.position + acc * Mathf.Pow(timestep, 2), s.velocity + acc * timestep, s.mass);

        return next_state;
    }

    // Euler integerator
    static StateVectors integrate_euler(StateVectors s, List<StateVectors> o, float timestep)
    {
        Vector3 acc = calcTotalGravity(s, o);
        StateVectors next_state = new StateVectors(s.position + s.velocity * timestep + acc * Mathf.Pow(timestep, 2), s.velocity + acc * timestep, s.mass);

        return next_state;
    }
}
