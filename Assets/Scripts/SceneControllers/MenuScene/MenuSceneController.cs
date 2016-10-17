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
		LEADERBOARD
	}


	public InputField InputInitialField;
	public Text InitialText;
	public GameUIController GmUIController;
	public AircraftGameController GameController;

	[Header("Settings")]
	public int PopupEveryGameOver;

	[Header("Leaderboards Buttons")]
	public Button Leaderboard_BasecampBtn;

	[Header("Main Menu Buttons")]
	public Button MainMenu_PlayBtn;
	public Button MainMenu_LeaderboardBtn;
	public StackingButtonController StackingButtons;

	[Header("In Game Menu Buttons")]
	public Button InGameMenu_PauseBtn;

	[Header("Pause Menu Buttons")]
	public Button PauseMenu_ResumeBtn;
	public Button PauseMenu_MainMenuBtn;

	[Header("Game Over Buttons")]
	public Button GameOver_ReplayBtn;
	public Button GameOver_MenuBtn;
	public Button GameOver_LeaderboardBtn;
	public Text TotalScore;
	public Text EnemiesScore;
	public Text BossScore;

	[Header("Leaderboard positions")]
	public RectTransform Leaderboard;
	public RectTransform LeaderboardStartPos;
	public RectTransform LeaderboardEndPos;

	[Header("Main menu positions")]
	public RectTransform MainMenu;
	public RectTransform MainMenuStartPos;
	public RectTransform MainMenuEndPos;

	[Header("GameOver Menu positions")]
	public RectTransform GameOverMenu;
	public RectTransform GameOverMenuStartPos;
	public RectTransform GameOverMenuEndPos;

	[Header("Pause")]
	public RectTransform PauseMenu;

	private GameScreen _CurrentScreen;
	private NotificationData _NotificationData;
	private int _GameOverCount;

	// Use this for initialization
	void Start () {

		_NotificationData = GetComponent<NotificationData> ();

		// set up button listeners
		MainMenu_PlayBtn.onClick.AddListener(delegate {
			if (InputInitialField.text.Length > 0) {
				AudioManager.Instance.Play("button_click", false, 1f, 0f);
				EventManager.Instance.TriggerEvent(new StartGameEvent());
				MainMenu_Hide();
				_CurrentScreen = GameScreen.GAME;
			}else{
				NotificationWindowManager.Instance.Show("Name is empty!", "Please input your initial name");
			}
		});

		MainMenu_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Leaderboard_Show();
//			_CurrentScreen = GameScreen.LEADERBOARD;

		});

		// show pause
		InGameMenu_PauseBtn.onClick.AddListener (delegate {
			if (GmUIController.GameController.IsPlaying) {
				EventManager.Instance.TriggerEvent (new PauseGameEvent ());
				PauseMenu_Show ();
				_CurrentScreen = GameScreen.PAUSE;
			}
		});

		Leaderboard_BasecampBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			MainMenu_Show();
			Leaderboard_Hide();
			_CurrentScreen = GameScreen.MAIN_MENU;
		});

		GameOver_ReplayBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			EventManager.Instance.TriggerEvent(new StartGameEvent());
			GameOverMenu_Hide();
			MainMenu_Hide();
			_CurrentScreen = GameScreen.GAME;
		});

		GameOver_MenuBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			GameOverMenu_Hide();
			MainMenu_Show();
			_CurrentScreen = GameScreen.MAIN_MENU;
		});

		GameOver_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
//			GameOverMenu_Hide();
			Leaderboard_Show();
