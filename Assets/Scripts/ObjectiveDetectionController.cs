using UnityEngine;
using System.Collections;

public class ObjectiveDetectionController : MonoBehaviour {

	[SerializeField]
	private float sightRange;

	[SerializeField]
	private string tag;

	void FixedUpdate(){

		RaycastHit2D[] hits = Physics2D.Raycast (this.gameObject.transform.position, Vector2.right, sightRange);


	}

}
