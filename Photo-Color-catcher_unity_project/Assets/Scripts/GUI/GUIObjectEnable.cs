using UnityEngine;
using System.Collections;

public class GUIObjectEnable : GUITool {

	public GameObject target;
	public bool enable;

	public override void Execute()
	{
		if (target != null)
			target.SetActive (enable);
	}
}
