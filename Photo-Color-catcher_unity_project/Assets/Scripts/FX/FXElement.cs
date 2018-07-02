using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Unity Component used to implement FX Components.
/// </summary>
public abstract class FXElement : MonoBehaviour
{

    /// <summary>
    /// The target of the FX.
    /// </summary>
    public GameObject _target;

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public virtual IEnumerator StartFX(float i_duration)
    {
        yield return null;
    }
}
