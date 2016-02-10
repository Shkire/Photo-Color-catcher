using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpController : MonoBehaviour
{

	private List<GameObject> collisionFloorList;

	private bool onJump;

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (!coll.collider.gameObject.layer.Equals (LayerMask.NameToLayer ("Ground")))
			return;
		if (coll.collider.bounds.max.y <= this.gameObject.GetComponent<Collider2D> ().bounds.min.y) 
		{
			//Estas encima
			SendMessage ("DidLand");
			if (collisionFloorList == null)
				collisionFloorList = new List<GameObject> ();
			collisionFloorList.Add (coll.collider.gameObject);
			onJump = false;
		}

		/*
		if (((this.gameObject.GetComponent<Collider2D> ().bounds.max.x >= coll.collider.bounds.min.x) && (this.gameObject.GetComponent<Collider2D> ().bounds.max.x <= coll.collider.bounds.max.x)) || ((this.gameObject.GetComponent<Collider2D> ().bounds.min.x >= coll.collider.bounds.min.x) && (this.gameObject.GetComponent<Collider2D> ().bounds.min.x <= coll.collider.bounds.max.x)))
			Debug.Log ("Estás dentro");
		else
			Debug.Log ("No estás dentro");
		*/
	}

	void OnCollisionExit2D (Collision2D coll)
	{
		if (!coll.collider.gameObject.layer.Equals (LayerMask.NameToLayer ("Ground")) || onJump)
			return;
		collisionFloorList.Remove (coll.collider.gameObject);
		if (collisionFloorList.Count == 0)
			//Estas cayendo
			SendMessage ("DidStartFalling");
	}

	void DidJump()
	{
		onJump = true;
		collisionFloorList = new List<GameObject> ();
	}

}
