using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIConfigImage : GUITool {

    public string path;

    public override void Execute()
    {
        ImageConfigurationManager.Instance.GetImageConfigurations(path);
    }

}
