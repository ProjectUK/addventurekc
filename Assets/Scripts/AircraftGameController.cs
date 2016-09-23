﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AircraftGameController : MonoBehaviour {

	public bool IsPlaying = false;
	private bool _TmpPlaying;


	[Header("Object References")]
	public AircraftController AircraftObject;

	[Header("Spawners")]
	public EnemySpawner EnemySpawnerObject;
	public PowerUpSpawner PowerUpSpawnerObject;
	public IncreasingSpawner CloudSpawner;

	[Header("UI")]
	public HorizontalBar WakeOMeterBar;
	public ItemRemainings LifeUI;
	public Text ScoreText;
	public PowerupInfoUI PowerupInfo;

	[Header("Wake Meter Values")]
	public float MaxWakeValue;
	public float CurrentWakeValue;
	public float WakeDecreasePerSecond;
	Coroutine WakeDecreaseRoutine;

	[Header("Static Values")]
	public int MaxLife;

	[Header("Game Values")]
	public int LifeRemaining;

	[Header("Parameters")]
	public float DistancePerSec = 5;

	[Header("Scores")]
	public float TotalScore = 0;
	public float DistanceScore;
	public float EnemiesCount;
	public float BossCount;
	public float CoffeeCount;

	public float ScorePerBoss = 1;
	public float ScorePerEnemy = 1;
	public float ScorePerCoffee = 1;

	public ScrollingBackgroundController BackgroundController;

	// Use this for initialization
	void Start () {
		_TmpPlaying = IsPlaying;	

		AircraftObject.OnExploding += OnAircraftExploding;
		AircraftObject.OnExploded += OnAircraftExploded;

		EventManager.Instance.AddListener<GameLoseEvent> (OnGameLoseEvent);
		EventManager.Instance.AddListener<CafeActionStartedEvent> (OnCafeActionStartedEvent);
		EventManager.Instance.AddListener<CafeActionEndedEvent> (OnCafeActionEndedEvent);
		EventManager.Instance.AddListener<ReceiveWakeEvent> (OnReceiveWakeEvent);
		EventManager.Instance.AddListener<EnemyDeadEvent> (OnEnemyDeadEvent);

		LifeRemaining = MaxLife;

		StartCoroutine (IEUpdateDistanceScore ());
	}
	
	// Update is called once per frame
	void Update () {

//		TotalScore = DistanceScore + EnemiesScore + BossScore + CoffeeScore;
//		TotalScore = EnemiesCount + BossCount + CoffeeCount;
		TotalScore = (EnemiesCount * ScorePerEnemy) + (BossCount * ScorePerBoss) + (CoffeeCount * ScorePerCoffee);

		UpdatePlaying ();
		UpdateWakeMeter ();
		UpdateUI ();
		UpdateScreen ();
	}

	// when changes in playing flag
	void UpdatePlaying() {
		if (_TmpPlaying != IsPlaying) {

			// save current states
			_TmpPlaying = IsPlaying;

			if (IsPlaying) {
				AircraftObject.Playing = true;
			} else {
				AircraftObject.Playing = false;
			}

			EnemySpawnerObject.IsPlaying = IsPlaying;
			PowerUpSpawnerObject.IsPlaying = IsPlaying;
			CloudSpawner.IsPlaying = IsPlaying;
			AircraftObject.IsPlaying = IsPlaying;
			PowerupInfo.IsPlaying = IsPlaying;
		}
	}

	void UpdateUI() {
		WakeOMeterBar.Value = CurrentWakeValue / MaxWakeValue;
		LifeUI.Value = LifeRemaining;
		ScoreText.text = TotalScore.ToString();
	}


	IEnumerator IEUpdateDistanceScore() {
		while(true) {
			yield return null;

			// distance
			if (Time.timeScale > 0 && IsPlaying) {
				yield return new WaitForSeconds (1);
				DistanceScore += DistancePerSec;
			}
		}
	}

	void UpdateWakeMeter() {
		if (CurrentWakeValue >= 0 && IsPlaying) {
			CurrentWakeValue -= WakeDecreasePerSecond*Time.deltaTime;
			
			if (CurrentWakeValue < 0) {
				OnWakeMeterDepleted ();
			}
		}
	}

	void UpdateScreen() {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}



	public void ResetGame() {

		LifeRemaining = MaxLife;
		CurrentWakeValue = MaxWakeValue;

		TotalScore = 0;
		EnemiesCount = 0;
		BossCount = 0;
		DistanceScore = 0;
		CoffeeCount = 0;

		AircraftObject.gameObject.SetActive (true);
		AircraftObject.Restart ();

		// remove enemies
		EnemySpawnerObject.StopSpawn ();
		EnemySpawnerObject.HideAll ();

		// remove powerups
		PowerUpSpawnerObject.StopSpawn ();
		PowerUpSpawnerObject.HideAll ();

		// remove clouds
		CloudSpawner.StopSpawn();
		CloudSpawner.HideAll();


		// remove bullets
		EventManager.Instance.TriggerEvent (new HideBulletEvent ());
	}

	public void StartGame() {

		AircraftObject.InitialGameStarting = false;
		AircraftObject.Playing = true;

		// initialize aircraft input
		AircraftObject.Init ();

		// background changing
		BackgroundController.IsChanging = true;

		//TODO: Background set to default at the beginning

		// aircraft shooting
		AircraftObject.StartShooting ();

		EnemySpawnerObject.StartSpawn ();
		PowerUpSpawnerObject.StartSpawn ();
		CloudSpawner.StartSpawn ();

		// let the game begins!
		IsPlaying = true;
	}

	//TODO: For what?? Gameover?
	public void StopGame() {
		AircraftObject.Playing = false;

		IsPlaying = false;
		AircraftObject.StopShooting ();

		EnemySpawnerObject.StopSpawn ();
		PowerUpSpawnerObject.StopSpawn ();
		CloudSpawner.StopSpawn ();
	}


	public void PauseGame() {
		IsPlaying = false;
		AircraftObject.Playing = false;
		Time.timeScale = 0;
	}

	public void ResumeGame() {
		IsPlaying = true;
		AircraftObject.Playing = true;
		Time.timeScale = 1;
	}

	public void InitialGameStart() {
		AircraftObject.InitialGameStarting = true;
		AircraftObject.Init ();
		ResumeGame ();
		ResetGame ();
	}

	// respawn aircraft after die
	public void RespawnAircraft() {
		
		StartCoroutine (WaitRespawn (0.5f));
	}

	IEnumerator WaitRespawn(float waitTime){
		yield return null;

		// show respawn particle
		Tinker.ParticleManager.Instance.Spawn("MagicDrain", AircraftObject.DefaultStartPos.position);

		yield return new WaitForSeconds (waitTime);

		// show character again
		AircraftObject.gameObject.SetActive (true);

		// reset states
		AircraftObject.Restart ();
		AircraftObject.StartShooting ();


	}


	#region delegates
	void OnAircraftExploding() {

		EventManager.Instance.TriggerEvent (new CameraShakeEvent (0.5f, 1, 10, 5));

		LifeRemaining--;
	}

	void OnAircraftExploded() {		
		if (LifeRemaining > 0) {
			RespawnAircraft ();
		} else {
//			StopGame ();
			EventManager.Instance.TriggerEvent (new GameLoseEvent ());
		}
	}

	#endregion

	#region event listeners
	void OnGameLoseEvent(GameLoseEvent eve) {
		IsPlaying = false;
		AircraftObject.Explode ();
	}

	void OnCafeActionStartedEvent(CafeActionStartedEvent eve) {
		IsPlaying = false;
	}

	void OnCafeActionEndedEvent(CafeActionEndedEvent eve) {
		IsPlaying = true;
	}

	void OnReceiveWakeEvent(ReceiveWakeEvent eve) {
		CurrentWakeValue += eve.WakeValue;
		CurrentWakeValue = Mathf.Clamp (CurrentWakeValue, 0, MaxWakeValue);

		CoffeeCount++;
	}

	void OnEnemyDeadEvent(EnemyDeadEvent eve) {

		EnemiesCount++;

		if (eve.AircraftType == EnemyController.AircraftType.BOSS) {
			BossCount++;
		}
	}
	#endregion

	#region misc events
	void OnWakeMeterDepleted() {
		EventManager.Instance.TriggerEvent (new GameLoseEvent ());
	}
	#endregion
}