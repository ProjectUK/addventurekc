using UnityEngine;
using System.Collections;

public class SceneDesignerController : MonoBehaviour {

	public EnemySpawner enemySpawner;
	public PowerUpSpawner powerupSpawner;
	public EntitySpawner cloudSpawner;
	public AircraftController aircraft;

	// Use this for initialization
	void Start () {
		enemySpawner.StartSpawn ();
		powerupSpawner.StartSpawn ();
		cloudSpawner.StartSpawn ();
		aircraft.StartShooting ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
