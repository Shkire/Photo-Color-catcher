using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the Blue enemy movement.
/// </summary>
public class BlueEnemyMovementController : EnemyMovementController
{

	/// <summary>
	/// The maximum response time.
	/// </summary>
	[SerializeField]
	private float maxResponseTime;

	/// <summary>
	/// The minimum response time.
	/// </summary>
	[SerializeField]
	private float minResponseTime;

	/// <summary>
	/// The left response time.
	/// </summary>
	private float leftResponseTime;

	/// <summary>
	/// Which type of movement has this enemy now when moving normally.
	/// </summary>
	private movementType nowMovingTo;

	/// <summary>
	/// The randomizer.
	/// </summary>
	private System.Random rnd;

	void OnEnable ()
	{
		rnd = new System.Random (Guid.NewGuid ().GetHashCode ());
	}

	void FixedUpdate ()
	{
		if (!pjEncountered) {
			leftResponseTime -= Time.fixedDeltaTime;
			if (leftResponseTime <= 0) {
				leftResponseTime = (float)(rnd.NextDouble () * (maxResponseTime - minResponseTime)) + minResponseTime;
				nowMovingTo = movementType.Undefined;
			}
		}

		MovementPattern ();

	}

	/// <summary>
	/// Enemy movement pattern when moving normally.
	/// </summary>
	protected override void NormalMovement ()
	{
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

	/// <summary>
	/// Enemy movement pattern when has encountered a Pj.
	/// </summary>
	protected override void PjEncounteredMovement ()
	{
		float pos = GameObject.FindGameObjectWithTag ("Player").transform.position.x;

		if (pos > this.transform.position.x)
			Move (1);
		if (pos < this.transform.position.x)
			Move (-1);
	}
		
}
