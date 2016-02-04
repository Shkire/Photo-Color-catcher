using UnityEngine;
using System.Collections;

public class ObjectiveDetectionController : MonoBehaviour {

	[SerializeField]
	private EnemyMovementController enemyMovementController;

	[SerializeField]
	private float sightRange;

	[SerializeField]
	private string tag;

	[SerializeField]
	private string[] layerCollisionList;

	private GameObject objectiveSight;

	private int layerCode;

	void OnEnable(){
		layerCode = LayerMask.GetMask (layerCollisionList);
	}

	void FixedUpdate(){
		if (objectiveSight == null) {
			RaycastHit2D[] hits = Physics2D.RaycastAll (this.gameObject.transform.position, Vector2.right, sightRange, layerCode);
			if (!tag.Equals (string.Empty))
				foreach (RaycastHit2D hit in hits) {
					if (hit.collider.tag.Equals (tag)) {
						objectiveSight = hit.collider.gameObject;
						enemyMovementController.hasEncounteredPj = true;
						break;
					}
				}
		}
	}

}
