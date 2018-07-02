using UnityEngine;
using System.Collections;

/// <summary>
/// GUIObject that reacts when is focused or unfocused.
/// </summary>
public class GUIDecoration : GUIObject
{

    /// <summary>
    /// Describes the possible situations when the decoration is activated.
    /// </summary>
    public enum DecorationContext
    {
        WhenFocused,
        WhenNonFocused,
        WhenSelected,
        Always,
        Never
    }

    public DecorationContext _behaviour;

    public override void Focused()
    {
        base.Focused();
        if (_behaviour.Equals(DecorationContext.Always) || _behaviour.Equals(DecorationContext.WhenFocused))
        {
            foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
                script.enabled = true;
            foreach (Renderer rend in GetComponents<Renderer>())
                rend.enabled = true;
            foreach (Behaviour comp in GetComponents<Behaviour>())
                comp.enabled = true;
        }
        else
        {
            foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
                script.enabled = false;
            foreach (Renderer rend in GetComponents<Renderer>())
                rend.enabled = false;
            foreach (Behaviour comp in GetComponents<Behaviour>())
                comp.enabled = false;
        }
    }

    public override void NonFocused()
    {
        base.NonFocused();
        if (_behaviour.Equals(DecorationContext.Always) || _behaviour.Equals(DecorationContext.WhenNonFocused))
        {
            foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
                script.enabled = true;
            foreach (Renderer rend in GetComponents<Renderer>())
                rend.enabled = true;
            foreach (Behaviour comp in GetComponents<Behaviour>())
                comp.enabled = true;
        }
        else
        {
            foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
                script.enabled = false;
            foreach (Renderer rend in GetComponents<Renderer>())
                rend.enabled = false;
            foreach (Behaviour comp in GetComponents<Behaviour>())
                comp.enabled = false;
        }
    }

    public override void Selected()
    {
        base.Selected();
        if (_behaviour.Equals(DecorationContext.WhenSelected))
        {
            foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
                script.enabled = true;
            foreach (Renderer rend in GetComponents<Renderer>())
                rend.enabled = true;
            foreach (Behaviour comp in GetComponents<Behaviour>())
                comp.enabled = true;
        }
    }
}
