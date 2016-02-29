using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the objective detection of a character.
/// </summary>
[RequireComponent (typeof(CharacterMovementController))]
public class ObjectiveDetectionController : MonoBehaviour
{
	/// <summary>
	/// The list of Config Elements.
	/// </summary>
	[SerializeField]
	private ObjectiveDetectionConfigElement[] config;

	/// <summary>
	/// The sight range of the character.
	/// </summary>
	[SerializeField]
	private float sightRange;

	void OnEnable ()
	{
		foreach (ObjectiveDetectionConfigElement configElem in config) {
			configElem.SetLayerCode ();
		}
	}

	void FixedUpdate ()
	{
		foreach (ObjectiveDetectionConfigElement configElem in config) {
			if (configElem.MustKeepSearching) {
				Vector2 vectorAux = Vector2.right;

				if (Quaternion.Angle (gameObject.transform.rotation, new Quaternion (0f, -1f, 0f, 0f)) == 0)
					vectorAux = Vector2.left;
		
				RaycastHit2D[] hits = Physics2D.RaycastAll (this.gameObject.transform.position, vectorAux, sightRange, configElem.FilteringLayer);
				
				foreach (RaycastHit2D hit in hits) {
					if ((configElem.searchTag.Equals (string.Empty) || (hit.collider.gameObject.tag.Equals (configElem.searchTag))) && !hit.collider.gameObject.Equals (this.gameObject)) {
						SendMessage ("HasEncountered" + configElem.message, hit.collider.gameObject);
						break;
					}
				}
			}
		}			
	}

	/// <summary>
	/// Stops searching the message objective.
	/// </summary>
	/// <param name="message">Objective that must stop searching.</param>
	public void StopSearching (string message)
	{
		foreach (ObjectiveDetectionConfigElement configElem in config) {
			if (configElem.message.Equals (message)) {
				configElem.MustKeepSearching = false;
				break;
			}
		}
	}
}