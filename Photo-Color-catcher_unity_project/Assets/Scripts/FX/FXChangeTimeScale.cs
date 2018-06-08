using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXChangeTimeScale : FXElement {

    public float _value;

    public override IEnumerator StartFX(float i_duration)
    {
        Time.timeScale = _value;

        yield break;
    }
}
