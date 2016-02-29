using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the Green enemy movement.
/// </summary>
public class GreenEnemyMovementController : EnemyMovementController
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
	/// The enemy that has been encountered.
	/// </summary>
	private GameObject enemyEncountered;

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
		if (enemyEncountered == null) {
			if (!hasTeam) {
				float dif = this.gameObject.transform.position.x - pj.transform.position.x;
				if (dif < 0)
					Move (-1);
				else if (dif > 0)
					Move (1);
			} else {
				float dif = this.gameObject.transform.position.x - pj.transform.position.x;
				if (dif < 0)
					Move (1);
				else if (dif > 0)
					Move (-1);
			}
		} else {
			float dif = this.gameObject.transform.position.x - enemyEncountered.transform.position.x;
			if (dif < 0)
				Move (1);
			else if (dif > 0)
				Move (-1);
		}
	}

	/// <summary>
	/// This character notices that has encountered a Pj.
	/// </summary>
	/// <param name="pj">Pj encountered.</param>
	public void HasEncounteredEnemy (GameObject enemy)
	{
		enemyEncountered = enemy;
		enemyEncountered.GetComponent<EnemyMovementController> ().TeamPlay (pj);
		SendMessage ("StopSearching", "Enemy");
	}
}