using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItemButtonStacker : MonoBehaviour {

	const string BUTTON_PREFAB = "Prefabs/UIs/ShopItem";

	ShopItemButton[] _ShopItems;
	RectTransform _RectTransform;

	[SerializeField] ShopItemData _ShopItemData;

	bool _IsItemLoaded = false;

	// Use this for initialization
	void Start () {
		_IsItemLoaded = false;
		_RectTransform = GetComponent<RectTransform> ();
		StartCoroutine (LoadItemsRoutine ());
	}

	IEnumerator LoadItemsRoutine() {
		for (int itShopItem = 0; itShopItem < _ShopItemData.ShopItems.Count; itShopItem++) {

			ShopItemModel shopItemModel = _ShopItemData.ShopItems [itShopItem];

			GameObject ShopButton = Instantiate (Resources.Load (BUTTON_PREFAB) as GameObject);
			ShopButton.transform.SetParent(this.transform);

			RectTransform rt = ShopButton.GetComponent<RectTransform> ();
			rt.localScale = new Vector3 (1, 1, 1);
			rt.sizeDelta = new Vector2 (0, rt.sizeDelta.y);

			ShopItemButton sib = ShopButton.GetComponent<ShopItemButton> ();

			//TODO: Image
//			sib.ItemImage.....
			sib.ShopItem = shopItemModel;
			//			sib.Prices = shopItemModel.Prices;
			sib.SetImage (shopItemModel.Image);
			sib.DetailsText.text = shopItemModel.Info;
			sib.NameText.text = shopItemModel.Name;

			sib.Init ();




		}

		_ShopItems = GetComponentsInChildren<ShopItemButton> ();
		yield return null;

		_IsItemLoaded = true;
	}

	void Update () {
		if (_IsItemLoaded) {
			float prevHeight = 0;
			float prevYPos = 0;
			float totalHeight = 0;
			for (int i = 0; i < _ShopItems.Length; i++) {
				ShopItemButton button = _ShopItems [i];
				button.SetPosition (new Vector2 (0, -prevHeight + prevYPos));
				
				prevHeight = button.GetSize ().y;
				prevYPos = button.GetPosition ().y;
				
				totalHeight += button.GetSize ().y;
			}
			_RectTransform.sizeDelta = new Vector2 (_RectTransform.sizeDelta.x, totalHeight);
		}
	}
}
