using UnityEngine;
using System.Collections;

public class GUIDecoration : GUIObject
{

    public enum DecorationContext
    {
        WhenFocused,
        WhenNonFocused,
        WhenSelected,
        Always,
        Never
    }

    public DecorationContext behaviour;

    public override void Focused()
    {
        base.Focused();
        if (behaviour.Equals(DecorationContext.Always) || behaviour.Equals(DecorationContext.WhenFocused))
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
        if (behaviour.Equals(DecorationContext.Always) || behaviour.Equals(DecorationContext.WhenNonFocused))
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
        if (behaviour.Equals(DecorationContext.WhenSelected))
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
