using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (CharacterMovementController))]
public class CharacterHealthController : MonoBehaviour {

	[SerializeField]
	private float health;

	[SerializeField]
	private float responseTime;

	private float blinkResponseTime = 1;

	private float leftResponseTime;
	private float blinkLeftResponseTime;


	private bool onDamage;





	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {



		if (onDamage) {
			leftResponseTime -= Time.fixedDeltaTime;
			if (leftResponseTime <= 0) {
				leftResponseTime = responseTime;
				onDamage = false;
			}

			//animar
			if(leftResponseTime >0 && leftResponseTime <=1){
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
				if(leftResponseTime >1 && leftResponseTime <=2){
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}
					if(leftResponseTime >2 && leftResponseTime <3){
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
			if(leftResponseTime >=3 ){
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}

		}



		if (health <= 0.0) {
			Destroy (gameObject);
		}

	}

	public void DoDamage (float damage) {


		if (!onDamage) {
			health -= damage;
			onDamage = true;
		}

	}


}
