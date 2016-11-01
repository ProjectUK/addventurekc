using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuyNotification : MonoBehaviour {

	const string ImageAssetPath = "Images/Shop/";

	CanvasGroup _CanvasGroup;

	public ShopItemModel ShopItem;

	[SerializeField] Button BuyButton;
	[SerializeField] Text PriceText;
	[SerializeField] Button CloseButton;

	[SerializeField] Image ItemImage;
	[SerializeField] Text ItemInfo;
	[SerializeField] Text ItemName;


	// Use this for initialization
	void Start () {
		_CanvasGroup = GetComponent<CanvasGroup> ();
		Disable ();


		CloseButton.onClick.AddListener (() => {
			Disable();
		});

		BuyButton.onClick.AddListener (() => {
			BuyItem();
		});

		EventManager.Instance.AddListener<DetailItemEvent> (OnDetailItemEvent);

	}

	void Disable () {
		_CanvasGroup.alpha = 0;
		_CanvasGroup.interactable = false;
		_CanvasGroup.blocksRaycasts = false;
	}

	void Enable () {
		_CanvasGroup.alpha = 1;
		_CanvasGroup.interactable = true;
		_CanvasGroup.blocksRaycasts = true;
	}

	public void Init() {
		SetImage (ShopItem.Image);
		ItemInfo.text = ShopItem.Info;
		ItemName.text = ShopItem.Name;
	}

	public void SetImage(string imageName) {
		Texture2D loadedTexture2D = Resources.Load (ImageAssetPath+imageName) as Texture2D;
		if (loadedTexture2D != null) {
			Sprite loadedSprite = Sprite.Create (loadedTexture2D, new Rect (0,0, loadedTexture2D.width, loadedTexture2D.height), Vector2.zero);
			ItemImage.sprite = loadedSprite;
		}
	}

	void OnDetailItemEvent(DetailItemEvent eve) {
		Debug.Log("Detail Item diterima:"+eve.ShopItem.Name);
		this.ShopItem = eve.ShopItem;
		Init ();
		Enable ();
		UpdateCurrentPrice ();
	}

	void BuyItem() {
		EventManager.Instance.TriggerEvent (new BuyItemEvent (ShopItem.Name));
		UpdateCurrentPrice ();
	}

	PriceModel _CurrentPriceModel;

	void UpdateCurrentPrice() {
		// Set the price if it has multiple price count
		if (ShopItem.Prices.Count > 1) {

			int currentItemLevel = GameSaveManager.Instance.GetPurchaseLevel (ShopItem.ID);
			_CurrentPriceModel = ShopItem.GetPurchasedLevel(currentItemLevel+1);

			// reached highest level
			if (_CurrentPriceModel.Level <= -1) {
				BuyButton.gameObject.SetActive(false);
			} else {
				BuyButton.gameObject.SetActive(true);
			}

			// set price
			PriceText.text = _CurrentPriceModel.Price.ToString ();

		} else {
			// cek udah beli aja karena cuma ada 1 level

			_CurrentPriceModel = ShopItem.GetPurchasedLevel (0);

			bool purchased = GameSaveManager.Instance.CheckPurchase (ShopItem.ID);
			if (purchased) {
				BuyButton.gameObject.SetActive(false);
				PriceText.text = "";
			} else {
				BuyButton.gameObject.SetActive(true);
				PriceText.text = _CurrentPriceModel.Price.ToString ();
			}

		}

		// set coin actives
		int currentCoin = GameSaveManager.Instance.GetCoins ();
		if (currentCoin >= _CurrentPriceModel.Price) {
			BuyButton.gameObject.SetActive (true);
		} else {
			BuyButton.gameObject.SetActive (false);
		}

	}
}
