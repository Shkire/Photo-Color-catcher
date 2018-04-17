using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIChangeDirectory : GUITool {

    public string path;

    public override void Execute()
    {
        FileSystemManager.Instance.ChangeDirectory(path);
    }
}
