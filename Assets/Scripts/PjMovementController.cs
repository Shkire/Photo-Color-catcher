using UnityEngine;
using System.Collections;
using Leap;
using System.IO.Ports;
using System;


/// <summary>
/// Controls the Pj movement.
/// </summary>
public class PjMovementController : CharacterMovementController
{

	/// <summary>
	/// .
	/// </summary>
	[SerializeField]
	Animator animation;

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


	SerialPort stream = new SerialPort("COM5", 9600); //Set the port (com4) and the baud rate (9600, is standard on most devices)
	string lineRead;
	int x;
	int y;

	void Start () {
		stream.Open(); //Open the Serial Stream.
		stream.ReadTimeout = 1;

	}

	void OnEnable()
	{
		LeapMotionController.CreateController (left,right,bottom,top);
	}
		
	void FixedUpdate ()
	{


		bool andando = false;

		LeapMotionController.UpdateFrames ();
		LeapMotionController.UpdateY ();

		if (LeapMotionController.MoreThanOneHand())
		{
			if (LeapMotionController.Right ()) {
				Move (1);
				andando = true;
			}
			if (LeapMotionController.Left ()) {
				Move (-1);
				andando = true;
			}
			if (LeapMotionController.Bottom() && LeapMotionController.Top() && LeapMotionController.Range_y() )
				Jump ();		
		}
	
		float movement = Input.GetAxis ("Horizontal");
		Move (movement);
		if (movement != 0) {
			andando = true;
		}
		float canJump = Input.GetAxis ("Vertical");
		if (canJump > 0)
			Jump ();

		//joystick
		if (stream.IsOpen) {

			try{

				lineRead = stream.ReadLine();
				dataCast(lineRead);

				if(y < 514)
					Move(1);
				if(y > 514)
					Move(-1);

			}catch (System.Exception){

			}
		}

		animation.SetBool ("Andando", andando);

	}


	void dataCast (string data){

		string[] dataArray = data.Split ('|');

		x = Convert.ToInt32(dataArray[0]);
		y = Convert.ToInt32(dataArray[1]);
			
	}


	public void TriggerDamage (){

		animation.SetTrigger ("Golpeado");
		StartCoroutine ("Invulnerable");
	}


	IEnumerator Invulnerable()
	{
		this.gameObject.layer = LayerMask.NameToLayer ("Invulnerable");
		yield return new WaitForSeconds (2f);
		this.gameObject.layer = LayerMask.NameToLayer ("Player");
	}

}