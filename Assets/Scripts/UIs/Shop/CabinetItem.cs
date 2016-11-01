using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CabinetItem : MonoBehaviour {

	public CanvasGroup ItemCanvasGroup;
	public Button ItemButton;
	public Image ItemImage;
	public Text ItemText;
	public Text ItemPrice;

	const string ImageAssetPath = "Images/Shop/";

	//	public List<PriceModel> Prices = new List<PriceModel>();
	public ShopItemModel ShopItem;

	PriceModel _CurrentPriceModel;

	CanvasGroup _CanvasGroup;


	// Use this for initialization
	void Start () {

		_CanvasGroup = GetComponent<CanvasGroup> ();

		EventManager.Instance.AddListener<BuyItemEvent> (OnBuyItemEvent);

		ItemButton.onClick.AddListener (() => {
			EventManager.Instance.TriggerEvent(new DetailItemEvent(ShopItem));
			Debug.Log("Detail Item dari");
		});

	}

	public void Init() {

		ItemText.text = ShopItem.Name;
		SetImage (ShopItem.Image);

		UpdateCurrentPrice ();

		if (ShopItem == null) {
			_CanvasGroup.alpha = 0;
			_CanvasGroup.interactable = false;
			_CanvasGroup.blocksRaycasts = false;
		}
	}

	void UpdateCurrentPrice() {
		// Set the price if it has multiple price count
		if (ShopItem.Prices.Count > 1) {

			int currentItemLevel = GameSaveManager.Instance.GetPurchaseLevel (ShopItem.ID);
			_CurrentPriceModel = ShopItem.GetPurchasedLevel(currentItemLevel+1);

			// reached highest level
			if (_CurrentPriceModel.Level == -1) {
//				_MaxedOut = false;
			}

			// set price
			ItemPrice.text = _CurrentPriceModel.Price.ToString ();

		} else {
			// cek udah beli aja karena cuma ada 1 level

			_CurrentPriceModel = ShopItem.GetPurchasedLevel (0);

			bool purchased = GameSaveManager.Instance.CheckPurchase (ShopItem.ID);
			if (purchased) {
				ItemPrice.text = "";
			} else {
				ItemPrice.text = _CurrentPriceModel.Price.ToString ();
			}

		}
	}
		
	public void SetImage(string imageName) {
		Texture2D loadedTexture2D = Resources.Load (ImageAssetPath+imageName) as Texture2D;
		if (loadedTexture2D != null) {
			Sprite loadedSprite = Sprite.Create (loadedTexture2D, new Rect (0,0, loadedTexture2D.width, loadedTexture2D.height), Vector2.zero);
			ItemImage.sprite = loadedSprite;
		}
	}

	void OnBuyItemEvent(BuyItemEvent eve) {
		if (ShopItem.Name == eve.ItemName) {

			//TODO: CEK DUITNYA ADA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			if (GameSaveManager.Instance.CheckCoinSufficient(_CurrentPriceModel.Price) ) {

				// cek multilevel
				if (ShopItem.Prices.Count > 1) {
					// Cek level sekarang belum melebihi
					int previousLevel = GameSaveManager.Instance.GetPurchaseLevel (ShopItem.ID);
					if (previousLevel < ShopItem.Prices.Count - 1) {
						// kurangi duitnya
						GameSaveManager.Instance.DecreaseCoins (_CurrentPriceModel.Price);

						// set levelnya
						int currentLevel = previousLevel + 1;

						GameSaveManager.Instance.SetPurchase (ShopItem.ID, currentLevel);

						// ubah jadi next price
						UpdateCurrentPrice ();

						Debug.Log ("Beli level");
					}
				} else {
					// satu level aja

					// kurangi duitnya
					GameSaveManager.Instance.DecreaseCoins (_CurrentPriceModel.Price);
					GameSaveManager.Instance.SetPurchase (ShopItem.ID, 1);

					UpdateCurrentPrice ();

					Debug.Log ("Beli satuan");

				}


			}
		}
	}
}
