using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the Red enemy movement.
/// </summary>
public class RedEnemyMovementController : EnemyMovementController
{
	void FixedUpdate ()
	{
		NormalMovement ();
	}

	protected override void NormalMovement ()
	{
		float pos = GameObject.FindGameObjectWithTag ("Player").transform.position.x;

		if (pos > this.transform.position.x)
			Move (1);
		if (pos < this.transform.position.x)
			Move (-1);
	}
}