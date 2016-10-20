using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOShopMenu : UISliding {

	[Header("UI References")]
	public Button ShopMenu_BackBtn;

	public override void Start ()
	{
		base.Start ();

		// set this overlay name
		UIOName = GameConst.UIO_SHOP_MENU;

		ShopMenu_BackBtn.onClick.AddListener (delegate {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			Hide(GameConst.INTENT_HIDE_SHOP_MAINMENU, false);
		});
	}

}
