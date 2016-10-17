using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitScoreSpawner : MonoBehaviour {

	const string HIT_SCORE_TEXT = "Prefabs/UIs/HitScoreText";
	[SerializeField] AircraftGameController _AircraftGameController;

	void Start () {
		// pool hit marker
		PoolManager.Instance.CreatePool("HitScoreText", Resources.Load(HIT_SCORE_TEXT) as GameObject, 10, true);

		EventManager.Instance.AddListener<EnemyDeadEvent> (OnEnemyDeadEvent);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnemyDeadEvent(EnemyDeadEvent eve) {
		// spawn marker
		GameObject hitScoreTextObj = PoolManager.Instance.GetPool("HitScoreText").Get();
		hitScoreTextObj.transform.position = eve.EnemyController.transform.position;
		hitScoreTextObj.SetActive (true);
		HitScoreText hitScoreText = hitScoreTextObj.GetComponent<HitScoreText> ();
		if (eve.EnemyController.EnemyType == EnemyController.AircraftType.BOSS) {
			hitScoreText.Show ("+" + _AircraftGameController.ScorePerBoss);
		} else {
			hitScoreText.Show ("+" + _AircraftGameController.ScorePerEnemy);
		}


	}
}
