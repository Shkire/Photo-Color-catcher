using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILaunchFX : GUITool{

    public GameObject _target;

    public override void Execute()
    {
        if (_target != null)
            _target.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);
    }
}
