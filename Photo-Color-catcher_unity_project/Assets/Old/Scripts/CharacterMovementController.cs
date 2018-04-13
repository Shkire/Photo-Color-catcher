using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Character movement controller.
/// </summary>
public abstract class CharacterMovementController : MonoBehaviour
{
	/// <summary>
	/// Character's speed.
	/// </summary>
	[SerializeField]
	protected float speed;

	/// <summary>
	/// Character's jump magnitude.
	/// </summary>
	[SerializeField]
	protected float jumpMagnitude;

	/// <summary>
	/// If the character is on floor.
	/// </summary>
	protected bool onFloor = false;

	/// <summary>
	/// Move the character.
	/// </summary>
	/// <param name="direction">Magnitude and direction of the movement (+Right/-Left).</param>
	protected void Move (float direction)
	{
		//Character's rotation (if necessary)
		//y=0 -> (0,0,0,1)
		//y=180 -> (0,-1,0,0)
		if (direction > 0) {
			if (Quaternion.Angle (gameObject.transform.rotation, new Quaternion (0f, -1f, 0f, 0f)) == 0) { 
				gameObject.transform.rotation = new Quaternion (0f, 0f, 0f, 1f);
			}
		} else if (direction < 0) {
			if (Quaternion.Angle (gameObject.transform.rotation, new Quaternion (0f, 0f, 0f, 1f)) == 0) { 
				gameObject.transform.rotation = new Quaternion (0f, -1f, 0f, 0f);
			}
		}

		//Movement
		this.gameObject.transform.Translate (new Vector3 (Math.Abs (direction) * speed, 0, 0));
	}

	/// <summary>
	/// The character jumps.
	/// </summary>
	protected void Jump ()
	{
        /*
		if (onFloor)
			this.gameObject.GetComponent<Rigidbody2D> ().ResetAndAddForce (Vector2.up * jumpMagnitude);
		onFloor = false;

		SendMessage ("DidJump");	
  */      
	}

	/// <summary>
	/// Action when the character lands.
	/// </summary>
	public void DidLand ()
	{
		onFloor = true;
	}

	/// <summary>
	/// Action when the character start falling.
	/// </summary>
	public void DidStartFalling ()
	{
		onFloor = false;
	}
}
