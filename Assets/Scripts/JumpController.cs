using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	[SerializeField]
	private CharacterMovementController pj;


	void OnTriggerEnter2D (Collider2D coll){
	
		pj.jumpAllowed = true;

	
	}

	void OnTriggerExit2D (Collider2D coll){

		pj.jumpAllowed = false;

	}
	



}
