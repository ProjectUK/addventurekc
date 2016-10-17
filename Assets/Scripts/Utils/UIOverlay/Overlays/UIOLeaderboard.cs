using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOLeaderboard : UISliding {

	[Header("UI References")]
	public Button Leaderboard_BasecampBtn;

	public override void Start ()
	{
		base.Start ();
		// set this overlay name
		UIOName = GameConst.UIO_LEADERBOARD;

		Leaderboard_BasecampBtn.onClick.AddListener(delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Hide(GameConst.INTENT_HIDE_LEADERBOARD_MAINMENU);
		});

	}


}
