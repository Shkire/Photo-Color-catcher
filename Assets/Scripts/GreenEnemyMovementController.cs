using UnityEngine;
using System.Collections;
using System;

public class GreenEnemyMovementController : EnemyMovementController {

	[SerializeField]
	private float maxResponseTime;

	[SerializeField]
	private float minResponseTime;

	private float leftResponseTime;

	private movementType nowMovingTo;

	private System.Random rnd;

	void OnEnable(){
		rnd = new System.Random (Guid.NewGuid ().GetHashCode ());
	}

	void FixedUpdate(){

		if (!pjEnconuntered) {
			leftResponseTime -= Time.fixedDeltaTime;
			if (leftResponseTime <= 0) {
				leftResponseTime = (float)(rnd.NextDouble () * (maxResponseTime - minResponseTime)) + minResponseTime;
				nowMovingTo = movementType.Undefined;
			}
		}

		MovementPattern ();

	}

	protected override void NormalMovement(){

		if (nowMovingTo.Equals (movementType.Undefined)) {
			int rndMovement = rnd.Next (1, 4);
			switch (rndMovement) {
			case 1:
				nowMovingTo = movementType.Left;
				break;
			case 2:
				nowMovingTo = movementType.Right;
				break;
			case 3:
				nowMovingTo = movementType.Stay;
				break;
			}
		}

		switch (nowMovingTo) {
		case movementType.Left:
			Move (-1);
			break;
		case movementType.Right:
			Move (1);
			break;
		case movementType.Stay:
			nowMovingTo = movementType.Stay;
			break;
		}
		
	}

	protected override void PjEncounteredMovement(){
		float dif = this.gameObject.transform.position.x-GameObject.FindGameObjectWithTag ("Player").transform.position.x;
		if (dif < 0)
			Move (-1);
		else if (dif > 0)
			Move (1);
	}

}