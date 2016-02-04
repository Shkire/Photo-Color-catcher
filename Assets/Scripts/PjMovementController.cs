using UnityEngine;
using System.Collections;

public class PjMovementController : CharacterMovementController {

	
	// Update is called once per frame
	void FixedUpdate () {
	
		float movement = Input.GetAxis ("Horizontal");
		Move (movement);
		float canJump = Input.GetAxis ("Vertical");
		if (canJump > 0)
			Jump ();
	}



}
