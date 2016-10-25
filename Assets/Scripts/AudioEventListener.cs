using UnityEngine;
using System.Collections;

public class AudioEventListener : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// play music DJ!
//		if (GameSaveManager.Instance.IsBGMPlay ()) {
//			AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);
////			AudioManager.Instance.Mute (AudioManager.AudioType.BGM);
//		} else {
//			AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);
////			AudioManager.Instance.Unmute (AudioManager.AudioType.BGM);
//		}
//
//		AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);

		EventManager.Instance.AddListener<StartGameEvent> (OnStartGameEvent);
		EventManager.Instance.AddListener<GameLoseEvent> (OnGameLoseEvent);
		EventManager.Instance.AddListener<CafeActionStartedEvent> (OnCafeActionStartedEvent);
		EventManager.Instance.AddListener<ReceiveWakeEvent> (OnReceiveWakeEvent);
		EventManager.Instance.AddListener<BossStartEvent> (OnBossStartEvent);
		EventManager.Instance.AddListener<BossDeadEvent> (OnBossDeadEvent);
		EventManager.Instance.AddListener<MainMenuEvent> (OnMainMenuEvent);
	}

	void OnGameLoseEvent(GameLoseEvent eve) {
		StartCoroutine (IEGameOver ());	
	}

	IEnumerator IEGameOver() {
		yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (2f));
//		if (GameSaveManager.Instance.IsBGMPlay ()) {
			BGMManager.Instance.CurrentBGM = "";

			AudioManager.Instance.SetVolume ("BGM", 0, 0.1f);
			AudioManager.Instance.SetVolume ("BGM_boss", 0, 0.1f);
			AudioManager.Instance.Play ("gameover", false, 0.5f, 0.05f);
//		}
	}

	void OnStartGameEvent(StartGameEvent eve) {
		// play music DJ!
//		if (GameSaveManager.Instance.IsBGMPlay ()) {
		BGMManager.Instance.CurrentBGM = "BGM";

		AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);
		AudioManager.Instance.SetVolume ("BGM", 1f, 0.5f);

		// set boss sound to 0 if it's playing
		AudioManager.Instance.SetVolume ("BGM_boss", 0, 0f);
//		} else {
//			AudioManager.Instance.SetVolume ("BGM", 0f, 0.5f);
//			AudioManager.Instance.SetVolume ("BGM_boss", 0f, 0.5f);
//		}
	}

	void OnCafeActionStartedEvent(CafeActionStartedEvent eve) {
//		if (GameSaveManager.Instance.IsSFXPlay ()) {
			AudioManager.Instance.Play ("enter_pitstop", false, 0.5f, 0.1f);
			AudioManager.Instance.Play ("coffee_machine", false, 0.5f, 0.1f);
//		}
	}

	void OnReceiveWakeEvent(ReceiveWakeEvent eve) {
		AudioManager.Instance.Play ("get_coffee", false, 0.5f, 0f);
	}

	void OnBossStartEvent (BossStartEvent eve) {
//		if (GameSaveManager.Instance.IsBGMPlay ()) {

		BGMManager.Instance.CurrentBGM = "BGM_boss";

			Debug.Log ("Start boss!");
			AudioManager.Instance.Play ("BGM_boss", true, 1f, 0.5f);
			AudioManager.Instance.SetVolume ("BGM_boss", 1, 0.5f);

			AudioManager.Instance.SetVolume ("BGM", 0, 0.5f);
//		}
	}

	void OnBossDeadEvent (BossDeadEvent eve) {
//		if (GameSaveManager.Instance.IsBGMPlay ()) {
		BGMManager.Instance.CurrentBGM = "BGM";

			AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);
			AudioManager.Instance.SetVolume ("BGM", 1, 0.5f);

			AudioManager.Instance.SetVolume ("BGM_boss", 0f, 0.5f);
//		}
	}
		
	void OnMainMenuEvent (MainMenuEvent eve) {
		BGMManager.Instance.CurrentBGM = "BGM";
		AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);

		AudioManager.Instance.SetVolume ("BGM_boss", 0f, 0.1f);
	}

	// Update is called once per frame
	void Update () {
	}
}
