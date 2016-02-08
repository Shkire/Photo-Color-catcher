using UnityEngine;
using System.Collections;
using System;

public class RedEnemyMovementController : EnemyMovementController {

	[SerializeField]
	private float maxResponseTime;

	[SerializeField]
	private float minResponseTime;

	[SerializeField]
	private GameObject pJ;

	private float leftResponseTime;

	private movementType nowMovingTo;

	private System.Random rnd;




	void OnEnable(){
		rnd = new System.Random (Guid.NewGuid ().GetHashCode ());
	}




	void FixedUpdate(){

		NormalMovement ();

	}

	protected override void NormalMovement(){

		float pos = pJ.transform.position.x;

		if (pos > this.transform.position.x)
			Move (1);
		if (pos < this.transform.position.x)
			Move (-1);

	}






}
