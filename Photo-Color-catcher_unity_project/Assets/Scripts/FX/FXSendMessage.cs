using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FXElement that allows to send a message to the target GameObject.
/// </summary>
public class FXSendMessage : FXElement
{

    /// <summary>
    /// The message to send.
    /// </summary>
    public string _value;

    /// <summary>
    /// Starts the FX.
    /// </summary>
    /// <param name="i_duration">Duration of the FX.</param>
    public override IEnumerator StartFX(float i_duration)
    {
        _target.SendMessage(_value, SendMessageOptions.DontRequireReceiver);

        yield break;
    }
}
