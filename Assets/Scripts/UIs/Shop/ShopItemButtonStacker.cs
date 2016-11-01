using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopItemButtonStacker : MonoBehaviour {

	const string BUTTON_PREFAB = "Prefabs/UIs/Cabinet";

	ShopCabinet[] _ShopCabinets;

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
		int itItem = 0;

		GameObject ShopCabinetObj = null;
		ShopCabinet shopCabinet = null;

		for (int itShopItem = 0; itShopItem < _ShopItemData.ShopItems.Count; itShopItem++) {

			ShopItemModel shopItemModel = _ShopItemData.ShopItems [itShopItem];

			if (itItem == 0) {

				ShopCabinetObj = Instantiate (Resources.Load (BUTTON_PREFAB) as GameObject);
				ShopCabinetObj.transform.SetParent (this.transform);
				RectTransform rt = ShopCabinetObj.GetComponent<RectTransform> ();
				rt.localScale = new Vector3 (1, 1, 1);
				rt.sizeDelta = new Vector2 (rt.sizeDelta.x, rt.sizeDelta.y);

				shopCabinet = ShopCabinetObj.GetComponent<ShopCabinet> ();

				shopCabinet.Item1.ShopItem = shopItemModel;
				shopCabinet.Item1.Init ();
			}else{
				shopCabinet.Item2.ShopItem = shopItemModel;
				shopCabinet.Item2.Init ();
			}

			if (itItem < ShopCabinet.MAX_ITEM-1) {
				itItem++;
			}else{
				itItem = 0;
			}
				
		}
			
		_ShopCabinets = GetComponentsInChildren<ShopCabinet> ();

		yield return null;

		_IsItemLoaded = true;
	}

	void Update () {
		if (_IsItemLoaded) {
			float prevHeight = 0;
			float prevYPos = 0;
			float totalHeight = 0;
			for (int i = 0; i < _ShopCabinets.Length; i++) {
				ShopCabinet button = _ShopCabinets [i];
				button.SetPosition (new Vector2 (0, -prevHeight + prevYPos));
				
				prevHeight = button.GetSize ().y;
				prevYPos = button.GetPosition ().y;
				
				totalHeight += button.GetSize ().y;
			}
			_RectTransform.sizeDelta = new Vector2 (_RectTransform.sizeDelta.x, totalHeight);
		}
	}
}
