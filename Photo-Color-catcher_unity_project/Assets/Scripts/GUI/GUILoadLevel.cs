using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadLevel : GUITool
{

    public string _path;

    public Vector2 _levelPos;

    public override void Execute()
    {
        LevelLoadingManager.Instance.StartLoadLevel(_path, _levelPos);
    }
}
