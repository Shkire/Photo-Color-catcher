using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to load and show (for world selection) all the previously generated worlds.
/// </summary>
public class GUILoadWorlds : GUITool {

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        WorldLoadingManager.Instance.StartLoadWorlds();
    }
}
