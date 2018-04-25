using UnityEngine;
using System.Collections;

public class GUIGenerateWorld : GUITool
{
    public Texture2D _img;

    public int[] _imageConfig;

    public string _name;

    public override void Execute()
    {
        ImgProcessManager.Instance.StartProcessImageAndGenerateWorld(_img, _imageConfig, _name);
    }

}
