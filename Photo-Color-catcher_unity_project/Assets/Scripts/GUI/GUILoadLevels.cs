using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILoadLevels : GUITool
{

    public string _path;

    public override void Execute()
    {
        WorldLevelsLoadingManager.Instance.StartLoadLevels(_path);
    }

}
