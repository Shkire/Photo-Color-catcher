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

	/// <summary>
	/// Minimum time between damage detections.
	/// </summary>
	[SerializeField]
	private float responseTime;


	[SerializeField]
	private Image gameOver;
	[SerializeField]
	private Image hP1;
	[SerializeField]
	private Image hP2;
	[SerializeField]
	private Image hP3;


	/// <summary>
	/// TEMPORARY.
	/// </summary>
	private float blinkResponseTime = 1;

	/// <summary>
	/// The left response time until the next possible damage detection.
	/// </summary>
	private float leftResponseTime;

	/// <summary>
	/// TEMPORARY.
	/// </summary>
	private float blinkLeftResponseTime;

	/// <summary>
	/// If the character has recieved damage.
	/// </summary>
	private bool onDamage;

	void FixedUpdate ()
	{

		if (onDamage) {
			leftResponseTime -= Time.fixedDeltaTime;
			if (leftResponseTime <= 0) {
				leftResponseTime = responseTime;
				onDamage = false;
			}

			//animar
			if (leftResponseTime > 0 && leftResponseTime <= 1) {
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
			if (leftResponseTime > 1 && leftResponseTime <= 2) {
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}
			if (leftResponseTime > 2 && leftResponseTime < 3) {
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
			if (leftResponseTime >= 3) {
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}
		}



		if (health < 2 && health >= 1) {
			if(hP3 != null)
				hP3.gameObject.SetActive(false);

		}else

		if (health < 1 && health >= 0) {
				if(hP2 != null)
					hP2.gameObject.SetActive(false);
		}else if (health <= 0.0) {
				Destroy (gameObject);

				if (gameOver != null) {
					hP1.gameObject.SetActive(false);
					gameOver.gameObject.SetActive (true);
					Time.timeScale = 0;
				}

		}
	}

	/// <summary>
	/// Does the points of damage specified.
	/// </summary>
	/// <param name="damage">Damage points.</param>
	public void DoDamage (float damage)
	{
		if (!onDamage) {
			health -= damage;
			onDamage = true;
		}
	}
}
