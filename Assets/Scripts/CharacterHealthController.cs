using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Controls a character's (Pj or Enemy) health.
/// </summary>
[RequireComponent (typeof(CharacterMovementController))]
public class CharacterHealthController : MonoBehaviour
{

	/// <summary>
	/// Number of HP.
	/// </summary>
	[SerializeField]
	private float health;

	void FixedUpdate ()
	{


	}

	/// <summary>
	/// Does the points of damage specified.
	/// </summary>
	/// <param name="damage">Damage points.</param>
	public void DoDamage (float damage)
	{
			health -= damage;
			PjMovementController pj = this.gameObject.GetComponent <PjMovementController>();
			if (pj != null)
				pj.TriggerDamage ();
		if (health <= 0)
			DestroyCharacter ();
	}

	void DestroyCharacter()
	{
		Destroy (this.gameObject);
		PjMovementController pj = this.gameObject.GetComponent <PjMovementController>();
		if (pj != null)
			Application.Quit ();
	}
}
