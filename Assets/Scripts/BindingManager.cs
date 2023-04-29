using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BindingManager
{
    private static Dictionary<string, KeyCode> bindings = new Dictionary<string, KeyCode>();
    private static Dictionary<string, string> axes = new Dictionary<string, string>();

    public static void AddBinding(string name, KeyCode key)
    {
        bindings.Add(name, key);
    }

    public static void AddAxis(string name, string axis)
    {
        axes.Add(name, axis);
    }

    public static KeyCode GetBinding(string name)
    {
        if (bindings.ContainsKey(name))
        {
            return bindings[name];
        }
        Debug.LogError("No binding set for " + name);
        return KeyCode.None;
    }

    public static string GetAxis(string name)
    {
        if (axes.ContainsKey(name))
        {
            return axes[name];
        }
        Debug.LogError("No axis set for " + name);
        return "";
    }

    public static bool GetKeyState(string name)
    {
        return Input.GetKey(GetBinding(name));
    }

    public static bool GetKeyStateDown(string name)
    {
        return Input.GetKeyDown(GetBinding(name));
    }

    public static bool GetKeyStateUp(string name)
    {
        return Input.GetKeyUp(GetBinding(name));
    }

    public static float GetAxisValue(string name)
    {
        return Input.GetAxis(GetAxis(name));
    }
}
