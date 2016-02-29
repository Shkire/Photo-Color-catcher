using UnityEngine;
using System.Collections;
using Leap;


public static class LeapMotionController{


	private static Frame leapFrame;
	private static HandList hands;
	private static Hand firstHand;
	private static Hand secondHand;

	private static float actual_y;
	private static float anterior_y;

	private static float actual_z;
	private static float anterior_z;

	private static int left;
	private static int right;
	private static int top;
	private static int bottom;
	private static int back;
	private static int front;

	private static Controller controller;

	public static void CreateController(int left_i, int right_i, int bottom_i, int top_i){
	
		if(controller == null)
			controller = new Controller ();

		left = left_i;
		right = right_i;
		bottom = bottom_i;
		top = top_i;


	}

	public static void CreateController(int front_i, int back_i){

		if(controller == null)
			controller = new Controller ();

		back = back_i;
		front = front_i;


	}

	public static void UpdateFrames(){

		leapFrame = controller.Frame ();
		hands = leapFrame.Hands;



	}

	public static void UpdateY(){
		
		firstHand = hands [0];
		anterior_y = actual_y;
		actual_y = firstHand.PalmPosition.y;


	}

	public static void UpdateZ(){
		secondHand = hands[1];
		anterior_z = actual_z;
		actual_z = secondHand.PalmPosition.z;

	}



	public static bool MoreThanOneHand(){
		return hands.Count >= 1;
	}

	public static bool Left(){
		return firstHand.PalmPosition.x < left;
	}
	 
	public static bool Right(){
		return firstHand.PalmPosition.x > right;
	}

	public static bool Range_y(){
		return anterior_y < actual_y;
	}

	public static bool Range_z(){
		return anterior_z < actual_z;
	}

	public static bool Bottom(){
		return firstHand.PalmPosition.y > bottom;
	}

	public static bool Top(){
		return firstHand.PalmPosition.y < top;
	}

	public static bool PreparePhoto(){
		return secondHand.PalmPosition.z<back  && secondHand.PalmPosition.z > front;
	}

	public static bool Photo(){
		return secondHand.PalmPosition.z > back && MoreThanOneHand();
	}
}

