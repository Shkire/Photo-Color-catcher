using UnityEngine;
using System.Collections;

public abstract class CharacterMovementController : MonoBehaviour {

	[SerializeField]
	protected float speed;

	[SerializeField]
	protected float jump;

	protected bool onFloor = false;

	protected void Move(float direction){

		//Movimiento izq der

		this.gameObject.transform.Translate(new Vector3(direction*speed,0,0));



		/*
		 * 
		 * Valorar entre Flip Sprite o Flip Gameobject
		 * 
		 * 
		 * */
		//mirada del pj
		if (direction > 0)
			this.gameObject.GetComponent<SpriteRenderer> ().flipX = true;
			//this.transform.rotation = new Quaternion (0,0,0,1.0f);

		if (direction < 0)
			this.gameObject.GetComponent<SpriteRenderer> ().flipX = false;
			//this.transform.rotation = new Quaternion (0,-1.0f,0,0);
		


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
