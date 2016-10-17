/**
 * 
 * Class to change the speed of the game
 * 
 */


using UnityEngine;
using System.Collections;

public class SpeedLevelChanger : MonoBehaviour {


	[System.Serializable]
	public class LevelModel{ 
		public float MinScore;
		public float MaxScore;
		public float EntitiesSpeedMultiplier;
		public float BulletSpeedMultiplier;
		public float BulletSpawnSpeedMultiplier;
		public float EnemyHealthMultiplier;
		public float WakeDecreaseMultiplier;
		public float MaxEnemySpawnRandomWait;
	}

	public LevelModel[] Levels;

	public AircraftGameController AircraftGame;

	Blackboard _Blackboard;

	public float CheckingInterval = 1;


	// Use this for initialization
	void Start () {

		_Blackboard = GetComponent<Blackboard> ();

		// in case we forgot to drag it
		if (AircraftGame == null) {
			AircraftGame = GameObject.FindObjectOfType<AircraftGameController> ();
		}

		StartCoroutine (CheckLevelRoutine ());
	}
	
	// Update is called once per frame
	void Update () {

	}

	// update every interval of second
	IEnumerator CheckLevelRoutine () {
		while (true) {
			checkLevel ();
			yield return new WaitForSeconds (CheckingInterval);
		}
	}

	LevelModel getLevelModel(float currentScore) {
		for (int i = 0; i < Levels.Length; i++) {
			if (Levels[i].MinScore <= currentScore && currentScore <= Levels[i].MaxScore ) {
				return Levels [i];
			}
		}
		return null;
	}

	void checkLevel() {
		LevelModel levelModel = getLevelModel (AircraftGame.TotalScore);

		if (levelModel != null) {
			_Blackboard.BulletSpawnSpeedMultiplier 	= levelModel.BulletSpawnSpeedMultiplier;
			_Blackboard.BulletSpeedMultiplier 		= levelModel.BulletSpeedMultiplier;
			_Blackboard.EntitiesSpeedMultiplier 	= levelModel.EntitiesSpeedMultiplier;
			_Blackboard.EnemyHealthMultiplier 		= levelModel.EnemyHealthMultiplier;
			_Blackboard.WakeDecreaseMultiplier 		= levelModel.WakeDecreaseMultiplier;
			_Blackboard.MaxEnemySpawnRandomWait 	= levelModel.MaxEnemySpawnRandomWait;
		}
	}
}
