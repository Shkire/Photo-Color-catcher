using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelConfigListAsset{
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/LevelConfigList")]
	[ContextMenu("Assets/Create/LevelConfigList")]
	public static void CreateAsset()
	{
		CustomAssetUtility.CreateAsset<LevelConfigList> ();
	}
	#endif

}
