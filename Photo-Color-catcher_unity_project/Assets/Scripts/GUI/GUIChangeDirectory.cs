using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to change the actual directory.
/// </summary>
public class GUIChangeDirectory : GUITool
{
    /// <summary>
    /// The path of the new actual directory.
    /// </summary>
    public string _path;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        FileSystemManager.Instance.StartChangeDirectory(_path);
    }
}
