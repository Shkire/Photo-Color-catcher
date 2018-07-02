using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FXElement that allows to enable or disable the target GameObject.
/// </summary>
public class FXEnableObject : FXElement
{

    /// <summary>
    /// If the GameObject will be enabled or disabled.
    /// </summary>
    public bool _enable;

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public override IEnumerator StartFX(float i_duration)
    {
        if (_target != null)
            _target.SetActive(_enable);

        yield break;
    }
}
