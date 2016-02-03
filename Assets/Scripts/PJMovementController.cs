using UnityEngine;
using System.Collections;

public class PJMovementController : CharacterMovementController {

	
	// Update is called once per frame
	void FixedUpdate () {
	
		float movement = Input.GetAxis ("Horizontal") * speed;
		Move (movement);
		float canJump = Input.GetAxis ("Vertical");
		if (canJump > 0)
			Jump ();
	}



}
