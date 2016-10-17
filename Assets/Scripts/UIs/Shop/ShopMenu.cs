using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour {

	[SerializeField] Text _CoinText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCoin ();
	}

	void UpdateCoin() {
		_CoinText.text = GameSaveManager.Instance.GetCoins ().ToString ();
	}
}
