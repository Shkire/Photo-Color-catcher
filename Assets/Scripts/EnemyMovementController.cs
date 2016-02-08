using UnityEngine;
using System.Collections;

public abstract class EnemyMovementController : CharacterMovementController {

	protected enum movementType
	{
		Undefined,
		Stay,
		Left,
		Right
	};

	protected bool pjEncountered;
	
	//Patron de movimiento estandar
	protected void MovementPattern(){

		if (!pjEncountered) {
			NormalMovement ();
		}
		else{
			PjEncounteredMovement ();
		}

	}

	protected virtual void NormalMovement(){
		
	}

	protected virtual void PjEncounteredMovement(){
	}

	public void HasEncounteredPj(){
		pjEncountered = true;
	}
}
