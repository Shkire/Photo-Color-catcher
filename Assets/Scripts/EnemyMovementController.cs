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
	protected GameObject pj;
	protected bool hasTeam;
	
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

	public virtual void TeamPlay(GameObject pj){
		hasTeam = true;
		pjEncountered = true;
		this.pj = pj;
		SendMessage ("StopSearching","Enemy");
	}

	public void HasEncounteredPj(GameObject pj){
		pjEncountered = true;
		this.pj = pj;
		SendMessage ("StopSearching","Pj");
	}
}
