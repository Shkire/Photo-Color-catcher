using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour
{

	[SerializeField]
	private EnemyMovementController[] enemies;

	[SerializeField]
	private float minSpawnTime;

	[SerializeField]
	private float maxSpawnTime;

	private float leftSpawnTime;

	private System.Random rnd;

	void OnEnable ()
	{
		rnd = new System.Random (Guid.NewGuid ().GetHashCode ());
	}

	void FixedUpdate ()
	{
		leftSpawnTime -= Time.fixedDeltaTime;
		if (leftSpawnTime <= 0) {
			int randomEnemy = rnd.Next (0, enemies.Length + 1);
			Instantiate (enemies [randomEnemy], this.transform.position, enemies [randomEnemy].transform.rotation);
			leftSpawnTime = (float)(rnd.NextDouble () * (maxSpawnTime - minSpawnTime)) + minSpawnTime;
		}
	}
}
