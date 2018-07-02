using UnityEngine;
using System.Collections;

/// <summary>
/// A GUITool used to enable or disable a GameObject.
/// </summary>
public class GUIObjectEnable : GUITool
{

    /// <summary>
    /// The target GameObject.
    /// </summary>
    public GameObject _target;

    /// <summary>
    /// If the GameObject will be enabled or disabled.
    /// </summary>
    public bool _enable;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        if (_target != null)
            _target.SetActive(_enable);
    }
}
