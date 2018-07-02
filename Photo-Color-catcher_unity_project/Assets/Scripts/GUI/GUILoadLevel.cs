using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// A GUITool used to load a previously generated level.
/// </summary>
public class GUILoadLevel : GUITool
{

    /// <summary>
    /// The World path.
    /// </summary>
    public string _path;

    /// <summary>
    /// The level position in the world.
    /// </summary>
    public Vector2 _levelPos;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        LevelLoadingManager.Instance.StartLoadLevel(_path, _levelPos);
    }
}
