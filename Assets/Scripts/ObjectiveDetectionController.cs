using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterMovementController))]
public class ObjectiveDetectionController : MonoBehaviour
{

	[SerializeField]
	private ObjectiveDetectionConfigElement[] config;

	[SerializeField]
	private float sightRange;

	void OnEnable ()
	{
		foreach (ObjectiveDetectionConfigElement configElem in config)
		{
			configElem.SetLayerCode ();
		}
			
	}

	void FixedUpdate ()
	{

		foreach (ObjectiveDetectionConfigElement configElem in config) {
			if (configElem.MustKeepSearching) {
				RaycastHit2D[] hits = Physics2D.RaycastAll (this.gameObject.transform.position, Vector2.right, sightRange, configElem.FilteringLayer);
				foreach (RaycastHit2D hit in hits) {
					Debug.Log (hit.collider.gameObject);
					if ((configElem.searchTag.Equals (string.Empty) || (hit.collider.gameObject.tag.Equals (configElem.searchTag))) && !hit.collider.gameObject.Equals(this.gameObject)) {
						SendMessage ("HasEncountered" + configElem.message, hit.collider.gameObject);
						break;
					}
				}
			}
		}
				
	}

	public void StopSearching(string message)
	{
		foreach (ObjectiveDetectionConfigElement configElem in config) {
			if (configElem.message.Equals(message)) {
				configElem.MustKeepSearching = false;
				break;
			}
		}
	}
}
	
