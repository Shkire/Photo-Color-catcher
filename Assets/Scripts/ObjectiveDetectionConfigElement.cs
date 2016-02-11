using UnityEngine;
using System.Collections;

[System.Serializable]
public class ObjectiveDetectionConfigElement {

	public string message;

	public string[] layers;

	public string searchTag;

	private bool keepSearching = true;

	private int layerCode;

	public bool MustKeepSearching
	{
		get
		{
			return keepSearching;
		}
		set
		{
			keepSearching = value;
		}
	}

	public int FilteringLayer
	{
		get
		{
			return layerCode;
		}
	}

	public void SetLayerCode()
	{
		layerCode = LayerMask.GetMask (layers);
	}

}
