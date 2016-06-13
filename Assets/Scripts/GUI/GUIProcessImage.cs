using UnityEngine;
using System.Collections;

public class GUIProcessImage : GUITool {

	public override void Execute()
	{
		ImgProcessManager.Instance.ProccesAndIndexImage ();
		Debug.Log ("HE PROCESADO");
	}

}
