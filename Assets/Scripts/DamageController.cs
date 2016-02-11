using UnityEngine;
using System.Collections;

public class DamageController : MonoBehaviour {


	[SerializeField]
	private float damage;


	//Better Stay but we need a reset on pj
	void OnCollisionEnter2D(Collision2D coll) {

		if (coll.gameObject.GetComponent<CharacterHealthController> ()) {
		
			coll.gameObject.GetComponent<CharacterHealthController> ().DoDamage (damage);

		}

	}


}
