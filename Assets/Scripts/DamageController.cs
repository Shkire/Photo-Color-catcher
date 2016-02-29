using UnityEngine;
using System.Collections;

/// <summary>
/// Character damage controller.
/// </summary>
public class DamageController : MonoBehaviour
{
	/// <summary>
	/// Number of damage points that this character do on each hit.
	/// </summary>
	[SerializeField]
	private float damage;

	//Change with stay
	void OnCollisionEnter2D (Collision2D coll)
	{
		if (coll.gameObject.GetComponent<CharacterHealthController> ()) {
			coll.gameObject.GetComponent<CharacterHealthController> ().DoDamage (damage);
		}
	}

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (coll.gameObject.GetComponent<CharacterHealthController> ()) {
			coll.gameObject.GetComponent<CharacterHealthController> ().DoDamage (damage);
		}
	}
}
