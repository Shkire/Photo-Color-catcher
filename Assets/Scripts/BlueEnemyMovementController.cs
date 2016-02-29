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
	private MovementType nowMovingTo;

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
				nowMovingTo = MovementType.Undefined;
			}
		}

		MovementPattern ();

	}

	/// <summary>
	/// Enemy movement pattern when moving normally.
	/// </summary>
	protected override void NormalMovement ()
	{
		if (nowMovingTo.Equals (MovementType.Undefined)) {
			int rndMovement = rnd.Next (1, 4);
			switch (rndMovement) {
			case 1:
				nowMovingTo = MovementType.Left;
				break;
			case 2:
				nowMovingTo = MovementType.Right;
				break;
			case 3:
				nowMovingTo = MovementType.Stay;
				break;
			}
		}

		switch (nowMovingTo) {
		case MovementType.Left:
			Move (-1);
			break;
		case MovementType.Right:
			Move (1);
			break;
		case MovementType.Stay:
			nowMovingTo = MovementType.Stay;
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
