using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelConfig {

	public string level;
	public int teleportMin;
	public int teleportMax;
	public int maxTeleportNumBySide;
	public int minPiecesByPlatform;
	public int maxPiecesByPlatform;
	public int minPlatforms;
	public int maxPlatforms;
}
