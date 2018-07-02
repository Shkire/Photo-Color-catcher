using UnityEngine;
using System.Collections;

/// <summary>
/// A GUITool used to generate a world from a previously configurated image.
/// </summary>
public class GUIGenerateWorld : GUITool
{
    
    /// <summary>
    /// The image.
    /// </summary>
    public Texture2D _img;

    /// <summary>
    /// The image division configuration ([columns,row]).
    /// </summary>
    public int[] _imageDivisionConfig;

    /// <summary>
    /// The image name.
    /// </summary>
    public string _name;

    /// <summary>
    /// Executes the action associated with the GUITool.
    /// </summary>
    public override void Execute()
    {
        ImgProcessManager.Instance.StartProcessImageAndGenerateWorld(_img, _imageDivisionConfig, _name);
    }

}
