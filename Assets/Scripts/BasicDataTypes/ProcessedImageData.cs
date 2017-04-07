﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CellContent = MapScheme.CellContent;

[System.Serializable]
public class ProcessedImageData{

	/*
	public enum LevelType
	{
		Undefined,
		GhostHunting,
		TimeTrial,
		BossBattle
	}

	private LevelType levelType;
	*/

	private List<MapScheme> levelSchemes;

	private float redProportion;

	private float greenProportion;

	private float blueProportion;

	private float redSaturation;

	private float greenSaturation;

	private float blueSaturation;

	private float grayLevel;

	private int dificulty;

	public ProcessedImageData(float i_redProportion, float i_greenProportion, float i_blueProportion, float i_grayAverage, float i_grayMedian)
	{
		System.Random rnd = new System.Random (Guid.NewGuid ().GetHashCode ());
		//levelType = LevelType.Undefined;
		levelSchemes = new List<MapScheme> ();
		redProportion = i_redProportion;
		greenProportion = i_greenProportion;
		blueProportion = i_blueProportion;
		//redSaturation = i_redSaturation;
		//greenSaturation = i_greenSaturation;
		//blueSaturation = i_blueSaturation;
		//grayLevel = i_grayLevel;
		//dificulty = 10 - rnd.Next (0, Mathf.RoundToInt (i_grayLevel * 10));
		int ghostHuntingLimit =1+Mathf.RoundToInt (redProportion * 10);
		int timeTrialLimit = 1 + ghostHuntingLimit + Mathf.RoundToInt (greenProportion * 10);
		int bossBattleLimit = 1 + timeTrialLimit + Mathf.RoundToInt (blueProportion * 10);
		int aux = rnd.Next (bossBattleLimit + 1);
		/*
		if (aux <= ghostHuntingLimit)
			levelType = LevelType.GhostHunting;
		else if (aux <= timeTrialLimit)
			levelType = LevelType.TimeTrial;
		else
			levelType = LevelType.BossBattle;
			*/
		levelSchemes = new List<MapScheme> ();
		levelSchemes.Add (new MapScheme());
	}

	public void CreateSchemes()
	{
		//MapScheme tempScheme = new MapScheme (levelType, dificulty);
		//levelSchemes.Add (tempScheme);
	}

	public int GetDificulty()
	{
		return dificulty;
	}

	public List<CellContent>[,] GetScheme()
	{
		return levelSchemes [0].GetScheme ();
	}
}
