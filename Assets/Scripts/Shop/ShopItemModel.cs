using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShopItemModel{
	public string ID;
	public string Name;
	public string Info;
	public string Image;
	public List<PriceModel> Prices = new List<PriceModel>();

	public PriceModel GetPurchasedLevel(int level) {

		PriceModel foundPriceModel = null;
		bool foundPurchasedItem = false;
		int itPurchase = 0;
		while (!foundPurchasedItem && itPurchase < Prices.Count) {
			PriceModel priceModel = Prices [itPurchase];
			if (priceModel.Level == level) {

				// copy found price model
				foundPriceModel = new PriceModel();
				foundPriceModel.Level = priceModel.Level;
				foundPriceModel.Price = priceModel.Price;

				foundPurchasedItem = true;
			}
			itPurchase++;
		}

		if (foundPurchasedItem) {
			return foundPriceModel;
		} else {
			// when not found price model it means it's the highest
			foundPriceModel = new PriceModel();
			foundPriceModel.Level = -1;
			foundPriceModel.Price = -1;
			return foundPriceModel;
		}

	}

//	public PriceModel GetHighestPurchasedLevel() {
//		Prices.Sort ((PriceModel x, PriceModel y) => {
//			return x.Level.CompareTo (y.Level);
//		});
//
//		PriceModel _CurrentPriceModel;
//		bool foundHighestPurchasedItem = false;
//		for (int itPrices = Prices.Count - 1; itPrices > 0; itPrices--) {
//			PriceModel priceModel = Prices [itPrices];
//			if (GameSaveManager.Instance.CheckPurchase (priceModel.ID)) {
//
//				if (itPrices < Prices.Count - 1) {
//					PriceModel nextPriceModel = Prices [itPrices + 1];
//					_CurrentPriceModel = nextPriceModel;
//				} else {
//					// udah full, diapain?
//					_CurrentPriceModel = new PriceModel();
//					_CurrentPriceModel.Price = -1;
//					_CurrentPriceModel.Level = -1;
//					_CurrentPriceModel.ID = "FULL";
//				}
//
//				foundHighestPurchasedItem = true;
//				break;
//			}
//		}
//
//		if (!foundHighestPurchasedItem) {
//			_CurrentPriceModel = Prices [1];
//		} 
//
//		return _CurrentPriceModel;
//	}
}
