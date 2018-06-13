using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXSendMessage : FXElement {

    public string _value;

    public override IEnumerator StartFX(float i_duration)
    {
        _target.SendMessage(_value, SendMessageOptions.DontRequireReceiver);

        yield break;
    } 
}
