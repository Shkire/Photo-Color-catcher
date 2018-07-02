using UnityEngine;
using System.Collections;

/// <summary>
/// Base Unity Component used to implement UI behaviours.
/// </summary>
public abstract class GUIObject : MonoBehaviour
{

    public virtual void Focused()
    {
        foreach (GameObject child in this.gameObject.GetChildren())
            child.SendMessage("Focused", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void NonFocused()
    {
        foreach (GameObject child in this.gameObject.GetChildren())
            child.SendMessage("NonFocused", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void Selected()
    {
        foreach (GameObject child in this.gameObject.GetChildren())
            child.SendMessage("Selected", SendMessageOptions.DontRequireReceiver);
    }
}
