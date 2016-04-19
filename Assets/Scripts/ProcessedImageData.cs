using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessedImageData{

	public enum LevelType
	{
		Undefined,
		GhostHunting,
		TimeTrial,
		BossBattle
	}

	private LevelType levelType;

	private List<MapScheme> levelSchemes;

	private float redProportion;

	private float greenProportion;

	private float blueProportion;

	private float redSaturation;

	private float greenSaturation;

	private float blueSaturation;

	private float blackLevel;

	private float dificulty;

	public ProcessedImageData(float i_redProportion, float i_greenProportion, float i_blueProportion, float i_redSaturation, float i_greenSaturation, float i_blueSaturation, float i_blackLevel)
	{
		levelType = LevelType.Undefined;
		levelSchemes = new List<MapScheme> ();
		redProportion = i_redProportion;
		greenProportion = i_greenProportion;
		blueProportion = i_blueProportion;
		redSaturation = i_redSaturation;
		greenSaturation = i_greenSaturation;
		blueSaturation = i_blueSaturation;
		blackLevel = i_blackLevel;
	}
}
