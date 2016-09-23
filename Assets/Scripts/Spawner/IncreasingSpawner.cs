using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IncreasingSpawner : EntitySpawner {

	public float StartMinRandomWait;
	public float EndMinRandomWait;
	public float StartMaxRandomWait;
	public float EndMaxRandomWait;

	// Iteration needed to lerp from start to end
	public int IterationStartToEnd = 1;

	int currentIteration = 0;


	public override void Initialize ()
	{
		MinRandomWait = StartMinRandomWait;
		MaxRandomWait = StartMaxRandomWait;
		currentIteration = 0;
	}

//	public override IEnumerator UpdateSpawn ()
//		MinRandomWait = StartMinRandomWait;
//		MaxRandomWait = StartMaxRandomWait;
//		currentIteration = 0;
//
//		while (true) {
//			yield return null;
//
//			if (IsPlaying) {
//				yield return new WaitForSeconds (Random.Range (MinRandomWait, MaxRandomWait));
//
//				// update wait time
//				float t = (float)currentIteration/(float)IterationStartToEnd;
//				MinRandomWait = Mathf.Lerp (StartMinRandomWait, EndMinRandomWait, t);
//				MaxRandomWait = Mathf.Lerp (StartMaxRandomWait, EndMaxRandomWait, t);
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
//
//			currentIteration++;
//
//		}
//	}

	public override void UpdateSpawn ()
	{
		if (!_IsSpawning)
			return;

		if (IsPlaying) {
			//			yield return new WaitForSeconds (Random.Range (MinRandomWait, MaxRandomWait));

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

				int randomNumber = Random.Range (0, RandomRange);

				// update wait time
				float t = (float)currentIteration/(float)IterationStartToEnd;
				MinRandomWait = Mathf.Lerp (StartMinRandomWait, EndMinRandomWait, t);
				MaxRandomWait = Mathf.Lerp (StartMaxRandomWait, EndMaxRandomWait, t);

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

}
