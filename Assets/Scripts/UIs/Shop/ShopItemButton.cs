using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class ShopItemButton : MonoBehaviour {

	const string ImageAssetPath = "Images/Shop/";


	[SerializeField] Ease ShowEase;
	[SerializeField] Ease HideEase;
	[SerializeField] float _ShowTime = 0.5f;
	[SerializeField] float _HideTime = 0.5f;


	[SerializeField] Button _Button;
	[SerializeField] RectTransform _ExpandedSize;
	[SerializeField] CanvasGroup _DetailsGroup;

	RectTransform _RectTransform;

	bool _Enlarged;
	Vector2 _InitSize;

	[Header("UI")]
	public Button BuyButton;
	public Text NameText;
	public Text DetailsText;
	public Text PriceText;
	public Image ItemImage;

//	public List<PriceModel> Prices = new List<PriceModel>();
	public ShopItemModel ShopItem;

	PriceModel _CurrentPriceModel;

	// Use this for initialization
	void Start () {
		
		Init ();

		_DetailsGroup.interactable = false;
		_DetailsGroup.blocksRaycasts = false;
		_DetailsGroup.alpha = 0;

		_Button.onClick.AddListener (() => {
			ToggleEnlargeRectTransform();
		});

		EventManager.Instance.AddListener<BuyItemEvent> (OnBuyItemEvent);

		// set buy button action
		BuyButton.onClick.AddListener (() => {
			EventManager.Instance.TriggerEvent (new BuyItemEvent (ShopItem.Name));
		});
	}

	public void Init() {
		_RectTransform = GetComponent <RectTransform> ();
		_InitSize = _RectTransform.sizeDelta;

		UpdateCurrentPrice ();
	}

	void UpdateCurrentPrice() {
		// Set the price if it has multiple price count
		if (ShopItem.Prices.Count > 1) {

			int currentItemLevel = GameSaveManager.Instance.GetPurchaseLevel (ShopItem.ID);
			_CurrentPriceModel = ShopItem.GetPurchasedLevel(currentItemLevel+1);

			// reached highest level
			if (_CurrentPriceModel.Level == -1) {
				BuyButton.interactable = false;
			}

			// set price
			PriceText.text = _CurrentPriceModel.Price.ToString ();

		} else {
			// cek udah beli aja karena cuma ada 1 level

			_CurrentPriceModel = ShopItem.GetPurchasedLevel (0);

			bool purchased = GameSaveManager.Instance.CheckPurchase (ShopItem.ID);
			if (purchased) {
				BuyButton.interactable = false;
				PriceText.text = "";
			} else {
				BuyButton.interactable = true;
				PriceText.text = _CurrentPriceModel.Price.ToString ();
			}

		}
	}


	void ToggleEnlargeRectTransform(){
		Vector2 prevSize = _RectTransform.sizeDelta;

		if (!_Enlarged) {

			// enlarge size
			_RectTransform.DOSizeDelta (new Vector2 (
				_RectTransform.sizeDelta.x,
				_ExpandedSize.sizeDelta.y), _ShowTime, false).SetUpdate(true).SetEase(ShowEase);

			float prevAlpha = _DetailsGroup.alpha;

			// show detail
			DOTween.To (() => prevAlpha,
				x => _DetailsGroup.alpha = x,
				_DetailsGroup.alpha = 1f,
				_ShowTime).OnComplete(()=>{
					_DetailsGroup.interactable = true;
					_DetailsGroup.blocksRaycasts = true;
				}).SetUpdate(true).SetEase(ShowEase);


			_Enlarged = true;
		} else {

			_RectTransform.DOSizeDelta (new Vector2 (
				prevSize.x,
				_InitSize.y), _HideTime, false).SetUpdate(true).SetEase(HideEase);

			float prevAlpha = _DetailsGroup.alpha;

			// hide detail
			DOTween.To (() => prevAlpha,
				x => _DetailsGroup.alpha = x,
				_DetailsGroup.alpha = 0f,
				_HideTime).OnComplete(()=>{
					_DetailsGroup.interactable = false;
					_DetailsGroup.blocksRaycasts = false;
				}).SetUpdate(true).SetEase(HideEase);

			_Enlarged = false;
		}
	}

	public void SetPosition(Vector2 position) {
		_RectTransform.anchoredPosition = position;
	}

	public Vector2 GetPosition() {
		return _RectTransform.anchoredPosition;
	}

	public Vector2 GetSize() {
		return _RectTransform.sizeDelta;
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
