using UnityEngine;
using System.Collections;
using Leap;

public class PjMovementController : CharacterMovementController {

	[SerializeField]
	int left;

	[SerializeField]
	int right;

	[SerializeField]
	int top;

	[SerializeField]
	int bottom;

	void OnEnable(){
		LeapMotionController.CreateController (left,right,bottom,top);
	}


	
	// Update is called once per frame
	void FixedUpdate () {


	

		LeapMotionController.UpdateFrames ();
		LeapMotionController.UpdateY ();

		//Debug.Log (LeapMotionController.Range_y());

		if (LeapMotionController.MoreThanOneHand()) {
			

			if (LeapMotionController.Right())
				Move (1);

			if (LeapMotionController.Left())
				Move (-1);

			if (LeapMotionController.Bottom() && LeapMotionController.Top() && LeapMotionController.Range_y() )
				Jump ();
				
		}
	


	

		float movement = Input.GetAxis ("Horizontal");
		Move (movement);
		float canJump = Input.GetAxis ("Vertical");
		if (canJump > 0)
			Jump ();
	}



}
