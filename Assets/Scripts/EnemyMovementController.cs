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

	protected bool pjEnconuntered;

	public bool hasEncounteredPj{
		set{
			pjEnconuntered = value;
		}
		get{
			return pjEnconuntered;
		}
	}
	
	//Patron de movimiento estandar
	protected void MovementPattern(){

		if (!pjEnconuntered) {
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
}
