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

		GameController.StoryOverlay.ScrollingStory.OnEnd += Handle_OnStoryEnd;
		GameController.Tutorial.OnEnd += Handle_OnTutorialEnd;
	}
		
	void Handle_OnCountEnd ()
	{
		GameController.StartGame ();
	}

	void Handle_OnStoryEnd () {
		GameController.HideStory ();
		GameController.ShowTutorial ();
	}

	void Handle_OnTutorialEnd () {
		StartCountdown ();
	}

	public void TestSpawn() {
		EnemySp.Spawn ();
	}

	void StartCountdown() {
		CountdownUI.StartCountdown (3, 0.65f, "GO!");
	}

	#region Event Listener

	void OnStartGameEvent (StartGameEvent eve) {
		GameController.ResetGame ();
		GameController.InitialGameStart ();


		// first start? then show story
		if (GameSaveManager.Instance.IsFirstPlay ()) {
			// show story first
			GameController.ShowStory();
		} else {
			GameController.HideStory();
			// immediately play countdown
			StartCountdown ();
		}

	}

	void OnPauseGameEvent (PauseGameEvent eve) {
		GameController.PauseGame ();
	}

	void OnResumeGameEvent(ResumeGameEvent eve) {
		GameController.ResumeGame ();
	}

	#endregion
}

