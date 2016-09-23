using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EntitySpawner : MonoBehaviour {

	[System.Serializable]
	public class SpawnModel
	{

		public string PoolName;

		[Range(0,100)]
		public int Percentage;

		int _PrevPercentage;
		public int GetPrevPercentage() {
			return _PrevPercentage;	
		}

		public void SetPrevPercentage(int percentage) {
			_PrevPercentage = percentage;
		}

		private int _PercentageStartRange;
		private int _PercentageEndRange;

		public void SetPercentageRange(int startRange, int endRange) {
			_PercentageStartRange = startRange;
			_PercentageEndRange = endRange;
		}

		public bool IsInRange(int value) {
			if (_PercentageStartRange <= value && value <= _PercentageEndRange) {
				return true;
			} else {
				return false;
			}
		}

		public int GetStartRange() {
			return _PercentageStartRange;
		}

		public int GetEndRange() {
			return _PercentageEndRange;
		}

	}

	public enum SPAWNING_STATE
	{
		PRE_BEFORE_SPAWN_WAIT,
		BEFORE_SPAWN_WAIT,
		SPAWN,
		AFTER_SPAWN_WAIT
	}

	protected SPAWNING_STATE _CurrentState;
	protected float _CurrentWaitTime 	= 0;
	protected float _TargetWaitTime 	= 0;
	protected Entity _CurrentSpawnedEntity;
	protected bool _IsSpawning = false;

	[Header("Random Variables")]
	public int RandomRange = 10;
	public int SpawnWhenLessThan = 10;
	public float MinRandomWait = 0.2f;
	public float MaxRandomWait = 2f;

	public Vector3 MaxOffset = new Vector3 ();
	public Vector3 MinOffset = new Vector3 ();


	[Header("Spawning positions")]
	public Transform[] Positions;

	[Header("Pools Preparation")]
	public GameObject[] Prefabs;

	List<GameObjectPool> Pools = new List<GameObjectPool>();

	[Header("Name of Pool to spawn")]
	public SpawnModel[] SpawnedModel;

	public bool IsPlaying;
	public int SpawnCount;

	// flag for waiting previous enemy ends
	bool _IsWaiting;

	private int _CurrentPercentageTotal;

//	private Coroutine _UpdateRoutine;

	#region Unity Methods

	void Update() {
		if (Application.isEditor) {
			DetectPercentageChange ();
		}

		UpdateSpawn ();
	}

	// Use this for initialization
	public virtual void Start () {
		SetPercentageRange ();
		CreatePools ();
	}

	#endregion

	// Use this for initialization
	public virtual void Initialize () {
	}

	public void StartSpawn() {
		Initialize ();

		_IsSpawning = true;
		_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;

		SpawnCount = 0;

//		_UpdateRoutine = StartCoroutine (UpdateSpawn ());
	}

	public void StopSpawn() {
		_IsSpawning = false;
//		if (_UpdateRoutine != null) 
//			StopCoroutine (_UpdateRoutine);
	}

	void CreatePools() {
		for (int i = 0; i < Prefabs.Length; i++) {
			GameObjectPool newPool = PoolManager.Instance.CreatePool (Prefabs[i].name, Prefabs[i], 10, true);
			Pools.Add (newPool);
		}
	}

	public void HideAll() {
		for (int i = 0; i < Pools.Count; i++) {
			Pools [i].HideAll ();
		}
	}

	public virtual Entity Spawn() {
		
		int randomNum = Random.Range (0, 101);

		SpawnModel model = GetDesiredRange (randomNum);

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
					return e;
				}
			}
			
		}
		return null;
	}

