using UnityEngine;
using System.Collections;

public class LevelConfigList : ScriptableObject{

	public LevelConfig[] levelConfigList;

	public PlatformPatternConfig[] GetPatterns(int i_dificulty)
	{
		foreach (LevelConfig config in levelConfigList) 
		{
			if (i_dificulty.ToString ().Equals (config.level)) 
			{
				return config.patternConfigList;
			}
		}
		return null;
	}

}
