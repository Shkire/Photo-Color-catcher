using UnityEngine;
using System.Collections;

public class PjDamageController : DamageController {

	[SerializeField]
	private ScoreController score;

	// Use this for initialization
	void Start()
	{
		GameObject scoreGo = GameObject.FindGameObjectWithTag ("Score");
		if (scoreGo != null)
			score = scoreGo.GetComponent<ScoreController> ();
	}

	protected override void OnCollisionEnter2D (Collision2D coll)
	{
		base.OnCollisionEnter2D (coll);
		if (coll.gameObject.GetComponent<CharacterHealthController> ()) {
			if (score != null)
				score.GetComponent<ScoreController> ().Score ();
		}
	}

	protected override void OnTriggerEnter2D (Collider2D coll)
	{
		base.OnTriggerEnter2D (coll);
		if (coll.gameObject.GetComponent<CharacterHealthController> ()) {
			if (score != null)
				score.GetComponent<ScoreController> ().Score ();
		}
	}
		
}
