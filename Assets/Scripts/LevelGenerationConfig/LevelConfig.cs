using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelConfig {

	public string level;
	public int teleportMin;
	public int teleportMax;
	public int maxTeleportNumBySide;
	public int platformPiecesMin;
	public int platformPiecesMax;
	public int maxPiecesByPlatform;
	public int maxPlatforms;
}
