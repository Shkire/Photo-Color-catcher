using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMovementController))]
public class CharacterHealthController : MonoBehaviour {

	[SerializeField]
	private float health;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (health <= 0.0) {

			Destroy (gameObject);
		
		}

	}

	public void DoDamage (float damage) {

		health -= damage;

	}




}
