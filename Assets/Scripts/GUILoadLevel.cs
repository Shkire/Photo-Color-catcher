using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadLevel : GUITool {

	private int id;

	public override void Execute()
	{
		LevelLoader.Instance.LoadAndConfig(id);
	}

	public void SetId(int i_id)
	{
		id = i_id;
	}
}
