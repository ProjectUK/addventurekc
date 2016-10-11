using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : EntitySpawner {

	[Header("Boss SpawnModel")]
	public SpawnModel[] BossSpawnedModel;

	[Header("Boss Configuration")]
	[SerializeField] int _BossAppearanceEveryKill = 25;

	bool _FirstNonBossSpawned = false;
	int _BossCount;
	int _EnemyKillCount;

	public override void Start ()
	{
		// Create bullet pools for shooting enemy
		PoolManager.Instance.CreatePool ("EnemyBulletPool", Resources.Load ("Prefabs/Bullets/EnemyBullet") as GameObject, 20, true);
		base.Start ();

		EventManager.Instance.AddListener<EnemyDeadEvent> (OnEnemyDeadEvent);

	}

	public override void Initialize ()
	{
		_FirstNonBossSpawned = false;
		_BossCount = 0;
		_EnemyKillCount = 0;
	}

	// spawn NON BOSS enemy
	public override Entity Spawn ()
	{
		int randomNum = Random.Range (0, 101);

		SpawnModel model = GetDesiredRange (SpawnedModel, randomNum);

		if (model != null) {
			string poolName = model.PoolName;

			GameObject spawnedEntity = PoolManager.Instance.GetPool (poolName).Get ();
			if (spawnedEntity != null) {
				Entity e = spawnedEntity.GetComponent<Entity> ();

				EnemyPatternSpawnerController ePatternSpawner = spawnedEntity.GetComponent<EnemyPatternSpawnerController> ();

				// prevent spawn boss
				if (ePatternSpawner != null) {
					if (ePatternSpawner.IsBossPatternSpawner)
						return null;
				}
				
				EnemyController eController = spawnedEntity.GetComponent<EnemyController> ();

				// prevent spawn boss
				if (eController.EnemyType == EnemyController.AircraftType.BOSS) {
					return null;
				}


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

	public Entity SpawnBoss() {
		int randomNum = Random.Range (0, BossSpawnedModel.Length);

		SpawnModel model = BossSpawnedModel[randomNum];

		if (model != null) {
			string poolName = model.PoolName;
			GameObject spawnedEntity = PoolManager.Instance.GetPool (poolName).Get ();

			if (spawnedEntity != null) {

				Entity e = spawnedEntity.GetComponent<Entity> ();

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

					_BossCount++;
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


	#region Event Listeners

	void OnEnemyDeadEvent(EnemyDeadEvent eve) {
		if (eve.AircraftType != EnemyController.AircraftType.BOSS) {

			// only count when there's no boss
			if (_BossCount <= 0) {
				_EnemyKillCount++;
			}

		} else {
			if (_BossCount > 0) {
				_BossCount--;
			}
		}

		if (_EnemyKillCount >= _BossAppearanceEveryKill) {
			_EnemyKillCount = 0;

			// only spawn boss if there's no boss
			if (_BossCount <= 0) {
				SpawnBoss ();

				// reset kill count
				_EnemyKillCount = 0;
			}
		}
	}

	#endregion
}