//	public virtual IEnumerator UpdateSpawn() {
//		while (true) {
//			yield return null;
//
//			if (IsPlaying) {
//				yield return new WaitForSeconds (Random.Range (MinRandomWait, MaxRandomWait));
//
//				int randomNumber = Random.Range (0, RandomRange);
//
//				if (randomNumber < SpawnWhenLessThan) {
//					
//					Entity currentSpawnedEntity = Spawn ();
//					
//					yield return StartCoroutine (SpawnWaiter (currentSpawnedEntity));
//				}									
//			}
//		}
//	}

	public virtual void UpdateSpawn() {

		if (!_IsSpawning)
			return;

		if (IsPlaying) {
//			yield return new WaitForSeconds (Random.Range (MinRandomWait, MaxRandomWait));

			if (_CurrentState == SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT) {

				// set target time
				_TargetWaitTime = Random.Range (MinRandomWait, MaxRandomWait);
				// reset wait tiem
				_CurrentWaitTime = 0;

				this._CurrentState = SPAWNING_STATE.BEFORE_SPAWN_WAIT;

			}else if (_CurrentState == SPAWNING_STATE.BEFORE_SPAWN_WAIT) {

				// reached the timer. Spawn
				if (_CurrentWaitTime >= _TargetWaitTime) {
					this._CurrentState = SPAWNING_STATE.SPAWN;
				} else {
					// increase time to target
					_CurrentWaitTime += Time.deltaTime;
				}
					
			}else if (_CurrentState == SPAWNING_STATE.SPAWN) {

				int randomNumber = Random.Range (0, RandomRange);

				// spawn chance
				if (randomNumber < SpawnWhenLessThan) {

					_CurrentSpawnedEntity = Spawn ();

					_CurrentState = SPAWNING_STATE.AFTER_SPAWN_WAIT;

				} else {
					// no spawn chance? back to first state
					_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;
				}
			}else if (_CurrentState == SPAWNING_STATE.AFTER_SPAWN_WAIT) {
				if (_CurrentSpawnedEntity != null) {
					if (!SpawnWaiter (_CurrentSpawnedEntity)) {
						_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;	
					}
				} else {
					// no spawn entity? back to first state
					_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;
				}
			}
											
		}
	}


//	protected virtual IEnumerator SpawnWaiter(Entity e) {
//		yield return null;
//	}

	// return true when waiting, return false when immediate spawn
	protected virtual bool SpawnWaiter(Entity e) {
		return false;
	}

	protected virtual void OnEntitySpawned(Entity e) {
	}


	void DetectPercentageChange () {

		if (SpawnedModel.Length <= 0)
			return;

		_CurrentPercentageTotal = 0;

		int becomeGreaterIndex = -1;

		// calculate total percentage and detect the increasing ones
		for (int i = 0; i < SpawnedModel.Length; i++) {
			SpawnModel currentModel = SpawnedModel [i];
			// detect change greater
			if (currentModel.Percentage > currentModel.GetPrevPercentage()) {
				becomeGreaterIndex = i;
			}
			_CurrentPercentageTotal += currentModel.Percentage;
			currentModel.SetPrevPercentage(currentModel.Percentage);
		}

		// decrease from other
		float RemainingPercentage = 100 - _CurrentPercentageTotal;
		if (RemainingPercentage < 0) {
			if (SpawnedModel.Length > 1) {
				for (int i = 0; i < SpawnedModel.Length; i++) {
					SpawnModel currentModel = SpawnedModel [i];
					
					//don't decrease from currently increasing model
					if (i != becomeGreaterIndex) {

						int decreasingValue = (int)Mathf.Ceil((-RemainingPercentage) / (SpawnedModel.Length - 1));

						if (currentModel.Percentage-decreasingValue > 0) {
							currentModel.Percentage -= decreasingValue;
						} else {
							// clamp to 0
							currentModel.Percentage = 0;
						}
					}
					
				}
			}
		}

	}

	private void SetPercentageRange() {
		// set the percentage range
		int startRange = 0;
		int endRange = 0;
		for (int i = 0; i < SpawnedModel.Length; i++) {
			SpawnModel currentModel = SpawnedModel [i];

			startRange = endRange + (i>0?1:0);
			endRange = startRange + currentModel.Percentage-1;

			currentModel.SetPercentageRange (startRange, endRange);
		}
	}

	protected SpawnModel GetDesiredRange(int value) {
		int i = 0;

		while (i < SpawnedModel.Length) {
			SpawnModel currentModel = SpawnedModel [i];
			if (currentModel.IsInRange(value)) {
				return currentModel;
			}else{
				i++;
			}
		}

		return null;

	}
}
