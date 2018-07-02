using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to launch a FXSequence.
/// </summary>
public class GUILaunchFX : GUITool
{

    /// <summary>
    /// The target FXSequence.
    /// </summary>
    public GameObject _target;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        if (_target != null)
            _target.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);
    }
}
