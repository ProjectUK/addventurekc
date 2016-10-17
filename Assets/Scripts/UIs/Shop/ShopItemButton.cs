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

	public List<PriceModel> Prices = new List<PriceModel>();

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
			EventManager.Instance.TriggerEvent (new BuyItemEvent (_CurrentPriceModel.ID));
		});
	}

	public void Init() {
		_RectTransform = GetComponent <RectTransform> ();
		_InitSize = _RectTransform.sizeDelta;

		UpdateCurrentPrice ();
	}

	void UpdateCurrentPrice() {
		// Set the price if it has multiple price count
		if (Prices.Count > 1) {

			//Get from savemanager the highest purchased item level
			Prices.Sort ((PriceModel x, PriceModel y) => {
				return x.Level.CompareTo (y.Level);
			});

			bool foundHighestPurchasedItem = false;
			for (int itPrices = Prices.Count - 1; itPrices > 0; itPrices--) {
				PriceModel priceModel = Prices [itPrices];
				if (GameSaveManager.Instance.CheckPurchase (priceModel.ID)) {

					if (itPrices < Prices.Count - 1) {
						PriceModel nextPriceModel = Prices [itPrices + 1];
						_CurrentPriceModel = nextPriceModel;
					} else {
						// udah full, diapain?
						_CurrentPriceModel = new PriceModel();
						_CurrentPriceModel.Price = -1;
						_CurrentPriceModel.Level = -1;
						_CurrentPriceModel.ID = "FULL";

						BuyButton.interactable = false;
					}

					foundHighestPurchasedItem = true;
					break;
				}
			}

			// default 0, then choose 1 (the next higher rank)
			if (!foundHighestPurchasedItem) {
				_CurrentPriceModel = Prices [1];
			} else {
			}

			// set price
			PriceText.text = _CurrentPriceModel.Price.ToString ();

		} else {
			// cek udah beli aja
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
		if (_CurrentPriceModel.ID == eve.ItemId) {

			//TODO: CEK DUITNYA ADA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			if (GameSaveManager.Instance.CheckCoinSufficient(_CurrentPriceModel.Price) ) {
				// kurangi duitnya
				GameSaveManager.Instance.DecreaseCoins(_CurrentPriceModel.Price);

				GameSaveManager.Instance.SetPurchase (eve.ItemId);

				// ubah jadi next price
				UpdateCurrentPrice();
			}
		}
	}

}
