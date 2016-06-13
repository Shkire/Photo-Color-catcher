using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Color = UnityEngine.Color;

[System.Serializable]
public class ImgPersistanceInfo
{
	private string p_path;
	private List<int> children;
	private Color[] pixels;

	public string path
	{
		get
		{
			return p_path;
		}
	}

	public ImgPersistanceInfo (string i_path, List<int> i_children, Color[] i_pixels)
	{
		p_path = i_path;
		children = i_children;
		pixels = i_pixels;
	}

	public bool Contains(int i_id)
	{
		return children.Contains (i_id);
	}
}

[System.Serializable]
public class MainGameData 
{
	private Dictionary<int,ImgPersistanceInfo> parentImgs;
	private int lastId;
	private List<int> otherAvailId;

	public MainGameData()
	{
		parentImgs = new Dictionary<int,ImgPersistanceInfo> ();
		lastId = 0;
		otherAvailId = new List<int> ();
	}

	public bool otherAvail{
		get{
			return otherAvailId.Count > 0;
		}
	}

	public string GetPath(int i_id)
	{
		if (parentImgs != null && parentImgs.ContainsKey (i_id))
			return parentImgs [i_id].path;
		return string.Empty;
	}

	public List<int> GetOtherAvail()
	{
		return otherAvailId;
	}

	public int GetLastId()
	{
		return lastId;
	}

	public void NextLastId()
	{
		lastId++;
	}

	public List<string> GetPaths()
	{
		List<string> paths = new List<string> ();
		if (parentImgs != null) 
		{
			foreach (ImgPersistanceInfo imgInfo in parentImgs.Values)
				paths.Add (imgInfo.path);
		}
		return paths;
	}

	public void AddParent(int i_id, ImgPersistanceInfo i_info)
	{
		parentImgs.Add (i_id, i_info);
	}

	public bool HasParent(int i_id)
	{
		return (parentImgs != null && parentImgs.ContainsKey (i_id));
	}

	public string GetParentPath(int i_id)
	{
		if (HasParent (i_id))
			return parentImgs [i_id].path;
		return string.Empty;
	}

	public int GetParent(int i_id)
	{
		foreach (int parent in parentImgs.Keys) 
		{
			if (parentImgs [parent].Contains (i_id))
				return parent;
		}
		return -1;
	}
}
