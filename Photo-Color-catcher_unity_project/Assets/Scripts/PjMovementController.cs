using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using UnityEngine.UI;

/// <summary>
/// Controls the Pj movement.
/// </summary>
public class PjMovementController : CharacterMovementController
{

	[SerializeField]
	private UnityEngine.UI.Image camera1;
	[SerializeField]
	private UnityEngine.UI.Image camera2;
	[SerializeField]
	private UnityEngine.UI.Image camera3;


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

	}
		
	void FixedUpdate ()
	{


		bool andando = false;

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


	public void TriggerDamage (float health){

		Debug.Log (health);

		if (health == 2.0)
			camera3.enabled = false;
		if (health == 1.0)
			camera2.enabled = false;
		if (health == 0)
			camera1.enabled = false;


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