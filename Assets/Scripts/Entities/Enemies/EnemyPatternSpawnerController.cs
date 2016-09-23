using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPatternSpawnerController : EnemyController {

	public bool IsBossPatternSpawner;

	[System.Serializable]
	public class EnemyMovementModel {
		public float WaitTime;
		public float Duration;

		// flag whether enemy stay until dead
		public bool StayTillDead;

		public Transform Position;

		// index of next movement to play to
		public int GotoMovementIndex = -1;
	}

	[System.Serializable]
	public class EnemyPatternModel {
		public string EnemyPoolName;
		public List<EnemyMovementModel> Movements;
	}

	// Holder of all enemies on this pattern
	public List<EnemyPatternModel> EnemyList = new List<EnemyPatternModel>();

	private int _VisibleEnemiesCount = 0;

	bool AllEnemiesSpawned = false;

	Coroutine checkPatternEndRoutine;
	List<Coroutine> moveEnemyRoutines = new List<Coroutine>();

	public override void Init ()
	{
		AllEnemiesSpawned = false;

		_VisibleEnemiesCount = 0;

		SpawnEnemy();
		checkPatternEndRoutine = StartCoroutine (CheckPatternEnd ());

		CurrentHealth = EnemyList.Count;
		MaxHealth = EnemyList.Count;
	}

	void SpawnEnemy() {

		//DEBUG. Wait some time
//		yield return new WaitForSeconds(1);

		for (int i = 0; i < EnemyList.Count; i++) {
			EnemyPatternModel enemyPattern = EnemyList [i];

			GameObjectPool enemyPool = PoolManager.Instance.GetPool (enemyPattern.EnemyPoolName);
			GameObject enemyGO = enemyPool.Get ();

			if (enemyGO != null) {
				EnemyController ec = enemyGO.GetComponent<EnemyController> ();
				if (ec != null) {
					// make it not moving by its speed so we can override its pattern movement
					ec.IsMoving = false;
					ec.gameObject.SetActive (true);
					ec.Init ();

					// Count up living enemies
					_VisibleEnemiesCount++;
					CurrentHealth = _VisibleEnemiesCount;

					// start moving routine
					Coroutine moveEnemyRoutine =  StartCoroutine (MoveEnemySequence(ec, enemyPattern));
					moveEnemyRoutines.Add (moveEnemyRoutine);

					if (ec.EnemyType == AircraftType.BOSS) {
						// boss event
						EventManager.Instance.TriggerEvent (new BossStartEvent ());
					}

				}
			}
		}
		AllEnemiesSpawned = true;
	}

	IEnumerator MoveEnemySequence(EnemyController ec, EnemyPatternModel pm) {
		// do movements sequence
		int i = 0;
		while (i < pm.Movements.Count && ec.gameObject.activeInHierarchy && ec.CurrentHealth > 0) {
			EnemyMovementModel mm = pm.Movements[i];


			// first position
			if (i == 0) {
				ec.transform.position = mm.Position.position;
				yield return new WaitForSeconds (mm.WaitTime);
			}else {
				yield return new WaitForSeconds (mm.WaitTime);
				// move
				float t = 0;
				float currentTime = 0;
				Vector3 prevPos = ec.transform.position;
				// move only when the object active (not dead)
				while (t <= 1 && ec.gameObject.activeInHierarchy) {
					ec.transform.position = Vector3.Lerp (prevPos, mm.Position.position, t);
					currentTime += Time.deltaTime * Blackboard.Instance.EntitiesSpeedMultiplier;
					t = currentTime/mm.Duration;
					yield return null;
				}
			}

			// if it's stay until dead, wait until it's dead
			while (mm.StayTillDead && ec.gameObject.activeInHierarchy && ec.CurrentHealth > 0)
				yield return null;

			if (mm.GotoMovementIndex > -1)
				i = mm.GotoMovementIndex;
			else
				i++;
		}
		yield return null;		

		ec.gameObject.SetActive (false);

		// the end of its movement
		_VisibleEnemiesCount--;
		CurrentHealth = _VisibleEnemiesCount;
	}

	IEnumerator CheckPatternEnd() {
		
		while (_VisibleEnemiesCount > 0 || !AllEnemiesSpawned)
			yield return null;

		if (OnDead != null) {
			OnDead (this);
		}

		// After all enemies spawned and there are no visible enemies, it's time to disabled
//		StopAllCoroutines ();
		StopCoroutine(checkPatternEndRoutine);
		for (int i = 0; i < moveEnemyRoutines.Count; i++) {
			if (moveEnemyRoutines [i] != null)
				StopCoroutine (moveEnemyRoutines [i]);
		}
		moveEnemyRoutines.Clear ();

		this.gameObject.SetActive (false);
	}
		
}
