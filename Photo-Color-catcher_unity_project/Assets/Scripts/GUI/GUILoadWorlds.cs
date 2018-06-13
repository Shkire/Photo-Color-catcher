using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILoadWorlds : GUITool {

    public override void Execute()
    {
        WorldLoadingManager.Instance.StartLoadWorlds();
    }
}
