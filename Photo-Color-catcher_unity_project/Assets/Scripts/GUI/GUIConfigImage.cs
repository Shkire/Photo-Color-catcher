using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GUITool used to configurate an image for processing and level generation.
/// </summary>
public class GUIConfigImage : GUITool
{
    /// <summary>
    /// The path of the image.
    /// </summary>
    public string path;

    public override void Execute()
    {
        ImageConfigurationManager.Instance.GetImageConfigurations(path);
    }

}
