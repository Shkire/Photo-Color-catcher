using UnityEngine;
using System.Collections;

public class GUIProcessImage : GUITool {

	public string path;

	public override void Execute()
	{
		ImgProcessManager.Instance.ProcessAndIndexImage (path);
	}

}
