using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUILoadLevel : GUITool {

	public int id;

	public bool isParent;

	public override void Execute()
	{
		if (isParent)
			LevelLoader.Instance.LoadLevelSelector (id);
		else
			LevelLoader.Instance.LoadLevel (id);
	}

	public void SetId(int i_id)
	{
		id = i_id;
	}
}
