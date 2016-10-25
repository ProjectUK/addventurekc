using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;


public class MenuSceneController : MonoBehaviour {

	public enum GameScreen
	{
		MAIN_MENU,
		GAME,
		GAME_OVER,
		PAUSE,
		LEADERBOARD,
		SHOP
	}

	public GameUIController GmUIController;
	public AircraftGameController GameController;

	[Header("Settings")]
	public int PopupEveryGameOver;

	[Header("Leaderboards Buttons")]
	public Button Leaderboard_BasecampBtn;

	[Header("In Game Menu Buttons")]
	public Button InGameMenu_PauseBtn;

	[Header("UI Overlays")]
	[SerializeField] UIOGameOverMenu _UIOGameOver;
	[SerializeField] UIOMainMenu _UIOMainMenu;
	[SerializeField] UIOPauseMenu _UIOPauseMenu;
	[SerializeField] UIOLeaderboard _UIOLeaderboard;
	[SerializeField] UIOShopMenu _UIOShopMenu;


	private GameScreen _CurrentScreen;
	private NotificationData _NotificationData;
	private int _GameOverCount;

	// Use this for initialization
	void Start () {

		_NotificationData = GetComponent<NotificationData> ();

		EventManager.Instance.AddListener<UIOHideEvent> (OnUIOHideEvent);
		EventManager.Instance.AddListener<UIOShowEvent> (OnUIOShowEvent);

		// show pause
		InGameMenu_PauseBtn.onClick.AddListener (delegate {
			if (GmUIController.GameController.IsPlaying) {
				EventManager.Instance.TriggerEvent (new PauseGameEvent ());
				_UIOPauseMenu.Show(false);
				_CurrentScreen = GameScreen.PAUSE;
			}
		});
			
		// add listener to events
		EventManager.Instance.AddListenerOnce<GameLoseEvent> (OnGameLoseEvent);

		// set initial states of menu
		_UIOGameOver.Hide(true);
		_UIOPauseMenu.Hide (true);
		_UIOLeaderboard.Hide (true);
		_UIOShopMenu.Hide (true);

	
		// play BGM at the very beginning of the game
		BGMManager.Instance.CurrentBGM = "BGM";
		AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);


	}

	public void TestLogin() {
		Social.localUser.Authenticate((bool success) => {
			// handle success or failure
			PlayGamesPlatform.Activate();
		});
	}

	void Update () {
		HandleBackButton ();
	}

	void HandleBackButton() {
		// detect pause button
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_CurrentScreen == GameScreen.GAME) {
				if (GmUIController.GameController.IsPlaying) {
					EventManager.Instance.TriggerEvent (new PauseGameEvent ());
					_UIOPauseMenu.Show(false);

					_CurrentScreen = GameScreen.PAUSE;
				}
			} else if (_CurrentScreen == GameScreen.PAUSE) {

				// resume
				EventManager.Instance.TriggerEvent (new ResumeGameEvent ());
				_UIOPauseMenu.Hide(false);

				_CurrentScreen = GameScreen.GAME;

			} else if (_CurrentScreen == GameScreen.GAME_OVER) {
				// back to main menu
				_UIOMainMenu.Show(false);
				_UIOGameOver.Hide(false);

				_CurrentScreen = GameScreen.MAIN_MENU;
			}else if (_CurrentScreen == GameScreen.LEADERBOARD) {

				// back to main menu
				_UIOMainMenu.Show(false);

				_UIOLeaderboard.Hide (false);
				_CurrentScreen = GameScreen.MAIN_MENU;
			}else if (_CurrentScreen == GameScreen.MAIN_MENU) {
				// exit
				//TODO:Prompt

				NotificationWindowManager.Instance.Show(
					"Quit Game?",
					"Are you sure you want to quit?",
					"Sure",
					()=>{
						Application.Quit();
					});
			}
		}
	}
		
	#region event listeners
	private void OnGameLoseEvent(GameLoseEvent eve) {
		StartCoroutine(IEGameLoseEvent ());
	}

	#endregion

	IEnumerator IEGameLoseEvent() {
		_GameOverCount++;
		float currTime = 0;

		float waitTime = 2f;

		// wait some moments while slowing down
		while (currTime <= waitTime) { 
			currTime += IndieTime.Instance.deltaTime;

			Time.timeScale = Mathf.Lerp (1, 0, currTime / waitTime);
			yield return null;
		}
		Time.timeScale = 0;

		_UIOGameOver.Show(false);

		if (_GameOverCount%PopupEveryGameOver == 0) {
			NotificationPopup ();
		}

		_CurrentScreen = GameScreen.GAME_OVER;
	}

	void NotificationPopup (){
		int randIndex = Random.Range (0, _NotificationData.NotificationList.Count);
		NotificationModel selectedNotification = _NotificationData.NotificationList [randIndex];

		NotificationWindowManager.Instance.Show(
			selectedNotification.HeaderText,
			selectedNotification.InfoText,
			selectedNotification.ButtonText,
			()=>{
			Application.OpenURL(selectedNotification.UrlTarget);
		});
	}

	void OnUIOHideEvent(UIOHideEvent eve) {
		switch (eve.OverlayName) {
		case GameConst.UIO_GAME_OVER_MENU:
			if (eve.Message == GameConst.INTENT_HIDE_GAMEOVER_MAINMENU) {
				// back to main menu
				_CurrentScreen = GameScreen.MAIN_MENU;
				_UIOMainMenu.Show(false);

				// play BGM
				BGMManager.Instance.CurrentBGM = "BGM";
				AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);

			} else if(eve.Message == GameConst.INTENT_HIDE_GAMEOVER_REPLAY)  {
				// to replay
				_CurrentScreen = GameScreen.GAME;
				_UIOMainMenu.Hide(false);
			}
			break;
		case GameConst.UIO_MAIN_MENU:
			if (eve.Message == GameConst.INTENT_HIDE_MAINMENU_PLAY) {
				_CurrentScreen = GameScreen.GAME;
			} else if (eve.Message == GameConst.INTENT_HIDE_MAINMENU_SHOP) {
				_CurrentScreen = GameScreen.SHOP;
				_UIOShopMenu.Show (false);
			}
			break;
		case GameConst.UIO_PAUSE_MENU:
			if (eve.Message == GameConst.INTENT_HIDE_PAUSE_MAINMENU) {
				_UIOMainMenu.Show (false);
				_CurrentScreen = GameScreen.MAIN_MENU;

				// play BGM
				BGMManager.Instance.CurrentBGM = "BGM";
				AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);

			}else if (eve.Message == GameConst.INTENT_HIDE_PAUSE_RESUME) {
				_CurrentScreen = GameScreen.GAME;
			}
			break;
		case GameConst.UIO_LEADERBOARD:
			if (eve.Message == GameConst.INTENT_HIDE_LEADERBOARD_MAINMENU) {
				_UIOMainMenu.Show (false);
				_CurrentScreen = GameScreen.MAIN_MENU;
			}
			break;
		case GameConst.UIO_SHOP_MENU:
			if (eve.Message == GameConst.INTENT_HIDE_SHOP_MAINMENU) {
				_UIOMainMenu.Show (false);
				_CurrentScreen = GameScreen.MAIN_MENU;

				// play BGM
				BGMManager.Instance.CurrentBGM = "BGM";
				AudioManager.Instance.Play ("BGM", true, 1f, 0.5f);

			}
			break;

		}
	}

	void OnUIOShowEvent(UIOShowEvent eve) {
	}
}
