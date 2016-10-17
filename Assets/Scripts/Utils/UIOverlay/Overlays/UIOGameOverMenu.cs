using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOGameOverMenu : UISliding {

	[SerializeField] AircraftGameController GameController;

	[Header("UI References")]
	public Button GameOver_ReplayBtn;
	public Button GameOver_MenuBtn;
	public Button GameOver_LeaderboardBtn;

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
			Hide(GameConst.INTENT_HIDE_GAMEOVER_REPLAY);
		});

		GameOver_MenuBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Hide(GameConst.INTENT_HIDE_GAMEOVER_MAINMENU);
		});

		GameOver_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Social.Active.ShowLeaderboardUI();
		});

	}

	public override void Show (string message)
	{
		base.Show (message);
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