//			_CurrentScreen = GameScreen.LEADERBOARD;
		});

		PauseMenu_MainMenuBtn.onClick.AddListener (delegate {
			EventManager.Instance.TriggerEvent(new MainMenuEvent());
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			MainMenu_Show();
			Leaderboard_Hide();
			PauseMenu_Hide();
			_CurrentScreen = GameScreen.MAIN_MENU;
		});

		// resume from pause
		PauseMenu_ResumeBtn.onClick.AddListener (delegate {
			EventManager.Instance.TriggerEvent(new ResumeGameEvent());
			PauseMenu_Hide();
			_CurrentScreen = GameScreen.GAME;
		});

		// add listener to events
		EventManager.Instance.AddListenerOnce<GameLoseEvent> (OnGameLoseEvent);
		EventManager.Instance.AddListenerOnce<StartGameEvent> (OnStartGameEvent);

		// set initial position of game over
		GameOverMenu.position = GameOverMenuEndPos.position;

		// set initial position of leaderboard
		Leaderboard.position = LeaderboardEndPos.position;

	}

	public void TestLogin() {
		Social.localUser.Authenticate((bool success) => {
			// handle success or failure
			PlayGamesPlatform.Activate();
		});
	}

	void Update () {
		// detect pause button
		if (Input.GetKeyDown(KeyCode.Escape)) {

			if (_CurrentScreen == GameScreen.GAME) {
				if (GmUIController.GameController.IsPlaying) {
					EventManager.Instance.TriggerEvent (new PauseGameEvent ());
					PauseMenu_Show ();
					_CurrentScreen = GameScreen.PAUSE;
				}
			} else if (_CurrentScreen == GameScreen.PAUSE) {

				// resume
				EventManager.Instance.TriggerEvent (new ResumeGameEvent ());
				PauseMenu_Hide ();
				_CurrentScreen = GameScreen.GAME;

			} else if (_CurrentScreen == GameScreen.GAME_OVER) {
				// back to main menu
				MainMenu_Show ();
				GameOverMenu_Hide ();
				_CurrentScreen = GameScreen.MAIN_MENU;
			}else if (_CurrentScreen == GameScreen.LEADERBOARD) {
				// back to main menu
				MainMenu_Show ();
				Leaderboard_Hide ();
				_CurrentScreen = GameScreen.MAIN_MENU;
			}else if (_CurrentScreen == GameScreen.MAIN_MENU) {
				// exit
				//TODO:Prompt
				Application.Quit();
			}
		}
	}

	public void Leaderboard_Hide() {
		//TODO: Do something with the text

		Leaderboard.DOMove (LeaderboardEndPos.position, 0.5f, false).SetEase (Ease.InQuad).SetUpdate (true);
	}

	public void Leaderboard_Show() {
		
//		Leaderboard.DOMove(LeaderboardStartPos.position, 1, false).SetEase(Ease.OutBounce).SetUpdate(true).OnComplete(() =>{
//			_CurrentScreen = GameScreen.LEADERBOARD;
//		});
		Social.Active.ShowLeaderboardUI();
	}


	public void MainMenu_Hide() {
		//TODO: Do something with the text
		MainMenu.DOMove(MainMenuEndPos.position, 0.5f, false).SetEase(Ease.InQuad).SetUpdate(true);
		InitialText.text = InputInitialField.text;
		StackingButtons.StopBouncing ();
	}

	public void MainMenu_Show() {
		MainMenu.DOMove(MainMenuStartPos.position, 1, false).SetEase(Ease.OutBounce).SetUpdate(true);
		StackingButtons.ImmediateCollapse ();
		StackingButtons.StartBouncing ();
	}

	public void GameOverMenu_Hide() {
		GameOverMenu.DOMove(GameOverMenuEndPos.position, 0.5f, false).SetEase(Ease.InQuad).SetUpdate(true);
	}

	public void GameOverMenu_Show() {

		// set score

		TotalScore.text = GameController.TotalScore.ToString();
		EnemiesScore.text = GameController.EnemiesCount.ToString();
		BossScore.text = GameController.BossCount.ToString();

		GameOverMenu.DOMove(GameOverMenuStartPos.position, 1, false).SetEase(Ease.OutBounce).SetUpdate(true);
	}


	public void PauseMenu_Hide() {
		PauseMenu.gameObject.SetActive (false);
	}

	public void PauseMenu_Show() {
		PauseMenu.gameObject.SetActive (true);
	}


	#region event listeners
	private void OnGameLoseEvent(GameLoseEvent eve) {
		StartCoroutine(IEGameLoseEvent ());
	}

	private void OnStartGameEvent(StartGameEvent eve) {
		StackingButtons.StopBouncing ();
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

		GameOverMenu_Show ();

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
}
