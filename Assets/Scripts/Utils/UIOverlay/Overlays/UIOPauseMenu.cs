using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOPauseMenu : UIOverlay {

	[Header("UI References")]
	public Button PauseMenu_ResumeBtn;
	public Button PauseMenu_MainMenuBtn;

	public override void Start ()
	{
		base.Start ();

		// set this overlay name
		UIOName = GameConst.UIO_PAUSE_MENU;

		PauseMenu_MainMenuBtn.onClick.AddListener (delegate {
			EventManager.Instance.TriggerEvent(new MainMenuEvent());
			AudioManager.Instance.Play("button_click", false, 1f, 0f);

			Hide(GameConst.INTENT_HIDE_PAUSE_MAINMENU, false);
		});

		// resume from pause
		PauseMenu_ResumeBtn.onClick.AddListener (delegate {
			EventManager.Instance.TriggerEvent(new ResumeGameEvent());
			Hide(GameConst.INTENT_HIDE_PAUSE_RESUME, false);
		});

	}

}
