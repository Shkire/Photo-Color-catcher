using UnityEngine;
using System.Collections;

public class GUISelectableElement : GUIObject{

	public override void Selected()
	{
		base.Selected ();
		gameObject.SendMessage ("Execute");
	}

}
