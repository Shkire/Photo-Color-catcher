using UnityEngine;
using System.Collections;

public class PJMovementController : CharacterMovementController {

	
	// Update is called once per frame
	void FixedUpdate () {
	
		Move ();
		Jump ();
	}



}
