using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour {

	[System.Serializable]
	public class PowerupSpawnModel
	{

		public string PoolName;

		// to ensure we can load this kind because it already purchased
		public string DatabaseName;

		public bool NeedPurchased;

		public int Probability;

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
	public int MinKilledEnemies = 0;

	public Vector3 MaxOffset = new Vector3 ();
	public Vector3 MinOffset = new Vector3 ();

	[Header("Current Killed")]
	public int KilledEnemies = 0;

	[Header("Spawning positions")]
	public Transform[] Positions;

	[Header("Pools Preparation")]
	public GameObject[] Prefabs;

	List<GameObjectPool> Pools = new List<GameObjectPool>();

	[Header("Name of Pool to spawn")]
	public PowerupSpawnModel[] SpawnedModel;

	// Purchased powerups
	List<PowerupSpawnModel> PurchasedPowerUps = new List<PowerupSpawnModel>();

	public bool IsPlaying;
	public int SpawnCount;

	// flag for waiting previous enemy ends
	bool _IsWaiting;

	private int _CurrentPercentageTotal;
	private int _TotalRange;

	#region Unity Methods

	public virtual void Update() {
		UpdateSpawn ();
	}

	// Use this for initialization
	public virtual void Start () {
		CreatePools ();

		EventManager.Instance.AddListener<EnemyDeadEvent> (OnEnemyDeadEvent);
	}

	#endregion

	// Use this for initialization
	public void Initialize () {

		// prepare which object has been purchased
		PurchasedPowerUps.Clear();

		for (int i = 0; i < SpawnedModel.Length; i++) {
			PowerupSpawnModel spawnedModel = SpawnedModel [i];
			Debug.Log (spawnedModel.DatabaseName + " need purchased:" + spawnedModel.NeedPurchased);

			// check purchase
			if (spawnedModel.NeedPurchased) {
				Debug.Log (spawnedModel.DatabaseName + " check purchased:" + GameSaveManager.Instance.CheckPurchase(spawnedModel.DatabaseName));

				if(GameSaveManager.Instance.CheckPurchase (spawnedModel.DatabaseName))
					PurchasedPowerUps.Add (spawnedModel);
			} else {
				PurchasedPowerUps.Add (spawnedModel);
			}
		}

		SetPercentageRange ();
	}

	public void StartSpawn() {
		Initialize ();

		_IsSpawning = true;
		_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;

		SpawnCount = 0;
	}

	public void StopSpawn() {
		_IsSpawning = false;
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

		int randomNum = Random.Range (0, _TotalRange);

		PowerupSpawnModel model = GetDesiredRange (PurchasedPowerUps.ToArray(), randomNum);

		if (model != null) {
			string poolName = model.PoolName;

			if (poolName != null && poolName != "") {

				GameObjectPool gameObjectPool = PoolManager.Instance.GetPool (poolName);

				if (gameObjectPool != null) {
					GameObject spawnedEntity = gameObjectPool.Get ();
					
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

			}


		}
		return null;
	}

	public virtual void UpdateSpawn() {

		if (!_IsSpawning)
			return;

		if (IsPlaying) {

			if (_CurrentState == SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT) {

				// set target time
				_TargetWaitTime = Random.Range (MinRandomWait, MaxRandomWait);
				// reset wait time
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
				
				if (KilledEnemies >= MinKilledEnemies) {

					// reset killed enemies
					KilledEnemies = 0;

					int randomNumber = Random.Range (0, RandomRange);
					// spawn chance
					if (randomNumber < SpawnWhenLessThan) {
						_CurrentSpawnedEntity = Spawn ();
						_CurrentState = SPAWNING_STATE.AFTER_SPAWN_WAIT;
					} else {
						// no spawn chance? back to first state
						_CurrentState = SPAWNING_STATE.PRE_BEFORE_SPAWN_WAIT;
					}
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

	// return true when waiting, return false when immediate spawn
	protected virtual bool SpawnWaiter(Entity e) {
		return false;
	}
		
	private void SetPercentageRange() {
		//reset
		_TotalRange = 0;
			
		// set the percentage range
		int startRange = 0;
		int endRange = 0;
		for (int i = 0; i < PurchasedPowerUps.Count; i++) {
			PowerupSpawnModel currentModel = PurchasedPowerUps [i];

			_TotalRange += currentModel.Probability;

			startRange = endRange + (i>0?1:0);
			endRange = startRange + currentModel.Probability-1;

			currentModel.SetPercentageRange (startRange, endRange);
		}
	}

	// Load PowerupSpawnModel using its probability range
	protected PowerupSpawnModel GetDesiredRange(PowerupSpawnModel[] targetArray, int value) {
		int i = 0;

		while (i < PurchasedPowerUps.Count) {
			PowerupSpawnModel currentModel = PurchasedPowerUps [i];
			if (currentModel.IsInRange(value)) {
				return currentModel;
			}else{
				i++;
			}
		}

		return null;
	}


	void OnEntitySpawned (Entity e)
	{
		if (e != null) {
			MovingEntity me = e.GetComponent<MovingEntity> ();
			if (me != null) {
				me.IsMoving = true;
			}
		}
	}

	void OnEnemyDeadEvent(EnemyDeadEvent e) {
		KilledEnemies++;
	}

}
