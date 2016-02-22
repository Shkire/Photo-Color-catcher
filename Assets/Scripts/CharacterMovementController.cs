using UnityEngine;
using System.Collections;
using System;

public abstract class CharacterMovementController : MonoBehaviour {

	[SerializeField]
	protected float speed;

	[SerializeField]
	protected float jump;

	protected bool onFloor = false;




	protected void Move(float direction){

		//Movimiento izq der
		this.gameObject.transform.Translate(new Vector3(Math.Abs(direction)*speed,0,0));

		/*
		 * 
		 * if y == 0 then 0,0 0,0 0,0 1,0
		 * 
		 * if y == 180 then 0,0 -1,0 0,0 0,0
		 * 
		 * 
		 * */

		//mirada del pj
		if (direction > 0) {
			if ( Quaternion.Angle(gameObject.transform.rotation, new Quaternion(0f,-1f,0f,0f)) == 0 ) { 
				gameObject.transform.rotation =  new Quaternion (0f,0f,0f,1f);
			}
		}

		if (direction < 0) {
			if (Quaternion.Angle (gameObject.transform.rotation, new Quaternion (0f, 0f, 0f, 1f)) == 0) { 
				gameObject.transform.rotation = new Quaternion (0f, -1f, 0f, 0f);
			}
		}
			

	}


	protected void Jump(){
		
		//Salto

		if (onFloor)
			this.gameObject.GetComponent<Rigidbody2D> ().ResetAndAddForce (Vector2.up * jump);

		onFloor = false;

		SendMessage ("DidJump");
		
	}

	public void DidLand(){
		//Aterriza
		onFloor = true;
	}

	public void DidStartFalling(){
		//Cae
		onFloor = false;
	}

}
