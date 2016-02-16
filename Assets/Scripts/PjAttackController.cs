using UnityEngine;
using System.Collections;

public class PjAttackController : MonoBehaviour {

	[SerializeField]
	private GameObject frame;

	private bool active;

	// Update is called once per frame
	void FixedUpdate () {

		if (active) {
			frame.GetComponent<Collider2D> ().enabled = false;
			active = false;
		}
		

		if (Input.GetKeyDown (KeyCode.Space)) {
			frame.GetComponent<SpriteRenderer> ().enabled = true;

		}


		if (Input.GetKeyUp (KeyCode.Space)) {
			frame.GetComponent<Collider2D> ().enabled = true;
			frame.GetComponent<SpriteRenderer> ().enabled = false;
			active = true;
		}



	}





}
