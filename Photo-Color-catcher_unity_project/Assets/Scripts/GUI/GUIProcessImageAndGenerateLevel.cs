using UnityEngine;
using System.Collections;

public class GUIProcessImageAndGenerateLevel : GUITool
{
    public Texture2D _img;

    public int[] _imageConfig;

    public override void Execute()
    {
        ImgProcessManager.Instance.StartProcessImageAndGenerateLevel(_img, _imageConfig);
    }

}
