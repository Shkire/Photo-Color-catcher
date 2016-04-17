using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadLevel : GUITool {

	public override void Execute()
	{
		SceneManager.LoadScene (0);
	}

}
