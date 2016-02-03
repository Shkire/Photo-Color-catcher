using UnityEngine;
using System.Collections;

public abstract class EnemyMovementController : CharacterMovementController {

	protected bool pjEnconuntered;
	
	//Patron de movimiento estandar
	protected void MovementPattern(){

		if (!pjEnconuntered) {
			NormalMovement ();
		}
		else{
			PjEnconteredMovement ();
		}

	}

	protected virtual void NormalMovement(){
		
	}

	protected virtual void PjEnconteredMovement(){
	}
}
