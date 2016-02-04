using UnityEngine;
using System.Collections;
using System;

public class BlueEnemyMovementController : EnemyMovementController {

	private System.Random rand;

	[SerializeField]
	private float timeLimit=6f;

	private float timeLeft;

	
	private float waittime= 3.0f;
	private float leftwaittime= 0.0f;


	void OnEnable(){

		leftwaittime = 4f;
		rand = new System.Random (Guid.NewGuid().GetHashCode());
	
	}

	void FixedUpdate(){

		leftwaittime -= Time.fixedDeltaTime;

		Debug.Log(leftwaittime);



		if(leftwaittime > 0 && leftwaittime < 2){



			NormalMovement();
		}
			

		if(leftwaittime > 2 && leftwaittime < 4){

			

		}
			


		if (leftwaittime <= 0){

			leftwaittime = 4f;

		}
	


	}
	 

	protected override void NormalMovement(){



		int i = rand.Next (-1, 2);

		Move (i);

	}

	protected override void PjEnconteredMovement(){



	}


}
