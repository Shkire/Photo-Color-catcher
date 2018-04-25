using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChildImgInfo
{
	private Vector2 p_pos;
	private ProcessedImage p_img;

	public ChildImgInfo (Vector2 i_pos, ProcessedImage i_img)
	{
		p_pos = i_pos;
		p_img = i_img;
	}

	public ProcessedImage img
	{
		get
		{
			return p_img;
		}
	}

	public Vector2 pos
	{
		get
		{ 
			return p_pos;
		}
	}
}


[System.Serializable]
public class PersistentImageData
{
	private int parentId;
	private string p_path;
	private Dictionary<int,ChildImgInfo> children;
	private Dictionary<int,ProcessedImageData> childrenData;

	public string path
	{
		get
		{
			return p_path;
		}
	}

	public PersistentImageData(ProcessedImage i_img, List<ProcessedImage> i_children, string i_path)
	{
		parentId = i_img.GetId ();
		p_path = i_path;
		children = new Dictionary<int, ChildImgInfo> ();
		childrenData = new Dictionary<int, ProcessedImageData> ();
		foreach (Vector2 pos in i_img.GetChildrenPos()) 
		{
			int id = i_img.GetChildId ((int)pos.x,(int)pos.y);
			ChildImgInfo info = null;
			foreach (ProcessedImage childImg in i_children) 
			{
				if (childImg.GetId() == id)
				{
					info = new ChildImgInfo (pos, childImg);
				}
			}
			children.Add (id,info);
		}
	}

	public void AddData(Dictionary<int,ProcessedImageData> i_imgData)
	{
		childrenData = i_imgData;
	}

	public bool ContainsChild(int i_id)
	{
		return (children != null && children.ContainsKey(i_id));
	}

	public ProcessedImage GetChild(int i_id)
	{
		if (ContainsChild (i_id))
			return children [i_id].img;
		return null;
	}

	public List<ChildImgInfo> GetChildrenInfo()
	{
		List<ChildImgInfo> childrenInfo = new List<ChildImgInfo> (children.Values);
		return childrenInfo;
	}
}
