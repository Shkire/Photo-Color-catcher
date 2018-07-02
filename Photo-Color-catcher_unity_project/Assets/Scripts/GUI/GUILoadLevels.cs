using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to load and show (for level selection) all the levels of a previously generated world.
/// </summary>
public class GUILoadLevels : GUITool
{

    /// <summary>
    /// The World path.
    /// </summary>
    public string _path;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        WorldLevelsLoadingManager.Instance.StartLoadLevels(_path);
    }

}
