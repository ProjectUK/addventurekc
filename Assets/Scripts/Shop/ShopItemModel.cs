using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShopItemModel{
	public string Name;
	public string Info;
	public string Image;
	public List<PriceModel> Prices = new List<PriceModel>();
}
