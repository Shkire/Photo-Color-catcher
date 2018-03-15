using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlatformPatternConfig
{
	public enum PlatformPattern
	{
		Undefined,
		Line,
		Stair,
		ZigZag,
		Copy
	}

	public PlatformPattern pattern;
	public int minLines;
	public int maxLines;
	public int minPiecesByPlatform;
	public int maxPiecesByPlatform;
	public int maxPlatforms;
}

[System.Serializable]
public class LevelConfig {

	public string level;
	public int teleportMin;
	public int teleportMax;
	public int maxTeleportNumBySide;
	//-------------------
	public int minPiecesByPlatform;
	public int maxPiecesByPlatform;
	public int minPlatforms;
	public int maxPlatforms;
	//----------------
	public PlatformPatternConfig[] patternConfigList;
}
