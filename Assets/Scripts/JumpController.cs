using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	[SerializeField]
	private PJMovementController pj;


	void OnTriggerEnter2D (Collider2D coll){
	
		pj.jumpAllowed = true;

	
	}

	void OnTriggerExit2D (Collider2D coll){

		pj.jumpAllowed = false;

	}
	



}
