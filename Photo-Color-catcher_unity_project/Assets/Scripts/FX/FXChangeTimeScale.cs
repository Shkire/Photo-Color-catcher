using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FXElement that allows to change the time scale of the game.
/// </summary>
public class FXChangeTimeScale : FXElement
{

    /// <summary>
    /// The new value of the time scale.
    /// </summary>
    public float _value;

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public override IEnumerator StartFX(float i_duration)
    {
        Time.timeScale = _value;

        yield break;
    }
}
