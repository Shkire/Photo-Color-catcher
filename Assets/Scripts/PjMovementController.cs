using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Controls the Pj movement.
/// </summary>
public class PjMovementController : CharacterMovementController
{
	/// <summary>
	/// The LeapMotion left plane value.
	/// </summary>
	[SerializeField]
	int left;

	/// <summary>
	/// The LeapMotion right plane value.
	/// </summary>
	[SerializeField]
	int right;

	/// <summary>
	/// The LeapMotion top plane value.
	/// </summary>
	[SerializeField]
	int top;

	/// <summary>
	/// The LeapMotion bottom plane value.
	/// </summary>
	[SerializeField]
	int bottom;

	void OnEnable()
	{
		LeapMotionController.CreateController (left,right,bottom,top);
	}
		
	void FixedUpdate ()
	{
		LeapMotionController.UpdateFrames ();
		LeapMotionController.UpdateY ();

		if (LeapMotionController.MoreThanOneHand())
		{
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