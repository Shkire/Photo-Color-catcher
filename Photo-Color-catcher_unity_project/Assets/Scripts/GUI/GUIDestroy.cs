using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to destroy the target GameObject.
/// </summary>
public class GUIDestroy : GUITool
{
    /// <summary>
    /// The target GameObject.
    /// </summary>
    public GameObject target;

    public override void Execute()
    {
        Destroy(target);
    }
}
