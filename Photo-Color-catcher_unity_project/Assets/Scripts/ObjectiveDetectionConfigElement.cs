using UnityEngine;
using System.Collections;

/// <summary>
/// Definition of a Config Element for ObjectiveDetection.
/// </summary>
[System.Serializable]
public class ObjectiveDetectionConfigElement
{
	/// <summary>
	/// The message which is going to be sent when the objective is detected (HasEnconuntered+message).
	/// </summary>
	public string message;

	/// <summary>
	/// The layer list where you are searching the object.
	/// </summary>
	public string[] layers;

	/// <summary>
	/// The tag you are searching by the object (if are you searching by tag).
	/// </summary>
	public string searchTag;

	/// <summary>
	/// If must keep searching this objective.
	/// </summary>
	private bool keepSearching = true;

	/// <summary>
	/// The layer code of the layer list.
	/// </summary>
	private int layerCode;

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectiveDetectionConfigElement"/> must keep searching this objective.
	/// </summary>
	/// <value><c>true</c> if must keep searching; otherwise, <c>false</c>.</value>
	public bool MustKeepSearching {
		get {
			return keepSearching;
		}
		set {
			keepSearching = value;
		}
	}

	/// <summary>
	/// Gets the filtering layer.
	/// </summary>
	/// <value>The filtering layer.</value>
	public int FilteringLayer {
		get {
			return layerCode;
		}
	}

	/// <summary>
	/// Sets the layer code.
	/// </summary>
	public void SetLayerCode ()
	{
		layerCode = LayerMask.GetMask (layers);
	}
}
