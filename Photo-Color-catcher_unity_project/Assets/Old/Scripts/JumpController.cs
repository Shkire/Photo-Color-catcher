using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls if the character can jump.
/// </summary>
[RequireComponent (typeof(CharacterController))]
public class JumpController : MonoBehaviour
{
	/// <summary>
	/// The list of floors the character is touching.
	/// </summary>
	private List<GameObject> collisionFloorList;

	/// <summary>
	/// If the character is jumping.
	/// </summary>
	private bool onJump;

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (!coll.collider.gameObject.layer.Equals (LayerMask.NameToLayer ("Ground")))
			return;
		if (coll.collider.bounds.max.y <= this.gameObject.GetComponent<Collider2D> ().bounds.min.y) {
			SendMessage ("DidLand");
			if (collisionFloorList == null)
				collisionFloorList = new List<GameObject> ();
			collisionFloorList.Add (coll.collider.gameObject);
			onJump = false;
		}
	}

	void OnCollisionExit2D (Collision2D coll)
	{
		if (!coll.collider.gameObject.layer.Equals (LayerMask.NameToLayer ("Ground")) || onJump)
			return;
		collisionFloorList.Remove (coll.collider.gameObject);
		if (collisionFloorList.Count == 0)
			SendMessage ("DidStartFalling");
	}

	/// <summary>
	/// Action whrn the character jumps
	/// </summary>
	void DidJump ()
	{
		onJump = true;
		collisionFloorList = new List<GameObject> ();
	}
}
