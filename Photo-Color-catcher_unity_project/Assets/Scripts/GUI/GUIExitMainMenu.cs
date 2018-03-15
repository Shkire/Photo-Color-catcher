using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUIExitMainMenu : GUITool {

	public override void Execute()
	{
		//Tags de compilación

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

}
