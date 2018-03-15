using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the enemy movement.
/// </summary>
public abstract class EnemyMovementController : CharacterMovementController
{

	/// <summary>
	/// Movement type enum declaration.
	/// </summary>
	protected enum MovementType
	{
		Undefined,
		Stay,
		Left,
		Right}
	;

	/// <summary>
	/// If the character has encountered a Pj.
	/// </summary>
	protected bool pjEncountered;

	/// <summary>
	/// The Pj that is objective of the character.
	/// </summary>
	protected GameObject pj;

	/// <summary>
	/// If the character is making team with another.
	/// </summary>
	protected bool hasTeam;

	/// <summary>
	/// Movement pattern of the character.
	/// </summary>
	protected void MovementPattern ()
	{
		if (!pjEncountered) {
			NormalMovement ();
		} else {
			PjEncounteredMovement ();
		}
	}

	/// <summary>
	/// Enemy movement pattern when moving normally.
	/// </summary>
	protected virtual void NormalMovement ()
	{
	}

	/// <summary>
	/// Enemy movement pattern when has encountered a Pj.
	/// </summary>
	protected virtual void PjEncounteredMovement ()
	{
	}

	/// <summary>
	/// Another character has joined this on team play.
	/// </summary>
	/// <param name="pj">Pj objective of these characters team play.</param>
	public virtual void TeamPlay (GameObject pj)
	{
		hasTeam = true;
		pjEncountered = true;
		this.pj = pj;
		SendMessage ("StopSearching", "Enemy");
	}

	/// <summary>
	/// This character notices that has encountered a Pj.
	/// </summary>
	/// <param name="pj">Pj encountered.</param>
	public void HasEncounteredPj (GameObject pj)
	{
		pjEncountered = true;
		this.pj = pj;
		SendMessage ("StopSearching", "Pj");
	}
}
