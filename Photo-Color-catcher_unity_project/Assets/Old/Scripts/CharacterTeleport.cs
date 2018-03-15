using UnityEngine;
using System.Collections;

public class CharacterTeleport : MonoBehaviour {

	[SerializeField]
	private CharacterTeleport other;

	void OnTriggerEnter2D (Collider2D coll)
	{
		if (other != null)
			coll.transform.position = other.transform.position;
	}
}
