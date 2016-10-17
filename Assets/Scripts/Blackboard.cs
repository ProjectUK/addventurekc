using UnityEngine;
using System.Collections;

public class Blackboard : MonoBehaviour {

	private static Blackboard s_Instance = null;
	public static Blackboard Instance {
		get{
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(Blackboard)) as Blackboard;

			}

			return s_Instance;
		}
	}

	public float EntitiesSpeedMultiplier = 1;
	public float BulletSpeedMultiplier = 1;
	public float BulletSpawnSpeedMultiplier = 1;
	public float EnemyHealthMultiplier = 1;
	public float WakeDecreaseMultiplier = 1;
	public float MaxEnemySpawnRandomWait = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
