using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadScene : GUITool {

	public string scene;

	public override void Execute()
	{
		SceneManager.LoadScene (scene);
	}
}
