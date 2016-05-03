using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelConfigListAsset{

	[MenuItem("Assets/Create/LevelConfigList")]
	[ContextMenu("Assets/Create/LevelConfigList")]
	public static void CreateAsset()
	{
		CustomAssetUtility.CreateAsset<LevelConfigList> ();
	}

}
