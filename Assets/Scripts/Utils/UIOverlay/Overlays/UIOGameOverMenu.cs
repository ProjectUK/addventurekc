using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class UIOGameOverMenu : UISliding {

	[SerializeField] AircraftGameController GameController;

	[Header("UI References")]
	public Button GameOver_ReplayBtn;
	public Button GameOver_MenuBtn;
	public Button GameOver_LeaderboardBtn;
	public Button GameOver_ShareBtn;
	public Button GameOver_FollowBtn;

	[Header("Score UIs")]
	public Text TotalScore;
	public Text EnemiesScore;
	public Text BossScore;


	public override void Start ()
	{
		base.Start ();

		// set this overlay name
		UIOName = GameConst.UIO_GAME_OVER_MENU;

		GameOver_ReplayBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			EventManager.Instance.TriggerEvent(new StartGameEvent());
			Hide(GameConst.INTENT_HIDE_GAMEOVER_REPLAY, false);
		});

		GameOver_MenuBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Hide(GameConst.INTENT_HIDE_GAMEOVER_MAINMENU, false);
		});

		GameOver_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
//			Social.Active.ShowLeaderboardUI();
			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI();
		});

		GameOver_ShareBtn.onClick.AddListener (delegate() {
			GeneralSharing.Instance.ShareSimpleText("I scored " + GameController.TotalScore.ToString()+" in Crows Coffee Adventure! https://play.google.com/store/apps/details?id=com.visionesiastudio.crowscoffeegame");
		});

		GameOver_FollowBtn.onClick.AddListener (delegate {
			FollowWindowManager.Instance.Show("Follow Crows Coffee",
				"Facebook",
				"Twitter",
				"Instagram",
				()=>{
					Application.OpenURL("https://facebook.com/socialcrow");
				},
				()=>{
					Application.OpenURL("https://twitter.com/crowscoffee");
				},
				()=>{
					Application.OpenURL("https://instagram.com/crowscoffee");
				});				
			});

	}

	public override void Show (string message, bool immediate)
	{
		base.Show (message, immediate);
		TotalScore.text = GameController.TotalScore.ToString();
		EnemiesScore.text = GameController.EnemiesCount.ToString();
		BossScore.text = GameController.BossCount.ToString();
	}

	public override void OnUIOShowEvent (UIOShowEvent eve)
	{
		switch(eve.OverlayName) {
		case GameConst.UIO_GAME_OVER_MENU:
			break;

		}
	}

}
