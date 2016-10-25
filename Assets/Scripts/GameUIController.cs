using UnityEngine;
using System.Collections;

public class GameUIController : MonoBehaviour {

	public AircraftGameController GameController;
	public EnemySpawner EnemySp;
	public Countdown CountdownUI;
	public MenuSceneController MenuSceneCtrl;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		CountdownUI.OnEnd += Handle_OnCountEnd;

		// make everything frozen. Except UI, because it use dotween, of course
		Time.timeScale = 0;

		EventManager.Instance.AddListener<StartGameEvent> (OnStartGameEvent);
		EventManager.Instance.AddListener<PauseGameEvent> (OnPauseGameEvent);
		EventManager.Instance.AddListener<ResumeGameEvent> (OnResumeGameEvent);
	}
		
	void Handle_OnCountEnd ()
	{
		GameController.StartGame ();
	}

//	public void PauseGame() {
//		GameController.PauseGame ();
//	}
//
//	public void StopGame() {
//		GameController.StopGame ();
//	}

	public void TestSpawn() {
		EnemySp.Spawn ();
	}

//	public void TestBack() {
//		MenuSceneCtrl.MainMenu_Show ();
//	}

	#region Event Listener

	void OnStartGameEvent (StartGameEvent eve) {
		GameController.ResetGame ();
		GameController.InitialGameStart ();
		// count down start
		CountdownUI.StartCountdown (3, 0.1f, "GO!");
	}

	void OnPauseGameEvent (PauseGameEvent eve) {
		GameController.PauseGame ();
	}

	void OnResumeGameEvent(ResumeGameEvent eve) {
		GameController.ResumeGame ();
	}

	#endregion
}

