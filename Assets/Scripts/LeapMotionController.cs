using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Reads info from LeapMotion.
/// </summary>
public static class LeapMotionController
{
	/// <summary>
	/// The frame read from leapMotion.
	/// </summary>
	private static Frame leapFrame;

	/// <summary>
	/// The list of hands read.
	/// </summary>
	private static HandList hands;

	/// <summary>
	/// The first hand.
	/// </summary>
	private static Hand firstHand;

	/// <summary>
	/// The second hand.
	/// </summary>
	private static Hand secondHand;

	/// <summary>
	/// The actual value on y axis.
	/// </summary>
	private static float actual_y;

	/// <summary>
	/// The last value on y axis.
	/// </summary>
	private static float last_y;

	/// <summary>
	/// The actual value on z axis.
	/// </summary>
	private static float actual_z;

	/// <summary>
	/// The last value on z axis.
	/// </summary>
	private static float last_z;

	/// <summary>
	/// The left limit plane on LeapMotion space.
	/// </summary>
	private static int left;

	/// <summary>
	/// The right limit plane on LeapMotion space.
	/// </summary>
	private static int right;

	/// <summary>
	/// The top limit plane on LeapMotion space.
	/// </summary>
	private static int top;

	/// <summary>
	/// The bottom limit plane on LeapMotion space.
	/// </summary>
	private static int bottom;

	/// <summary>
	/// The back limit plane on LeapMotion space.
	/// </summary>
	private static int back;

	/// <summary>
	/// The front limit plane on LeapMotion space.
	/// </summary>
	private static int front;

	/// <summary>
	/// The controller.
	/// </summary>
	private static Controller controller;

	/// <summary>
	/// Creates the controller and configure it.
	/// </summary>
	/// <param name="left_i">Left plane value.</param>
	/// <param name="right_i">Right plane value.</param>
	/// <param name="bottom_i">Bottom plane value.</param>
	/// <param name="top_i">Top plane value.</param>
	public static void CreateController (int left_i, int right_i, int bottom_i, int top_i)
	{
		if (controller == null)
			controller = new Controller ();

		left = left_i;
		right = right_i;
		bottom = bottom_i;
		top = top_i;
	}

	/// <summary>
	/// Creates the controller and configure it.
	/// </summary>
	/// <param name="front_i">Front plane value.</param>
	/// <param name="back_i">Back plane value.</param>
	public static void CreateController (int front_i, int back_i)
	{
		if (controller == null)
			controller = new Controller ();

		back = back_i;
		front = front_i;
	}

	/// <summary>
	/// Updates the frame.
	/// </summary>
	public static void UpdateFrames ()
	{
		leapFrame = controller.Frame ();
		hands = leapFrame.Hands;
	}

	/// <summary>
	/// Updates the value of y axis.
	/// </summary>
	public static void UpdateY ()
	{
		firstHand = hands [0];
		last_y = actual_y;
		actual_y = firstHand.PalmPosition.y;
	}

	/// <summary>
	/// Updates the value of z axis.
	/// </summary>
	public static void UpdateZ ()
	{
		secondHand = hands [1];
		last_z = actual_z;
		actual_z = secondHand.PalmPosition.z;
	}

	/// <summary>
	/// Checks if there is more than one hand.
	/// </summary>
	/// <returns><c>true</c>, if there is more than one hand, <c>false</c> otherwise.</returns>
	public static bool MoreThanOneHand ()
	{
		return hands.Count >= 1;
	}

	/// <summary>
	/// Checks if player is moving left.
	/// </summary>
	public static bool Left ()
	{
		return firstHand.PalmPosition.x < left;
	}

	/// <summary>
	/// Checks if player is moving right.
	/// </summary>
	public static bool Right ()
	{
		return firstHand.PalmPosition.x > right;
	}

	/// <summary>
	/// Checks if value on y axis is increasing.
	/// </summary>
	/// <returns><c>true</c>, if y has increased, <c>false</c> otherwise.</returns>
	public static bool Range_y ()
	{
		return last_y < actual_y;
	}

	/// <summary>
	/// Checks if value on z axis is increasing.
	/// </summary>
	/// <returns><c>true</c>, if z has increased, <c>false</c> otherwise.</returns>
	public static bool Range_z ()
	{
		return last_z < actual_z;
	}

	/// <summary>
	/// Checks if player has passed the bottom plane.
	/// </summary>
	public static bool Bottom ()
	{
		return firstHand.PalmPosition.y > bottom;
	}

	/// <summary>
	/// Checks if player has passed the top plane.
	/// </summary>
	public static bool Top ()
	{
		return firstHand.PalmPosition.y < top;
	}

	/// <summary>
	/// Prepares the photo frame.
	/// </summary>
	/// <returns><c>true</c>, if player has prepared the photo frame, <c>false</c> otherwise.</returns>
	public static bool PreparePhoto ()
	{
		return secondHand.PalmPosition.z < back && secondHand.PalmPosition.z > front;
	}

	/// <summary>
	/// Checks if the player has taken a photo.
	/// </summary>
	public static bool Photo ()
	{
		return secondHand.PalmPosition.z > back && MoreThanOneHand ();
	}
}

