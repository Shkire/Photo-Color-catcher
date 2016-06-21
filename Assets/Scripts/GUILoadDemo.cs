using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadDemo : GUITool {

	[SerializeField]
	private string demo;

	public override void Execute()
	{
		SceneManager.LoadScene ("scene"+demo);
	}

}
