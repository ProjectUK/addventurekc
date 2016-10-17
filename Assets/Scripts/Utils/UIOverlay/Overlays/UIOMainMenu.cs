using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOMainMenu : UISliding {

	[Header("UI References")]
	public Button MainMenu_PlayBtn;
	public Button MainMenu_LeaderboardBtn;
	public StackingButtonController StackingButtons;

	public InputField InputInitialField;
	public Text InitialText;

	public override void Start ()
	{
		base.Start ();

		// set this overlay name
		UIOName = GameConst.UIO_MAIN_MENU;

		EventManager.Instance.AddListenerOnce<StartGameEvent> (OnStartGameEvent);

		MainMenu_PlayBtn.onClick.AddListener(delegate {
			if (InputInitialField.text.Length > 0) {
				AudioManager.Instance.Play("button_click", false, 1f, 0f);
				EventManager.Instance.TriggerEvent(new StartGameEvent());

				Hide(GameConst.INTENT_HIDE_MAINMENU_PLAY);
//				MainMenu_Hide();
//				_CurrentScreen = GameScreen.GAME;

			}else{
				NotificationWindowManager.Instance.Show("Name is empty!", "Please input your initial name");
			}
		});

		MainMenu_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Social.Active.ShowLeaderboardUI();
		});

	}

	public override void Hide (string message)
	{
		base.Hide (message);
		StackingButtons.StopBouncing ();
	}

	public override void Show (string message)
	{
		base.Show (message);
		StackingButtons.ImmediateCollapse ();
		StackingButtons.StartBouncing ();
	}

	private void OnStartGameEvent(StartGameEvent eve) {
		StackingButtons.StopBouncing ();
	}


}
