using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : EntitySpawner {

	bool _FirstNonBossSpawned = false;

	public override void Start ()
	{
		// Create bullet pools for shooting enemy
		PoolManager.Instance.CreatePool ("EnemyBulletPool", Resources.Load ("Prefabs/Bullets/EnemyBullet") as GameObject, 20, true);
		base.Start ();
	}

	public override void Initialize ()
	{
		_FirstNonBossSpawned = false;
	}

	public override Entity Spawn ()
	{
		int randomNum = Random.Range (0, 101);

		SpawnModel model = GetDesiredRange (randomNum);

		if (model != null) {
			string poolName = model.PoolName;

			GameObject spawnedEntity = PoolManager.Instance.GetPool (poolName).Get ();
			if (spawnedEntity != null) {

				Entity e = spawnedEntity.GetComponent<Entity> ();

				EnemyPatternSpawnerController ePatternSpawner = spawnedEntity.GetComponent<EnemyPatternSpawnerController> ();
				EnemyController eController = spawnedEntity.GetComponent<EnemyController> ();

				// prevent boss on first try
				if (!_FirstNonBossSpawned) {
					// check boss inside pattern spawner
					if (ePatternSpawner != null) {
						if (ePatternSpawner.IsBossPatternSpawner) {
							return null;
						}
					}else if (eController != null) {
						if (eController.EnemyType == EnemyController.AircraftType.BOSS) {
							return null;
						}
					}
				}


				if (e != null) {
					// get random position
					int randomPosIndex = Random.Range(0, Positions.Length-1);

					Vector3 randomPosition = Positions [randomPosIndex].position;
					Vector3 endPosition = new Vector3 (
						randomPosition.x + Random.Range(MinOffset.x, MaxOffset.x),
						randomPosition.y + Random.Range(MinOffset.y, MaxOffset.y),
						randomPosition.z + Random.Range(MinOffset.z, MaxOffset.z));
					e.transform.position = randomPosition;
					e.gameObject.SetActive (true);

					OnEntitySpawned (e);

					e.Init ();
					SpawnCount++;

					if (!_FirstNonBossSpawned)
						_FirstNonBossSpawned = true;

					return e;
				}
			}

		}
		return null;
	}

	protected override void OnEntitySpawned (Entity e)
	{

		if (e != null) {
			EnemyController ec = e.GetComponent<EnemyController> ();
			if (ec.EnemyType == EnemyController.AircraftType.NORMAL ||
				ec.EnemyType == EnemyController.AircraftType.SHOOTING ||
				ec.EnemyType == EnemyController.AircraftType.BOSS) {

				ec.IsMoving = true;

				// boss event
				if (ec.EnemyType == EnemyController.AircraftType.BOSS) {
					Debug.LogError ("BOSS EVENT!");
					EventManager.Instance.TriggerEvent (new BossStartEvent ());
				}

			} else if(ec.EnemyType == EnemyController.AircraftType.PATTERN) {
				ec.IsWaitTillDead = true;
				
				// By default the PATTERN type has to on (0,0,0) position
				ec.transform.position = Vector3.zero;
			}
		}
	}

	protected override bool SpawnWaiter (Entity e)
	{
		if (e != null) {
			EnemyController ec = e.GetComponent<EnemyController> ();
			if (ec.IsWaitTillDead) {
				// android debug: looks like enemy controller is active in hierarchy even though it's already
				// spew out all enemies, so we check current health which actually count how
				// many entity we have
				if (ec.gameObject.activeInHierarchy && ec.CurrentHealth > 0)
					return true;
			}	
		}
		return false;
	}

}
