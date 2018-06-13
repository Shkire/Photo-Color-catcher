using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXEnableObject : FXElement{

    public bool _enable;

    public override IEnumerator StartFX(float i_duration)
    {
        if (_target != null)
            _target.SetActive(_enable);

        yield break;
    }
}
