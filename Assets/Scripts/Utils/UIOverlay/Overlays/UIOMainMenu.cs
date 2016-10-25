using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOMainMenu : UISliding {

	[Header("UI References")]
	public Button MainMenu_PlayBtn;
	public Button MainMenu_LeaderboardBtn;
	public Button MainMenu_ShopBtn;
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
				EventManager.Instance.TriggerEvent(new StartGameEvent(InputInitialField.text));

				Hide(GameConst.INTENT_HIDE_MAINMENU_PLAY, false);


//				MainMenu_Hide();
//				_CurrentScreen = GameScreen.GAME;

			}else{
				NotificationWindowManager.Instance.Show(
					"Whoa Whoa Whoa! You Forgot Your Name?!",
					"Why the rush? Got too much coffee? :D");
			}
		});

		MainMenu_LeaderboardBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Social.Active.ShowLeaderboardUI();
		});

		MainMenu_ShopBtn.onClick.AddListener (delegate {
			Hide(GameConst.INTENT_HIDE_MAINMENU_SHOP, false);
		});


	}

	public override void Hide (string message, bool immediate)
	{
		base.Hide (message, immediate);
		StackingButtons.StopBouncing ();
	}

	public override void Show (string message, bool immediate)
	{
		base.Show (message, immediate);
		StackingButtons.ImmediateCollapse ();
		StackingButtons.StartBouncing ();
	}

	private void OnStartGameEvent(StartGameEvent eve) {
		StackingButtons.StopBouncing ();
	}


}
