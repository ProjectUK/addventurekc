using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

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

	// Use this for initialization
	void Start () {
		
		Init ();

		_DetailsGroup.interactable = false;
		_DetailsGroup.blocksRaycasts = false;
		_DetailsGroup.alpha = 0;

		_Button.onClick.AddListener (() => {
			ToggleEnlargeRectTransform();
		});
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Init() {
		_RectTransform = GetComponent <RectTransform> ();
		_InitSize = _RectTransform.sizeDelta;
	}

	void ToggleEnlargeRectTransform(){
//		_RectTransform.sizeDelta

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

}
